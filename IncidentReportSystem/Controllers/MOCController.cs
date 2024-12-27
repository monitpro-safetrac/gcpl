using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MonitPro.Validations;
using MonitPro.Models;
using MonitPro.BLL;
using MonitPro.Models.Incident;
using MonitPro.Models.CAPA;
using MonitPro.Models.MOC;
using System.IO;
using MonitPro.Models.Account;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using ClosedXML.Excel;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace WorkPermitSystem.Controllers
{
    [ValidateSession]
    public class MOCController : Controller
    {

        AdminBLL adminBLL = new AdminBLL();
        MOCBLL MocBLL = new MOCBLL();
        MOCListViewModel MOCViewModel = new MOCListViewModel();
        List<UserProfile> UserProfiles = new List<UserProfile>();
        List<Role> UserRoles = new List<Role>();
        List<Plants> IncidentPlants = new List<Plants>();
        List<MOCClassification> MocClass = new List<MOCClassification>();
        List<MOCType> Moctype = new List<MOCType>();
        List<MOCCategory> MocCategory = new List<MOCCategory>();
        List<MOCStatus> mocstatus = new List<MOCStatus>();
        List<MOCRecomPriority> recomPriority = new List<MOCRecomPriority>();
        List<MOCRecomCategory> recomcategory = new List<MOCRecomCategory>();
        List<MOCPriority> mocpriority = new List<MOCPriority>();
        List<CAPAObservationStatus> TrackActionStatus = new List<CAPAObservationStatus>();


        public MOCController()
        {
            IncidentPlants = MocBLL.GetPlants();
            MocClass = MocBLL.GetMOCClassification();
            Moctype = MocBLL.GetMOCType();
            MocCategory = MocBLL.GetMOCCategory();
            mocstatus = MocBLL.GetMOCStatus();
            UserProfiles = MocBLL.GetMOCApprover();
            mocpriority = MocBLL.GetMOCPriority();
            recomPriority = MocBLL.GetMOCRecomPriority();
            recomcategory = MocBLL.GetMOCRecomCategory();
            TrackActionStatus = MocBLL.GetCAPAObservationStatus();
        }

        [HttpGet]
        public ActionResult CreateMOC(int MOCID = 0)
        {
            NewMOCModel NewMOC = new NewMOCModel();
            MOCa moca = new MOCa();
            if (MOCID > 0)
            {
                moca = MocBLL.GetMOC(MOCID);

                NewMOC.moca = moca;
            }
            else
            {
                var moccategory1 = MocCategory.Where(y => y.ID != -1).ToList();
                moca.mocCategory = moccategory1;
                moca.GetMocReasonForChange = MocBLL.GetMOCReasonForChanges();
                moca.CreatedBy = CurrentUser.FullName;
                NewMOC.moca = moca;
            }


            NewMOC.Roles = CurrentUser.Roles;
            NewMOC.UserFullName = CurrentUser.FullName;
            NewMOC.ProfileImage = CurrentUser.ProfileImage;
            NewMOC.CurrentUserID = CurrentUser.UserID;
            NewMOC.IsRestrict = CurrentUser.IsRestrict;
            var deptid = CurrentUser.DepartmentID;
            var dept = MocBLL.GetMOCFunMgr(deptid);
            var DeptManager = dept;
            if (NewMOC.moca.MOCFunCMgrID > 0)
            {
                 DeptManager = dept.Where(y => y.ID != 0).ToList();
            }
            else
            {
                DeptManager = dept.Where(y => y.ID != 0 && y.ID != NewMOC.CurrentUserID).ToList();
            }
            var plants = IncidentPlants.Where(y => y.ID != -1).ToList();

            var mocclass = MocClass.Where(y => y.ID != -1).ToList();
            var Mtype = Moctype.Where(y => y.ID != -1).ToList();
            moca.CreatedDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            NewMOC.DeptManagerList = DeptManager;
            NewMOC.PlantList = plants;
            
            var Equipmentdd = adminBLL.EquipmentDD();
            ViewBag.EquipmentList = new SelectList(Equipmentdd, "EquipmentID", "EquipmentName");
            NewMOC.mocclass = mocclass;
            NewMOC.moctype = Mtype;

            return View(NewMOC);
        }

       
        [HttpPost]
      
        public ActionResult CreateMOC(NewMOCModel newmoc)
        {
            newmoc.CurrentUserID = CurrentUser.UserID;
            MOCa moca = new MOCa();
            newmoc.Roles = CurrentUser.Roles;
            newmoc.UserFullName = CurrentUser.FullName;
            newmoc.ProfileImage = CurrentUser.ProfileImage;

            newmoc.IsRestrict = CurrentUser.IsRestrict;
            newmoc.moca.CreatedDate= DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            var deptid = CurrentUser.DepartmentID;
            var dept = MocBLL.GetMOCFunMgr(deptid);
            var DeptManager = dept.Where(y => y.ID != 0).ToList();
            var plants = IncidentPlants.Where(y => y.ID != -1).ToList();
            var moccategory = MocCategory.Where(y => y.ID != -1).ToList();
            var mocclass = MocClass.Where(y => y.ID != -1).ToList();
            var Mtype = Moctype.Where(y => y.ID != -1).ToList();
            if (newmoc.moca.ImageFile != null)
            {

                var fileName = Path.GetFileName(newmoc.moca.ImageFile.FileName);
                var path = Path.Combine(Server.MapPath("~/MOCAttachments/"), fileName);
                newmoc.moca.ImageFile.SaveAs(path);
            }
            moca.CreatedBy = CurrentUser.FullName;
            moca.MOCID = MocBLL.MOCInsertUpdate(newmoc, CurrentUser.UserID);
            moca = MocBLL.GetMOC(moca.MOCID);
            if ((newmoc.moca.MOCStatusID == 1) && (newmoc.moca.MOCStatusIdentify == "20"))
            {
                ViewBag.Message = string.Format("MOC is Created Successfully");
            }
            else if (newmoc.moca.MOCStatusID == 2)
            {
                ViewBag.FunctionalApprove = string.Format(" MOC has been submitted successfully ");
            }
            else if (newmoc.moca.MOCStatusID == 3)
            {
                ViewBag.FunctionalApprove = string.Format("{0} has been Approved", moca.MOCNumber);
            }
            else if ((newmoc.moca.MOCStatusID == 1) && (newmoc.moca.MOCStatusIdentify == "20"))
            {
                ViewBag.Reprocess = string.Format("MOC is Reprocessed");
            }
                    moca.CreatedDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            var Equipmentdd = adminBLL.EquipmentDD();
            ViewBag.EquipmentList = new SelectList(Equipmentdd, "EquipmentID", "EquipmentName");
            newmoc.DeptManagerList = DeptManager;
            newmoc.PlantList = plants;
            moca.mocCategory = moccategory;
            newmoc.mocclass = mocclass;
            newmoc.moctype = Mtype;
            newmoc.moca = moca;
            return View(newmoc);
        }
        [HttpGet]
        public ActionResult EditMoc(int MOCID = 0)
        {
            NewMOCModel NewMOC = new NewMOCModel();
            MOCa moca = new MOCa();
            MOCa moca1 = new MOCa();
            if (MOCID > 0)
            {
                moca = MocBLL.GetMOC(MOCID);
               
            }
            var prioritylist = mocpriority.Where(y => y.ID != -1).ToList();

            NewMOC.AllMOCOBserList = MocBLL.GetAllMOCObservation().Where(y => (y.CategoryName == "Design Review" && y.PriorityID == 1) || y.CategoryName == "Risk Assessment").ToList();
           var PSSRObserList = MocBLL.GetAllMOCObservation().Where(y => y.CategoryName == "PSSR").ToList();
            foreach (var item in NewMOC.AllMOCOBserList)
            {
                if(MOCID == item.MOCID)
                {
                   NewMOC.RecomID =  item.ObservationID;
                }
            }
            foreach(var item1 in PSSRObserList)
            {
                if(MOCID == item1.MOCID)
                {
                    NewMOC.PSSRRecomID = item1.ObservationID;
                }
            }
            NewMOC.MocPSSRObserList = MocBLL.CheckPSSRCriticalRecomm(MOCID);
            NewMOC.MocObserList = MocBLL.CheckCriticalRecomm(MOCID);
            NewMOC.ApprovalStage = MocBLL.GetApprovalStageApprovar(MOCID);
            NewMOC.ApproverUserID = moca.ApproverUserID;
            NewMOC.ApprovalStageID = moca.ApproverStageID;

            NewMOC.Roles = CurrentUser.Roles;
            foreach (var i in NewMOC.Roles)
            {
                NewMOC.RoleID = i.RoleID;
            }
            NewMOC.UserFullName = CurrentUser.FullName;
            NewMOC.ProfileImage = CurrentUser.ProfileImage;
            NewMOC.Prioritylist = prioritylist;
            var deptid = CurrentUser.DepartmentID;
            var dept = MocBLL.GetMOCApprover();
           
            NewMOC.FunMgrList = dept;
            NewMOC.PlantList = IncidentPlants;
            var Equipmentdd = adminBLL.EquipmentDD();
            ViewBag.EquipmentList = new SelectList(Equipmentdd, "EquipmentID", "EquipmentName");
            ViewBag.Consequences = new SelectList("012345");

            ViewBag.Likelihood = new SelectList("ABCDE");
            NewMOC.CurrentUserID = CurrentUser.UserID;
            var dept1 = MocBLL.GetMOCFunMgr(deptid);
            var DeptManager = dept1.Where(y => y.ID != 0).ToList();
            NewMOC.DeptManagerList = DeptManager;
            NewMOC.IsRestrict = CurrentUser.IsRestrict;
            NewMOC.mocclass = MocClass;
            NewMOC.moctype = Moctype;
            NewMOC.moca = moca;
            return View(NewMOC);
        }

        [HttpPost]
        public ActionResult EditMoc(NewMOCModel newmoc)
        {
            MOCa moca = new MOCa();
            moca = MocBLL.GetMOC(newmoc.moca.MOCID);
            if (newmoc.moca.ImageFile != null && newmoc.moca.MOCStatusIdentify == "80")
            {

                var fileName = Path.GetFileName(newmoc.moca.ImageFile.FileName);
                var path = Path.Combine(Server.MapPath("~/MOCAttachments/"), fileName);
                newmoc.moca.ImageFile.SaveAs(path);
     
                moca.MOCID = MocBLL.MOCInsertUpdate(newmoc, CurrentUser.UserID);

            }

            if (newmoc.moca.ImageFile == null && newmoc.moca.MOCStatusIdentify == "80")
            {
                moca.MOCID = MocBLL.MOCInsertUpdate(newmoc, CurrentUser.UserID);
            }

            if (moca.MOCID > 0 && newmoc.moca.MOCStatusIdentify == "80")
            {
          
                ViewBag.Message = string.Format("{0} is Saved Successfully", moca.MOCNumber);
            }
            
            newmoc.MocPSSRObserList = MocBLL.CheckPSSRCriticalRecomm(moca.MOCID);
            newmoc.MocObserList = MocBLL.CheckCriticalRecomm(moca.MOCID);
            newmoc.ApprovalStage = MocBLL.GetApprovalStageApprovar(moca.MOCID);
            newmoc.MOCID = moca.MOCID;
            var prioritylist = mocpriority.Where(y => y.ID != -1).ToList();
            var moccategory = MocCategory.Where(y => y.ID != -1).ToList();
            newmoc.Roles = CurrentUser.Roles;
            newmoc.UserFullName = CurrentUser.FullName;
            newmoc.ProfileImage = CurrentUser.ProfileImage;
            var deptid = CurrentUser.DepartmentID;
            newmoc.Prioritylist = prioritylist;
            var dept = MocBLL.GetMOCApprover();

            newmoc.FunMgrList = dept;
            newmoc.CurrentUserID = CurrentUser.UserID;

            newmoc.IsRestrict = CurrentUser.IsRestrict;
            newmoc.PlantList = IncidentPlants;
            moca.mocCategory = moccategory;
            newmoc.mocclass = MocClass;
            newmoc.moctype = Moctype;
            newmoc.moca = moca;
            return RedirectToAction("MOCList");
        }


        public ActionResult MOCList()
        {
            MOCViewModel.MOCList = MocBLL.GetOpenMOC();
            MOCViewModel.ApproverModel = MocBLL.GetApproverForMOCPageList();

            MOCViewModel.MOCObserList = MocBLL.GetAllMOCObservation();
            MOCViewModel.CurrentUser = CurrentUser.UserID;
            var status = mocstatus.Where(y => y.ID != 11 && y.ID !=13).ToList();
            MOCViewModel.MocstatusList = status;

            ViewBag.IncidentPlant = new SelectList(IncidentPlants, "ID", "Name");
            var approver = MocBLL.GetMOCApprover();
            MOCViewModel.MOCClass = MocClass;
            MOCViewModel.approver = approver;
            MOCViewModel.Coordinator = UserProfiles;
            MOCViewModel.MocCategory = MocCategory;
            MOCViewModel.MocType = Moctype;
            MOCViewModel.Roles = CurrentUser.Roles;
            MOCViewModel.UserFullName = CurrentUser.FullName;
            MOCViewModel.ProfileImage = CurrentUser.ProfileImage;

            MOCViewModel.IsRestrict = CurrentUser.IsRestrict;
            MOCSearchViewModel mocsearch = new MOCSearchViewModel();
            ViewBag.fromdate = mocsearch.MOCFromDate;
            ViewBag.Todate = mocsearch.MOCToDate;
            ViewBag.PlantID = mocsearch.Plant;
            ViewBag.moccategory = mocsearch.MOCCategory;
            ViewBag.moctype = mocsearch.MOCType;
            ViewBag.mocclass = mocsearch.ClassID;
            ViewBag.moccor = mocsearch.MOCcoordinator;
            ViewBag.actionerid = mocsearch.ActionerID;
            ViewBag.mstatus = mocsearch.MOCStatus;
            return View(MOCViewModel);
        }

        [HttpPost]
     
        public ActionResult MOCList(MOCSearchViewModel Mocsearch)
        {

            MOCViewModel.MOCList = MocBLL.SearchOpenMOC(Mocsearch);
            var status = mocstatus.Where(y => y.ID != 11 && y.ID != 13).ToList();

            MOCViewModel.MOCObserList = MocBLL.GetAllMOCObservation();
            MOCViewModel.MocstatusList = status;
            var approver = MocBLL.GetMOCApprover();
            ViewBag.IncidentPlant = new SelectList(IncidentPlants, "ID", "Name");
            MOCViewModel.ApproverModel = MocBLL.GetApproverForMOCPageList();
            MOCViewModel.approver = approver;
            MOCViewModel.MOCClass = MocClass;
            MOCViewModel.Coordinator = UserProfiles;
            MOCViewModel.MocCategory = MocCategory;
            MOCViewModel.MocType = Moctype;
            MOCViewModel.MOCSearchVM.Plant = Mocsearch.Plant;
            MOCViewModel.MOCSearchVM.MOCFromDate = Mocsearch.MOCFromDate;
            MOCViewModel.MOCSearchVM.MOCToDate = Mocsearch.MOCToDate;
            MOCViewModel.MOCSearchVM.MOCType = Mocsearch.MOCType;
            MOCViewModel.MOCSearchVM.MOCCategory = Mocsearch.MOCCategory;
            MOCViewModel.MOCSearchVM.ClassID = Mocsearch.ClassID;
            MOCViewModel.MOCSearchVM.MOCcoordinator = Mocsearch.MOCcoordinator;
            MOCViewModel.MOCSearchVM.ActionerID = Mocsearch.ActionerID;
            MOCViewModel.MOCSearchVM.MOCStatus = Mocsearch.MOCStatus;
            MOCViewModel.Roles = CurrentUser.Roles;
            MOCViewModel.UserFullName = CurrentUser.FullName;
            MOCViewModel.ProfileImage = CurrentUser.ProfileImage;

            MOCViewModel.IsRestrict = CurrentUser.IsRestrict;
            ViewBag.fromdate = Mocsearch.MOCFromDate;
            ViewBag.Todate = Mocsearch.MOCToDate;
            ViewBag.PlantID = Mocsearch.Plant;
            ViewBag.moccategory = Mocsearch.MOCCategory;
            ViewBag.moctype = Mocsearch.MOCType;
            ViewBag.mocclass = Mocsearch.ClassID;
            ViewBag.moccor = Mocsearch.MOCcoordinator;
            ViewBag.actionerid = Mocsearch.ActionerID;
            ViewBag.mstatus = Mocsearch.MOCStatus;
            return View(MOCViewModel);
        }

        public ActionResult ExportMOCList(string currentFromDate, string currentEndDate, string moccor, int PlantID, int moccategory, int moctype, int mocclass, int mstatus, int actionerid)
        {

            using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "[ExportMOCList]";
                objCom.CommandType = CommandType.StoredProcedure;
                objCom.Parameters.AddWithValue("@MOCPlant", PlantID);
                objCom.Parameters.AddWithValue("@MOCStatus", mstatus);
                objCom.Parameters.AddWithValue("@FromDate", currentEndDate == null ? string.Empty : currentFromDate);
                objCom.Parameters.AddWithValue("@ToDate", currentEndDate == null ? string.Empty : currentEndDate);
                objCom.Parameters.AddWithValue("@MOCType", moctype);
                objCom.Parameters.AddWithValue("@MOCCategory", moccategory);
                objCom.Parameters.AddWithValue("@Class", mocclass);
                objCom.Parameters.AddWithValue("@Approver", actionerid);
                objCom.Parameters.AddWithValue("@MOCcoor", moccor);
                objCon.Open();
                objCom.Connection = objCon;
                SqlDataAdapter Adapter = new SqlDataAdapter();
                Adapter.SelectCommand = objCom;
                DataSet dataSet = new DataSet();
                Adapter.Fill(dataSet);
                objCon.Close();
                var wb = new XLWorkbook(Server.MapPath("~/Template/MOCList.xlsx"));
                var worksheet = wb.Worksheet("ClosedList");
                worksheet.Cell("C4").Value = "Report Generated by : " + CurrentUser.FullName;
                //worksheet.Cell("D4").Value = CurrentUser.FullName;
                worksheet.Cell("C5").Value = "Report Duration : " + currentFromDate + " to  " + currentEndDate;
                //worksheet.Cell("D5").Value = currentFromDate + " to  " + currentEndDate;

                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    var rangeWithDataSecond = worksheet.Cell(7, 1).InsertTable(dataSet.Tables[0].AsEnumerable());
                }
                else
                {
                    worksheet.Cell("C8").Value = "No Data Found";
                }

                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=MOCList.xlsx");
                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }

            }
            return View();
        }

        [HttpGet]
        public ActionResult MOCObservations(int MOCID = 0, int ObservID = 0)
        {
            MOCObservationViewModel mocobservationvm = new MOCObservationViewModel();
            List<ObservationViewModelMOC> observationmoclist = new List<ObservationViewModelMOC>();
            MOCObservation mocobservation = new MOCObservation();
            MOCa moca = new MOCa();
            moca = MocBLL.GetMOC(MOCID);

            observationmoclist = MocBLL.GetObservationModel(MOCID, ObservID);
            var action = MocBLL.GetActionList();
            //var Action = action.Where(y => y.UserID != 0).ToList();
            List<MOCRecomCategory> recomcategory = new List<MOCRecomCategory>();
            recomcategory = MocBLL.GetMOCRecomCategory();
            var recomcat = recomcategory.Where(y => y.ID != -1).ToList();

            var recompriority1 = MocBLL.GetMOCRecomPriority();
            var repriority = recompriority1.Where(y => y.ID != -1).ToList();
           
            mocobservationvm.CurrentUser = CurrentUser.UserID;
            mocobservation.ActionList = action;
            mocobservation.TargetDate = DateTime.Today.AddDays(30).ToString("dd/MM/yyyy");
            mocobservation.MOCID = MOCID;
            mocobservation.MOCNo = moca.MOCNumber;
            mocobservation.Recompriority = repriority;
            mocobservation.Recomcategory = recomcat;
            mocobservation.MOCDescription = moca.MOCDescription;
            mocobservation.MOCPlant = moca.PlantName;
            
            mocobservationvm.Roles = CurrentUser.Roles;
            mocobservationvm.UserFullName = CurrentUser.FullName;
            mocobservationvm.ProfileImage = CurrentUser.ProfileImage;

            mocobservationvm.IsRestrict = CurrentUser.IsRestrict;
            mocobservationvm.ObservationViewModelListMOC1 = observationmoclist;
            mocobservationvm.MOCObservation = mocobservation;
            foreach (var a in observationmoclist)
            {
                if (CurrentUser.UserID == a.ActionBy)
                {
                    mocobservationvm.MOCObservation.CompletedBy = a.ActionBy;
                }
            }
            return View(mocobservationvm);
        }

        public ActionResult SaveMOCObservations(MOCObservation cpObservation)
        {
            cpObservation.CurrentUser = CurrentUser.UserID;

            MocBLL.SaveMOCObservation(cpObservation);

            return View();
        }



        [HttpPost]
        public ActionResult EditMOCObservation(int observationID)
        {
            MCObservationViewModel mcobservation = new MCObservationViewModel();

            mcobservation = MocBLL.EditMOCObservation(observationID);

            if (mcobservation.mocobservation.CompletedBy == CurrentUser.UserID)
            {
                mcobservation.mocobservation.CompletedDate = DateTime.Now.ToString("dd/MM/yyyy");
            }

            return Json(new { mcobservation });
            //return View(insObservation);
        }
        [HttpGet]
        public ActionResult TrackActions()
        {
            AllMOCObservation trackaction = new AllMOCObservation();

            ViewBag.IncidentPlant = new SelectList(IncidentPlants, "ID", "Name");

            List<ObservationViewModelMOC> obsvm = new List<ObservationViewModelMOC>();
            obsvm = MocBLL.GetAllMOCObservation();
            var action = MocBLL.GetActionList();
            var Action = action.Where(y => y.UserID != 0).ToList();
            var trackStatus = TrackActionStatus.Where(y => y.ID != 4 && y.ID != 5).ToList();
            List<MOCRecomCategory> recomcategory = new List<MOCRecomCategory>();
            recomcategory = MocBLL.GetMOCRecomCategory();
            var recomcat = recomcategory.Where(y => y.ID != 0).ToList();

            var recompriority1 = MocBLL.GetMOCRecomPriority();
            var repriority = recompriority1.Where(y => y.ID != 0).ToList();
            trackaction.ActionList = Action;
            trackaction.MOCTrackActionStatus = trackStatus;
            trackaction.Recompriority = repriority;
            trackaction.Recomcategory = recomcat;
            trackaction.ObservationViewModelList1 = obsvm;

            MOCSearchViewModel mocs = new MOCSearchViewModel();
             ViewBag.fromdate = mocs.MOCFromDate;
            ViewBag.todate = mocs.MOCToDate;
            ViewBag.plantid = mocs.Plant;
            ViewBag.recomp = mocs.RecomPriorityID;
            ViewBag.recomc = mocs.RecomCategoryID;
            ViewBag.actionerid = mocs.ActionerID;
            ViewBag.recomstatus = mocs.RecomStatus;

            trackaction.Roles = CurrentUser.Roles;
            trackaction.UserFullName = CurrentUser.FullName;
            trackaction.ProfileImage = CurrentUser.ProfileImage;

            trackaction.IsRestrict = CurrentUser.IsRestrict;
            return View(trackaction);
        }

        [HttpPost]
        public ActionResult TrackActions(MOCSearchViewModel mocsearch)
        {
            AllMOCObservation trackaction = new AllMOCObservation();
           
            var action = MocBLL.GetActionList();
            var Action = action.Where(y => y.UserID != 0).ToList();
            var trackStatus = TrackActionStatus.Where(y => y.ID != 4 && y.ID != 5).ToList();
            List<MOCRecomCategory> recomcategory = new List<MOCRecomCategory>();
            recomcategory = MocBLL.GetMOCRecomCategory();
            var recomcat = recomcategory.Where(y => y.ID != 0).ToList();
            var recompriority1 = MocBLL.GetMOCRecomPriority();
            var repriority = recompriority1.Where(y => y.ID != 0).ToList();
            

            trackaction.ObservationViewModelList1 = MocBLL.SearchOpenMOCForObservation(mocsearch);
            ViewBag.IncidentPlant = new SelectList(IncidentPlants, "ID", "Name");
            trackaction.ActionList = Action;
            trackaction.MOCTrackActionStatus = trackStatus;
            trackaction.Recompriority = repriority;
            trackaction.Recomcategory = recomcat;
            
            trackaction.MOCSearchVM.MOCFromDate = mocsearch.MOCFromDate;
            trackaction.MOCSearchVM.MOCToDate = mocsearch.MOCToDate;
            trackaction.MOCSearchVM.Plant = mocsearch.Plant;
            trackaction.MOCSearchVM.RecomPriorityID = mocsearch.RecomPriorityID;
            trackaction.MOCSearchVM.RecomCategoryID = mocsearch.RecomCategoryID;
            trackaction.MOCSearchVM.RecomStatus = mocsearch.RecomStatus;
            trackaction.MOCSearchVM.ActionerID = mocsearch.ActionerID;
        
            ViewBag.fromdate = mocsearch.MOCFromDate;
            ViewBag.todate = mocsearch.MOCToDate;
            ViewBag.plantid = mocsearch.Plant;
            ViewBag.recomp = mocsearch.RecomPriorityID;
            ViewBag.recomc = mocsearch.RecomCategoryID;
            ViewBag.actionerid = mocsearch.ActionerID;
            ViewBag.recomstatus = mocsearch.RecomStatus;

            trackaction.Roles = CurrentUser.Roles;
            trackaction.UserFullName = CurrentUser.FullName;
            trackaction.ProfileImage = CurrentUser.ProfileImage;

            trackaction.IsRestrict = CurrentUser.IsRestrict;
            return View(trackaction);

        }

        public ActionResult ExportTrackActions(string currentFromDate, string currentEndDate, int ActionerID, int PlantID, int recomstatus, int recompriority, int recomcategory)
        {


            using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "ExportMOCTrackActions";
                objCom.CommandType = CommandType.StoredProcedure;
                objCom.Parameters.AddWithValue("@Plant", PlantID);
                objCom.Parameters.AddWithValue("@RecommPriority", recompriority);
                objCom.Parameters.AddWithValue("@RecommCategory", recomcategory);
                objCom.Parameters.AddWithValue("@FromDate", currentFromDate == null ? string.Empty : currentFromDate);
                objCom.Parameters.AddWithValue("@ToDate", currentEndDate == null ? string.Empty : currentEndDate);
                objCom.Parameters.AddWithValue("@ActionBy", ActionerID);
                objCom.Parameters.AddWithValue("@RecommStatus", recomstatus);

           
                objCon.Open();
                objCom.Connection = objCon;
                SqlDataAdapter Adapter = new SqlDataAdapter();
                Adapter.SelectCommand = objCom;
                DataSet dataSet = new DataSet();
                Adapter.Fill(dataSet);
                objCon.Close();
                var wb = new XLWorkbook(Server.MapPath("~/Template/TrackActionsMOC.xlsx"));
                var worksheet = wb.Worksheet("ObservationsList");
                worksheet.Cell("C4").Value = "Report Generated by : " + CurrentUser.FullName;
                //worksheet.Cell("D4").Value = CurrentUser.FullName;
                worksheet.Cell("C5").Value = "Report Duration : " + currentFromDate + " to  " + currentEndDate;
                //worksheet.Cell("D5").Value = currentFromDate + " to  " + currentEndDate;

                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    var rangeWithDataSecond = worksheet.Cell(7, 1).InsertTable(dataSet.Tables[0].AsEnumerable());
                }
                else
                {
                    worksheet.Cell("C8").Value = "No Data Found";
                }

                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=MOCTrackActions.xlsx");
                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }

            }
            return View();
        }

        [HttpGet]
        public ActionResult MOCHistory()
        {
            MOCViewModel.Roles = UserRoles;
            MOCViewModel.CurrentUser = CurrentUser.UserID;
            ViewBag.PlantsList = new SelectList(IncidentPlants, "ID", "Name");
            MOCViewModel.MOCList = MocBLL.GetAllClosedMOC();
            MOCViewModel.MocCategory = MocCategory;
            MOCViewModel.MOCClass = MocClass;
            MOCViewModel.MocType = Moctype;
            MOCViewModel.MocstatusList = mocstatus;
            MOCViewModel.Roles = CurrentUser.Roles;
            MOCViewModel.UserFullName = CurrentUser.FullName;
            MOCViewModel.ProfileImage = CurrentUser.ProfileImage;
            MOCViewModel.IsRestrict = CurrentUser.IsRestrict;

            MOCSearchViewModel mocsearch = new MOCSearchViewModel();
            ViewBag.fromdate = mocsearch.MOCFromDate;
            ViewBag.Todate = mocsearch.MOCToDate;
            ViewBag.PlantID = mocsearch.Plant;
            ViewBag.moccategory = mocsearch.MOCCategory;
            ViewBag.moctype = mocsearch.MOCType;
            ViewBag.mocclass = mocsearch.ClassID;
  

            return View(MOCViewModel);
        }
        
        [HttpPost]
        public ActionResult MOCHistory(MOCSearchViewModel mocsearch)
        {
            mocsearch.MOCStatus = 3;
            MOCViewModel.Roles = UserRoles;
            MOCViewModel.CurrentUser = CurrentUser.UserID;
            MOCViewModel.MOCList = MocBLL.SearchClosedMOC(mocsearch);
            ViewBag.PlantsList = new SelectList(IncidentPlants, "ID", "Name");
            MOCViewModel.MOCClass = MocClass;
            MOCViewModel.MocCategory = MocCategory;
            MOCViewModel.MocType = Moctype;
            MOCViewModel.MocstatusList = mocstatus;

            MOCViewModel.MOCSearchVM.Plant = mocsearch.Plant;
            MOCViewModel.MOCSearchVM.MOCFromDate = mocsearch.MOCFromDate;
            MOCViewModel.MOCSearchVM.MOCToDate = mocsearch.MOCToDate;
            MOCViewModel.MOCSearchVM.ClassID = mocsearch.ClassID;
            MOCViewModel.MOCSearchVM.MOCType = mocsearch.MOCType;
            MOCViewModel.MOCSearchVM.MOCCategory = mocsearch.MOCCategory;

            ViewBag.fromdate = mocsearch.MOCFromDate;
            ViewBag.Todate = mocsearch.MOCToDate;
            ViewBag.PlantID = mocsearch.Plant;
            ViewBag.moccategory = mocsearch.MOCCategory;
            ViewBag.moctype = mocsearch.MOCType;
            ViewBag.mocclass = mocsearch.ClassID;
       

            MOCViewModel.Roles = CurrentUser.Roles;
            MOCViewModel.UserFullName = CurrentUser.FullName;
            MOCViewModel.ProfileImage = CurrentUser.ProfileImage;

            MOCViewModel.IsRestrict = CurrentUser.IsRestrict;
            return View(MOCViewModel);
        }
        public ActionResult ExportMOCHistory(string currentFromDate, string currentEndDate, int PlantID, int moccategory, int moctype, int mocclass)
        {


            using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "ExportMOCHistory";
                objCom.CommandType = CommandType.StoredProcedure;
                
                objCom.Parameters.AddWithValue("@MOCPlant", PlantID);
                objCom.Parameters.AddWithValue("@FromDate", currentEndDate == null ? string.Empty : currentFromDate);
                objCom.Parameters.AddWithValue("@ToDate", currentEndDate == null ? string.Empty : currentEndDate);
                objCom.Parameters.AddWithValue("@MOCCategory", moccategory);
                objCom.Parameters.AddWithValue("@Class", mocclass);
                objCom.Parameters.AddWithValue("@MOCType", moctype);
                objCon.Open();
                objCom.Connection = objCon;
                SqlDataAdapter Adapter = new SqlDataAdapter();
                Adapter.SelectCommand = objCom;
                DataSet dataSet = new DataSet();
                Adapter.Fill(dataSet);
                objCon.Close();
                var wb = new XLWorkbook(Server.MapPath("~/Template/MOCHistory.xlsx"));
                var worksheet = wb.Worksheet("ClosedList");
                worksheet.Cell("C4").Value = "Report Generated by : " + CurrentUser.FullName;
                //worksheet.Cell("D4").Value = CurrentUser.FullName;
                worksheet.Cell("C5").Value = "Report Duration : " + currentFromDate + " to  " + currentEndDate;
                //worksheet.Cell("D5").Value = currentFromDate + " to  " + currentEndDate;

                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    var rangeWithDataSecond = worksheet.Cell(7, 1).InsertTable(dataSet.Tables[0].AsEnumerable());
                }
                else
                {
                    worksheet.Cell("C8").Value = "No Data Found";
                }

                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename= ClosedMOCHistoryRecord.xlsx");
                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }

            }
            return View();
        }
     
        [HttpPost]
        public ActionResult UpdateStatus(int MOCID)
        {
            NewMOCModel newVM = new NewMOCModel();
            MOCa moca = new MOCa();

            moca = MocBLL.GetMOC(MOCID);
            newVM.statuslist = mocstatus;
            newVM.moca = moca;
            return View(newVM);
        }

    
        [HttpPost]
        public ActionResult UpdateMOCStatus(int MOCID, int StatusID, string CloseComments)
        {
            string strMessage = String.Empty;
            try
            {
                MocBLL.UpdateMOCStatus(MOCID, StatusID, CloseComments, CurrentUser.UserID);
            }
            catch (Exception ex)
            {
                strMessage = ex.Message;
            }
            return Json(new { strMessage });
        }

        [HttpGet]
        public ActionResult MOCObservers(int MOCID)
        {
            MOCa moca = new MOCa();
            moca = MocBLL.GetMOC(MOCID);
            MOCApproverList approverlist = new MOCApproverList();

            List<Employee> EmployeeList = MocBLL.GetAllEmployees();
            moca.MOCID = MOCID;
            var appget = MocBLL.GetApprovalStages();

            var appsave = MocBLL.GetApprovalStagesSave(MOCID);
            if (appsave.ApprovalList.Count > 0)
            {
                approverlist.ApprovalList = appsave.ApprovalList;
            }
            else
            {
                approverlist.ApprovalList = appget.ApprovalList;
            }
            approverlist.MOCID = MOCID;
            approverlist.MOCNo = moca.MOCNumber;
            approverlist.MOCTitle = moca.MOCTitle;
            approverlist.MOCDescription = moca.MOCDescription;
            approverlist.PlantName = moca.PlantName;
            approverlist.EmployeeList = EmployeeList;
            approverlist.UserFullName = CurrentUser.FullName;
            approverlist.ProfileImage = CurrentUser.ProfileImage;

            approverlist.IsRestrict = CurrentUser.IsRestrict;
            return View(approverlist);
        }
        
        [HttpPost]
       
        public ActionResult MOCObservers(MOCApproverList approverList, List<ApprovalList> ApprovalList)
        {
            MOCa moca = new MOCa();
            approverList.UserID = CurrentUser.UserID;
            List<Employee> EmployeeList = MocBLL.GetAllEmployees();
            int temp = 0;
            foreach (var item in ApprovalList)
            {
                if (((item.IsTeamApprover == 2) && (item.ApprovalTargetDate != null))||(item.IsTeamApprover == 1 && item.ApprovalTargetDate == null))
                {
                  
                }
                else
                {
                    temp = temp + 1;
                   
                    break;
                }

            }
            if (temp > 0)
            {
                ViewBag.Error = "Kindly, fill the Target Date in Approver stages. Alternatively, choose info Option.";
            }
            else
            {
                MocBLL.SaveApprovals(approverList, ApprovalList);
                ViewBag.Message1 = string.Format("MOC Coordinator and Design review person have been notified for review");

            }

            if (approverList.Approver > 0)
            {
                MocBLL.ApproverAdd(approverList.MOCID, approverList.Approver, approverList.UserID, approverList.TargetDate);

            }
            approverList.ApprovalList = ApprovalList;
            approverList.EmployeeList = EmployeeList;
            approverList.UserFullName = CurrentUser.FullName;
            approverList.ProfileImage = CurrentUser.ProfileImage;

            approverList.IsRestrict = CurrentUser.IsRestrict;
            return View(approverList);
        }

        //[HttpPost]
        //      public ActionResult ApproverAdd(int MOCID, int ApproverStagesID, int EmployeeID, string TargetDate)
        //      {
        //          string strMessage = String.Empty;

        //          try
        //          {
        //            MocBLL.ApproverAdd(MOCID, ApproverStagesID, EmployeeID, TargetDate);

        //          }
        //          catch (Exception ex)
        //          {
        //              strMessage = ex.Message;
        //          }
        //          return RedirectToAction("MOCObservers",new {MOCID=MOCID });
        //      }


        public int FuncationalManagerApprovers(FuncationalManagerApprove funap, int MOCID)
        {

            try
            {
                funap.UserID = CurrentUser.UserID;
                return MocBLL.FuncationalManagerApprovers(funap, MOCID);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult UploadAttachments(int MOCID)
        {
            AttachmentViewModel attachmentviewmodel = new AttachmentViewModel();

            MOCa moca = new MOCa();
            moca = MocBLL.GetMOC(MOCID);
            attachmentviewmodel.MOCDescription = moca.MOCDescription;
            attachmentviewmodel.StatusID = moca.MOCStatusID;
            attachmentviewmodel.PlantArea = moca.PlantName;
            attachmentviewmodel.MocAttachments.MOCNo = moca.MOCNumber;
            attachmentviewmodel.MocAttachments.MOCId = MOCID;

            attachmentviewmodel.mocattach = MocBLL.GetMOCAttachments(MOCID);
            attachmentviewmodel.UserFullName = CurrentUser.UserName;
            attachmentviewmodel.UserID = CurrentUser.UserID;
            attachmentviewmodel.Roles = CurrentUser.Roles;

            attachmentviewmodel.IsRestrict = CurrentUser.IsRestrict;
            return PartialView(attachmentviewmodel);
        }
   
        [HttpPost]
  
        public ActionResult UploadAttachments(MOCAttachment mocattachments)
        {

            AttachmentViewModel attachmentviewmodel = new AttachmentViewModel();
            MOCa moca = new MOCa();
            moca = MocBLL.GetMOC(mocattachments.MOCId);
            attachmentviewmodel.MOCDescription = moca.MOCDescription;
            attachmentviewmodel.PlantArea = moca.PlantName;
            attachmentviewmodel.MocAttachments.MOCNo = moca.MOCNumber;
           
            attachmentviewmodel.MOCID = mocattachments.MOCId;

            if (mocattachments.ImageFile != null )
            {
                var fileName = Path.GetFileName(mocattachments.ImageFile.FileName);
                var path = Path.Combine(Server.MapPath("~/MOCAttachments/"), fileName);
                mocattachments.ImageFile.SaveAs(path);


                bool statusFlag = MocBLL.UploadAttachments(mocattachments, CurrentUser.UserID);
                return RedirectToAction("UploadAttachments", new { MOCID = mocattachments.MOCId });
            }

            attachmentviewmodel.MOCDescription = moca.MOCDescription;
            attachmentviewmodel.StatusID = moca.MOCStatusID;
            attachmentviewmodel.Roles = CurrentUser.Roles;
            attachmentviewmodel.UserFullName = CurrentUser.FullName;
            attachmentviewmodel.ProfileImage = CurrentUser.ProfileImage;

            attachmentviewmodel.IsRestrict = CurrentUser.IsRestrict;
            moca = MocBLL.GetMOC(mocattachments.MOCId);

            attachmentviewmodel.MocAttachments.MOCId = mocattachments.MOCId;

            attachmentviewmodel.mocattach = MocBLL.GetMOCAttachments(mocattachments.MOCId);

            return PartialView(attachmentviewmodel);

        }

        [HttpPost]
    
        public ActionResult DeleteAttachments(int MOCAttachID)
        {

            MocBLL.DeleteAttachments(MOCAttachID);
            string strMessage = "Image Deleted Successfully";
            return Json(new { strMessage });
        }


        [HttpGet]
        public ActionResult TemporaryMOC()
        {
            TemporaryMOCModel temp = new TemporaryMOCModel();

            temp.TemporaryMOC = MocBLL.GetTemporaryList();
            foreach (var i in temp.TemporaryMOC)
            {
                temp.FactoryManager = i.FactoryManagerID;
                temp.CloseStatus = i.TempStatus;
            }
            temp.MocCoordinateUserList= MocBLL.GetActionList();
            temp.TempStatusList = MocBLL.GetMOCTempStatus();
            ViewBag.PlantsList = new SelectList(IncidentPlants, "ID", "Name");
            temp.CurrentUser = CurrentUser.UserID;
            temp.Roles = CurrentUser.Roles;
            temp.UserFullName = CurrentUser.FullName;
            temp.ProfileImage = CurrentUser.ProfileImage;

            temp.IsRestrict = CurrentUser.IsRestrict;
            return View(temp);
        }

        [HttpPost]

        public ActionResult TemporaryMOC(MOCSearchViewModel mocsearch)
        {
            TemporaryMOCModel temp = new TemporaryMOCModel();
            temp.TemporaryMOC = MocBLL.SearchTempMOC(mocsearch);
            foreach (var i in temp.TemporaryMOC)
            {
                temp.FactoryManager = i.FactoryManagerID;
                temp.CloseStatus = i.TempStatus;
            }
            temp.MocCoordinateUserList = MocBLL.GetActionList();
            temp.TempStatusList = MocBLL.GetMOCTempStatus();
            ViewBag.PlantsList = new SelectList(IncidentPlants, "ID", "Name");
            temp.CurrentUser = CurrentUser.UserID;
            temp.Roles = CurrentUser.Roles;
            temp.UserFullName = CurrentUser.FullName;
            temp.ProfileImage = CurrentUser.ProfileImage;

            temp.IsRestrict = CurrentUser.IsRestrict;
            return View(temp);
        }
        public int TemporaryMOCApprove(TemporaryMOCList temp)
        {
            temp.UserID = CurrentUser.UserID;
                
            try
            {
                return MocBLL.TemporaryMOCApprove(temp);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       

        [HttpPost]
   
        public ActionResult UpdateTemporaryStatus(int MOCID)
        {
            TemporaryMOCModel newVM = new TemporaryMOCModel();
            MOCa moca = new MOCa();
            moca = MocBLL.GetMOC(MOCID);

            newVM.TemporaryMOC = MocBLL.GetTemporaryList();
            newVM.CurrentUser = CurrentUser.UserID;
            newVM.moca = moca;
           
            return View(newVM);
        }
       
        [HttpPost]
      
        public ActionResult UpdateTemporaryMOCStatus(int MOCID, int StatusID, string CloseComments)
        {
            string strMessage = String.Empty;
            try
            {
                MocBLL.UpdateTemporaryMOCStatus(MOCID, StatusID, CloseComments, CurrentUser.UserID);
                strMessage = "Data Saved Successfully";
            }
            catch (Exception ex)
            {
                strMessage = ex.Message;
            }
            return Json(new { strMessage });
        }
    
        [HttpGet]
        public ActionResult MOCClosureList(int MOCID = 0)
        {
            MOCClosureList objMOCClosurelist = new MOCClosureList();
            GetMOCClosureList obj = new GetMOCClosureList();
            MOCa moca = new MOCa();
            moca = MocBLL.GetMOC(MOCID);
            objMOCClosurelist.MOCID = moca.MOCID;


            objMOCClosurelist.GetMOCClosureList = MocBLL.GetMOCClosureList(MOCID);
            foreach (var i in objMOCClosurelist.GetMOCClosureList)
            {
                objMOCClosurelist.moci = i.MOCID;
            }
            objMOCClosurelist.Roles = CurrentUser.Roles;
            objMOCClosurelist.UserFullName = CurrentUser.FullName;
            objMOCClosurelist.ProfileImage = CurrentUser.ProfileImage;

            objMOCClosurelist.IsRestrict = CurrentUser.IsRestrict;
            return View(objMOCClosurelist);
        }
       
        [HttpPost]
  
        public ActionResult MOCClosureList(MOCClosureList mocclosure, List<GetMOCClosureList> GetMOCClosureList)
        {
            string strMessage = String.Empty;
            try
            {
               
                MocBLL.SaveMOCClosureList(mocclosure, GetMOCClosureList);
                
             
                    ViewBag.SaveMessage = "MOC Saved Successfully";
                

                   
                
            }
            catch (Exception ex)
            {
                strMessage = ex.Message;
            }

            mocclosure.Roles = CurrentUser.Roles;
            mocclosure.UserFullName = CurrentUser.FullName;
            mocclosure.ProfileImage = CurrentUser.ProfileImage;

            mocclosure.IsRestrict = CurrentUser.IsRestrict;
            return View(mocclosure);

        }

        public ActionResult MOCDashboard()
        {

            MOCDashboard Mocdash = new MOCDashboard();
            Mocdash.DesigID = CurrentUser.Designation;
            Mocdash.Roles = CurrentUser.Roles;
            Mocdash.UserFullName = CurrentUser.FullName;
            Mocdash.UserID = CurrentUser.UserID;
            Mocdash.ProfileImage = CurrentUser.ProfileImage;

            Mocdash.IsRestrict = CurrentUser.IsRestrict;
            return View(Mocdash);
        }
        public JsonResult MOCClassMonthlyCount(DateTime startDate, DateTime endDate)
        {
            MOCDashboard Mocdash = new MOCDashboard()
            {
                Roles = UserRoles,
                ClassCount = MocBLL.GetMOCClassCount(startDate, endDate)
            };
            return Json(Mocdash.ClassCount.ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult MOCPlantCount(DateTime startDate, DateTime endDate)
        {
            MOCDashboard Mocdash = new MOCDashboard()
            {
                Roles = UserRoles,
                PlantCount = MocBLL.GetMOCPlantCount(startDate, endDate)
            };
            return Json(Mocdash.PlantCount.ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult MOCCategoryCount(DateTime startDate, DateTime endDate)
        {
            MOCDashboard Mocdash = new MOCDashboard()
            {
                Roles = UserRoles,
                CategoryCount = MocBLL.GetMOCCategoryCount(startDate, endDate)
            };
            return Json(Mocdash.CategoryCount.ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult MOCStatusCount(DateTime startDate, DateTime endDate)
        {
            MOCDashboard Mocdash = new MOCDashboard()
            {
                Roles = UserRoles,
                StatusCount = MocBLL.GetMOCStatusCount(startDate, endDate)
            };
            return Json(Mocdash.StatusCount.ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult MOCPriorityCount(DateTime startDate, DateTime endDate)
        {
            MOCDashboard Mocdash = new MOCDashboard()
            {
                Roles = UserRoles,
                PriorityCount = MocBLL.GetMOCPriorityCount(startDate, endDate)
            };
            return Json(Mocdash.PriorityCount.ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult MOCRecomStatusCount(DateTime startDate, DateTime endDate)
        {
            MOCDashboard Mocdash = new MOCDashboard()
            {
                Roles = UserRoles,
                RecomStatusCount = MocBLL.GetMOCRecomStatusCount(startDate, endDate)
            };
            return Json(Mocdash.RecomStatusCount.ToList(), JsonRequestBehavior.AllowGet);
        }

       
        public ActionResult MOCPrintPdf(int id)
        {
            try
            {
                using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[MOCPDF]";
                    objCom.Parameters.AddWithValue("@MOCID", id);
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCon.Open();
                    objCom.Connection = objCon;
                    SqlDataAdapter Adapter = new SqlDataAdapter();
                    Adapter.SelectCommand = objCom;
                    DataSet dataSet = new DataSet();
                    Adapter.Fill(dataSet);
                    objCon.Close();

                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition","filename=MOCPDF.pdf");
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);

                    iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 20f, 20f, 30f, 10f);
                 
                    PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                    pdfDoc.Open();
              


                    PdfPTable TitleTable = new PdfPTable(3);
                    TitleTable.LockedWidth = true;
                    TitleTable.SetWidths(new float[] { 8f, 14f, 8f });
                    //TitleTable.SpacingBefore = 10f;
                    //TitleTable.SpacingAfter = 1f;
                    TitleTable.TotalWidth = 555f;
                    PdfPCell Wpcell = new PdfPCell();
                    string imageURL = Server.MapPath("~/Images/gcpl_logo2.png");
                    iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(imageURL);
                    gif.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
                    gif.SpacingBefore = 10f;

                    gif.ScaleAbsolute(140f, 45f);
                    Wpcell = new PdfPCell(gif);

                    TitleTable.AddCell(Wpcell);

                    var phrase = new Phrase(new Chunk("\n Management Of Change\n \t \t \t \t \t \t (MOC)" + "\n\n\n", FontFactory.GetFont("Times New Roman", 14, iTextSharp.text.Font.BOLD)));

                    TitleTable.AddCell(PhraseCell(phrase, PdfPCell.ALIGN_CENTER));

                    Wpcell = new PdfPCell(new Phrase("\n \t \t \t \t \t \t \t STATUS  \n \t \t \t \t \t\t \t  "+ dataSet.Tables[0].Rows[0][1].ToString(), FontFactory.GetFont("Times New Roman", 13, iTextSharp.text.Font.BOLD)));
                    Wpcell.HorizontalAlignment = Element.ALIGN_LEFT;

                    TitleTable.AddCell(Wpcell);

                    String FONT = "C:/Windows/Fonts/wingding.ttf";
                    String CheckedCheckboxText = "\u00fe";
                    String BlankCheckboxText = "o";
                    BaseFont bf = BaseFont.CreateFont(FONT, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    iTextSharp.text.Font f = new iTextSharp.text.Font(bf, 10);
                    Paragraph CheckedCheckbox = new Paragraph(CheckedCheckboxText, f);
                    Paragraph uncheckbox = new Paragraph(BlankCheckboxText, f);

                    String Blank = "";
                    Paragraph uncheckblank = new Paragraph(Blank, f);
                    pdfDoc.Add(TitleTable);
                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));
                    var font8 = FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD);
                    var font9 = FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL);
                    var font7 = FontFactory.GetFont("Times New Roman", 11, iTextSharp.text.Font.BOLD);

                    PdfPTable Details = new PdfPTable(5);
                    Details.TotalWidth = 555f;
                    Details.LockedWidth = true;
                    Details.SetWidths(new float[] { 2f, 6f, 8f, 7f, 7f });
                    Wpcell = new PdfPCell(new Phrase(new Chunk("1", font8)));
                    Wpcell.BackgroundColor = new BaseColor(System.Drawing.Color.LightGray);

                    Details.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Change Initiation", font8)));
                    Wpcell.BackgroundColor = new BaseColor(System.Drawing.Color.LightGray);

                    Wpcell.Colspan = 4;
                    Details.AddCell(Wpcell);
                    pdfDoc.Add(Details);

                    PdfPTable tablesender = new PdfPTable(5);
                    tablesender.TotalWidth = 555f;
                    tablesender.LockedWidth = true;
                    tablesender.SetWidths(new float[] { 2f, 6f, 8f, 7f, 7f });


                    tablesender.AddCell(PhraseCell(new Phrase("1.1", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("MOC Initiation Date", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][16].ToString(), FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("MOC ID", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][19].ToString(), FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                   
                    tablesender.AddCell(PhraseCell(new Phrase("1.2", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("Plant/Area", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][2].ToString(), FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("Estimated Cost(INR)", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][22].ToString(), FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));


                    tablesender.AddCell(PhraseCell(new Phrase("1.3", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("Change Type", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][3].ToString(), FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("Emergency", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][26].ToString(), FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    tablesender.AddCell(PhraseCell(new Phrase("1.4", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("Originator", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][15].ToString(), FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("Discipline Lead", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][17].ToString(), FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                   
                    pdfDoc.Add(tablesender);

                    PdfPTable title = new PdfPTable(3);
                    title.TotalWidth = 555f;
                    title.LockedWidth = true;
                    title.SetWidths(new float[] { 2f, 6f, 22f });

                    title.AddCell(PhraseCell(new Phrase("1.5", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    title.AddCell(PhraseCell(new Phrase("Change Title", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    title.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][6].ToString(), FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(title);
                    PdfPTable ChangeEffect = new PdfPTable(5);
                    ChangeEffect.TotalWidth = 555f;
                    ChangeEffect.LockedWidth = true;
                    ChangeEffect.SetWidths(new float[] { 2f, 6f, 8f, 7f, 7f });
                   // ChangeEffect.AddCell(PhraseCell(new Phrase("1.6", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    //ChangeEffect.AddCell(PhraseCell(new Phrase("Change Effective Date", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                   // ChangeEffect.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][25].ToString(), FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                   // ChangeEffect.AddCell(PhraseCell(new Phrase("Expiry Date", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                   // ChangeEffect.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][24].ToString(), FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    ChangeEffect.AddCell(PhraseCell(new Phrase("1.6", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    ChangeEffect.AddCell(PhraseCell(new Phrase("Change Category", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    ChangeEffect.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][4].ToString(), FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    ChangeEffect.AddCell(PhraseCell(new Phrase("Secondary Changes", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    ChangeEffect.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][5].ToString(), FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(ChangeEffect);
                    PdfPTable des = new PdfPTable(3);
                    des.TotalWidth = 555f;
                    des.LockedWidth = true;
                    des.SetWidths(new float[] { 2f, 6f, 22f });

                    des.AddCell(PhraseCell(new Phrase("1.7", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    des.AddCell(PhraseCell(new Phrase("Change Description", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    des.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][7].ToString(), FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    pdfDoc.Add(des);

                    PdfPTable reasonchange = new PdfPTable(5);
                    reasonchange.TotalWidth = 555f;
                    reasonchange.LockedWidth = true;
                    reasonchange.SetWidths(new float[] { 2f, 6f, 8f, 7f, 7f });

                    //reasonchange.AddCell(PhraseCell(new Phrase("1.8", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    //reasonchange.AddCell(PhraseCell(new Phrase("Reason For Change", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    //reasonchange.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][28].ToString(), FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    //reasonchange.AddCell(PhraseCell(new Phrase("Estimated Cost(INR)", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    //reasonchange.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][22].ToString(), FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(reasonchange);

                    PdfPTable changeJust = new PdfPTable(3);
                    changeJust.TotalWidth = 555f;
                    changeJust.LockedWidth = true;
                    changeJust.SetWidths(new float[] { 2f, 6f, 22f });

                    changeJust.AddCell(PhraseCell(new Phrase("1.8", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    changeJust.AddCell(PhraseCell(new Phrase("Change Justification", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    changeJust.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][13].ToString(), FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    pdfDoc.Add(changeJust);

                    PdfPTable process = new PdfPTable(5);
                    process.TotalWidth = 555f;
                    process.LockedWidth = true;
                    process.SetWidths(new float[] { 2f, 6f, 8f, 7f, 7f });

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1.9", font8)));
                    Wpcell.Rowspan = 2;
                    process.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Risk assessment (The Risk of Doing change)", font8)));
                    Wpcell.Rowspan = 2;
                    process.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Impact", font8)));
                    process.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Likelihood", font8)));
                    process.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("RA Rating", font8)));
                    process.AddCell(Wpcell);

                  
                    var RARating = dataSet.Tables[0].Rows[0][36].ToString();
                    var RARatingNew = RARating.Trim();
                    Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[0].Rows[0][34].ToString(), font8)));
                    process.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[0].Rows[0][35].ToString(), font8)));
                    process.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(RARatingNew, font8)));
                    process.AddCell(Wpcell);
                    pdfDoc.Add(process);
                 
                    PdfPTable att = new PdfPTable(2);
                    att.TotalWidth = 555f;
                    att.LockedWidth = true;
                    att.SetWidths(new float[] { 0.75f, 10f});
                    Wpcell = new PdfPCell(new Phrase(new Chunk("1.10", font8)));
                    att.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Attachments", font8)));
      
                    att.AddCell(Wpcell);
                    if (dataSet.Tables[15].Rows.Count > 0)
                    {
                        for (int rows = 0; rows < dataSet.Tables[15].Rows.Count; rows++)
                        {
                            for (int column = 0; column < dataSet.Tables[15].Columns.Count; column++)
                            {
                                Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[15].Rows[rows][column].ToString(), font9)));
                                att.AddCell(Wpcell);
                            }
                        }
                    }
                    else
                    {
                        for (int column = 0; column < dataSet.Tables[15].Columns.Count; column++)
                        {

                            Wpcell = new PdfPCell(new Phrase(new Chunk("Data Not Available", font9)));
                            att.AddCell(Wpcell);
                        }
                    }
                   
                    pdfDoc.Add(att);

                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));
                    PdfPTable LineManagerApproval = new PdfPTable(5);
                    LineManagerApproval.TotalWidth = 555f;
                    LineManagerApproval.LockedWidth = true;
                    LineManagerApproval.SetWidths(new float[] { 2f, 6f, 8f, 7f, 7f });
                    Wpcell = new PdfPCell(new Phrase(new Chunk("2", font8)));
                    Wpcell.BackgroundColor = new BaseColor(System.Drawing.Color.LightGray);
                    LineManagerApproval.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Line Manager Approval", font8)));
                    Wpcell.BackgroundColor = new BaseColor(System.Drawing.Color.LightGray);
                    Wpcell.Colspan = 4;
                    LineManagerApproval.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("2.1", font8)));
                    LineManagerApproval.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Verified idea, practicality and risk assessment", font8)));
                    LineManagerApproval.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[0].Rows[0][29].ToString(), font9)));
                    LineManagerApproval.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Cross business review / Cross business team completed", font8)));
                    LineManagerApproval.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[0].Rows[0][30].ToString(), font9)));
                    LineManagerApproval.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2.2", font8)));
                    LineManagerApproval.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Line Manager Comments", font8)));
                    LineManagerApproval.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[0].Rows[0][23].ToString(), font9)));
                    Wpcell.Colspan = 3;
                    LineManagerApproval.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2.3", font8)));
                    LineManagerApproval.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Line Manager Name", font8)));
                    LineManagerApproval.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[0].Rows[0][17].ToString(), font9)));
                    LineManagerApproval.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Approved Date", font8)));
                    LineManagerApproval.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[0].Rows[0][32].ToString(), font9)));
                    LineManagerApproval.AddCell(Wpcell);

                    //Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    //LineManagerApproval.AddCell(Wpcell);
                    //Wpcell = new PdfPCell(new Phrase(new Chunk("MOC Request Owner", font8)));
                    //LineManagerApproval.AddCell(Wpcell);
                    //Wpcell = new PdfPCell(new Phrase(new Chunk("" , font9)));
                    //LineManagerApproval.AddCell(Wpcell);
                    //Wpcell = new PdfPCell(new Phrase(new Chunk("Look Back Required", font8)));
                    //LineManagerApproval.AddCell(Wpcell);
                    //Wpcell = new PdfPCell(new Phrase(new Chunk("" , font9)));
                    //LineManagerApproval.AddCell(Wpcell);


                    pdfDoc.Add(LineManagerApproval);
                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));
                    PdfPTable approvers = new PdfPTable(6);
                    approvers.TotalWidth = 555f;
                    approvers.LockedWidth = true;
                    approvers.SetWidths(new float[] { 2f, 8f, 8f, 7f, 6f,6f });

                    Wpcell = new PdfPCell(new Phrase(new Chunk("3", font8)));
                    Wpcell.BackgroundColor = new BaseColor(System.Drawing.Color.LightGray);
                    approvers.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("MOC Review (Technical Authority)", font8)));
                    Wpcell.BackgroundColor = new BaseColor(System.Drawing.Color.LightGray);
                    Wpcell.Colspan = 5;
                    approvers.AddCell(Wpcell);

                    //Wpcell = new PdfPCell(new Phrase(new Chunk("3.1", font8)));
                    //approvers.AddCell(Wpcell);
                    //Wpcell = new PdfPCell(new Phrase(new Chunk("MOC Prirority", font8)));
                    //approvers.AddCell(Wpcell);
                    //Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[0].Rows[0][18].ToString(), font9)));
                 
                    //approvers.AddCell(Wpcell);
                    
                    //Wpcell = new PdfPCell(new Phrase(new Chunk("MOC Coordinator ", font8)));
                    //approvers.AddCell(Wpcell);
                    //if (dataSet.Tables[14].Rows.Count > 0)
                    //{
                    //    Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[14].Rows[0][0].ToString(), font9)));
                    //    Wpcell.Colspan = 2;
                    //}
                    //else
                    //{
                    //    Wpcell = new PdfPCell(new Phrase(new Chunk("" , font9)));
                    //    Wpcell.Colspan = 2;
                    //}
                    //approvers.AddCell(Wpcell);

                    
                    //approvers.AddCell(Wpcell);
                   


                    Wpcell = new PdfPCell(new Phrase(new Chunk("S.NO", font8)));
                    approvers.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Role", font8)));
                    approvers.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name", font8)));
                    approvers.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Remarks", font8)));
                    approvers.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Target Date", font8)));
                    approvers.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Approval Date", font8)));
                    approvers.AddCell(Wpcell);
                    if (dataSet.Tables[1].Rows.Count > 0)
                    {
                        Wpcell = new PdfPCell(new Phrase(new Chunk("3.1", font9)));
                        approvers.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("Design Review", font9)));
                        approvers.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[1].Rows[0][0].ToString(), font9)));
                        approvers.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[1].Rows[0][1].ToString(), font9)));
                        approvers.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[1].Rows[0][2].ToString(), font9)));
                        approvers.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[1].Rows[0][3].ToString(), font9)));
                        approvers.AddCell(Wpcell);

                        Wpcell = new PdfPCell(new Phrase(new Chunk("3.2", font9)));
                        approvers.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("Risk Assessment", font9)));
                        approvers.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[3].Rows[0][0].ToString(), font9)));
                        approvers.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[3].Rows[0][1].ToString(), font9)));
                        approvers.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[3].Rows[0][2].ToString(), font9)));
                        approvers.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[3].Rows[0][3].ToString(), font9)));
                        approvers.AddCell(Wpcell);

                        Wpcell = new PdfPCell(new Phrase(new Chunk("3.3", font9)));
                        approvers.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("Technical Approval", font9)));
                        approvers.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[4].Rows[0][0].ToString(), font9)));
                        approvers.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[4].Rows[0][1].ToString(), font9)));
                        approvers.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[4].Rows[0][2].ToString(), font9)));
                        approvers.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[4].Rows[0][3].ToString(), font9)));
                        approvers.AddCell(Wpcell);
                    }
                    else
                    {

                        Wpcell = new PdfPCell(new Phrase(new Chunk("Data Not Available", font9)));
                        Wpcell.Colspan = 6;
                        approvers.AddCell(Wpcell);

                    }

                    pdfDoc.Add(approvers);
                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));
                    PdfPTable approvers1 = new PdfPTable(6);
                    approvers1.TotalWidth = 555f;
                    approvers1.LockedWidth = true;
                    approvers1.SetWidths(new float[] { 2f, 8f, 8f, 7f, 6f, 6f });

                    Wpcell = new PdfPCell(new Phrase(new Chunk("4", font8)));
                    Wpcell.BackgroundColor = new BaseColor(System.Drawing.Color.LightGray);
                    approvers1.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("MOC Approval (Decision Team/Line Manager)", font8)));
                    Wpcell.Colspan = 5;
                    Wpcell.BackgroundColor = new BaseColor(System.Drawing.Color.LightGray);
                    approvers1.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("S.NO", font8)));
                    approvers1.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Role", font8)));
                    approvers1.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name", font8)));
                    approvers1.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Remarks", font8)));
                    approvers1.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Target Date", font8)));
                    approvers1.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Approval Date", font8)));
                    approvers1.AddCell(Wpcell);
                    if (dataSet.Tables[1].Rows.Count > 0)
                    {
                        Wpcell = new PdfPCell(new Phrase(new Chunk("4.1", font9)));
                        approvers1.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("Operations Lead", font9)));
                        approvers1.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[5].Rows[0][0].ToString(), font9)));
                        approvers1.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[5].Rows[0][1].ToString(), font9)));
                        approvers1.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[5].Rows[0][2].ToString(), font9)));
                        approvers1.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[5].Rows[0][3].ToString(), font9)));
                        approvers1.AddCell(Wpcell);

                        Wpcell = new PdfPCell(new Phrase(new Chunk("4.2", font9)));
                        approvers1.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("Maintenance Lead", font9)));
                        approvers1.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[6].Rows[0][0].ToString(), font9)));
                        approvers1.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[6].Rows[0][1].ToString(), font9)));
                        approvers1.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[6].Rows[0][2].ToString(), font9)));
                        approvers1.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[6].Rows[0][3].ToString(), font9)));
                        approvers1.AddCell(Wpcell);

                        Wpcell = new PdfPCell(new Phrase(new Chunk("4.3", font9)));
                        approvers1.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("HSEF Lead", font9)));
                        approvers1.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[7].Rows[0][0].ToString(), font9)));
                        approvers1.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[7].Rows[0][1].ToString(), font9)));
                        approvers1.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[7].Rows[0][2].ToString(), font9)));
                        approvers1.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[7].Rows[0][3].ToString(), font9)));
                        approvers1.AddCell(Wpcell);

                        Wpcell = new PdfPCell(new Phrase(new Chunk("4.4", font9)));
                        approvers1.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("Factory Manager Approval", font9)));
                        approvers1.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[8].Rows[0][0].ToString(), font9)));
                        approvers1.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[8].Rows[0][1].ToString(), font9)));
                        approvers1.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[8].Rows[0][2].ToString(), font9)));
                        approvers1.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[8].Rows[0][3].ToString(), font9)));
                        approvers1.AddCell(Wpcell);

                        Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                        approvers1.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("Condition for MOC Approve", font8)));
                        approvers1.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[0].Rows[0][31].ToString(), font9)));
                        Wpcell.Colspan = 4;
                        approvers1.AddCell(Wpcell);


                    }

                    else
                    {
                        
                            Wpcell = new PdfPCell(new Phrase(new Chunk("Data Not Available", font9)));
                        Wpcell.Colspan = 6;
                            approvers1.AddCell(Wpcell);
                        
                    }

                    pdfDoc.Add(approvers1);
                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));



                    PdfPTable recommend = new PdfPTable(10);
                    recommend.TotalWidth = 750f;
                    recommend.LockedWidth = true;
                    recommend.SetWidths(new float[] { 2f, 3f, 9f, 5f, 6f, 9f, 5f, 6f,6f, 5f });
                    Wpcell = new PdfPCell(new Phrase(new Chunk("5", font8)));
                    Wpcell.BackgroundColor = new BaseColor(System.Drawing.Color.LightGray);
                    recommend.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Implementation Action List", font7)));
                    Wpcell.BackgroundColor = new BaseColor(System.Drawing.Color.LightGray);
                    Wpcell.Colspan = 9;
                    recommend.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    recommend.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Recom ID", font8)));
                    recommend.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Actions/ Deliverables", font8)));
                    recommend.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Category", font8)));
                    recommend.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Priority", font8)));
                    recommend.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Action Taken", font8)));
                    recommend.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Action by", font8)));
                    recommend.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Target Date", font8)));
                    recommend.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Completed Date", font8)));
                    recommend.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("Remarks", font8)));
                    recommend.AddCell(Wpcell);

                    if (dataSet.Tables[13].Rows.Count > 0)
                    {
                        for (int rows = 0; rows < dataSet.Tables[13].Rows.Count; rows++)
                        {
                            for (int column = 0; column < dataSet.Tables[13].Columns.Count; column++)
                            {
                                Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[13].Rows[rows][column].ToString(), font9)));
                                recommend.AddCell(Wpcell);
                            }
                        }
                    }
                    else
                    {
                        for (int column = 0; column < dataSet.Tables[13].Columns.Count; column++)
                        {

                            Wpcell = new PdfPCell(new Phrase(new Chunk("Data Not Available", font9)));
                            recommend.AddCell(Wpcell);
                        }
                    }
                    pdfDoc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
                    pdfDoc.NewPage();
                    pdfDoc.Add(recommend);
                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                    var tempanalysis = dataSet.Tables[0].Rows[0][3].ToString();
                    if (tempanalysis == "Temporary\r\n")
                    {
                        PdfPTable TempTable = new PdfPTable(8);
                        TempTable.TotalWidth = 750f;
                        TempTable.LockedWidth = true;
                        TempTable.SetWidths(new float[] { 4f, 4f, 9f, 5f, 5f, 6f, 9f, 5f });
                        Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                        TempTable.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("Temporary MOC (if Applicable)", font7)));
                        Wpcell.Colspan = 7;
                        TempTable.AddCell(Wpcell);

                        Wpcell = new PdfPCell(new Phrase(new Chunk("Start Date", font8)));
                        TempTable.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("6 Months Validity Date", font8)));
                        TempTable.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("Reason for Extension", font8)));
                        TempTable.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("Approver Name", font8)));
                        TempTable.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("Approver comments", font8)));
                        TempTable.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("Final validity", font8)));
                        TempTable.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("Normalization comments", font8)));
                        TempTable.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("Status", font8)));
                        TempTable.AddCell(Wpcell);


                        if (dataSet.Tables[16].Rows.Count > 0)
                        {
                            for (int rows = 0; rows < dataSet.Tables[16].Rows.Count; rows++)
                            {
                                for (int column = 0; column < dataSet.Tables[16].Columns.Count; column++)
                                {
                                    Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[16].Rows[rows][column].ToString(), font9)));
                                    TempTable.AddCell(Wpcell);
                                }
                            }
                        }
                        else
                        {
                            for (int column = 0; column < dataSet.Tables[16].Columns.Count; column++)
                            {

                                Wpcell = new PdfPCell(new Phrase(new Chunk("Data Not Available", font9)));
                                TempTable.AddCell(Wpcell);
                            }
                        }
                        pdfDoc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());

                        pdfDoc.Add(TempTable);


                        pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));
                    }

                    PdfPTable PSSRClose = new PdfPTable(6);
                    PSSRClose.TotalWidth = 750f;
                    PSSRClose.LockedWidth = true;
                    PSSRClose.SetWidths(new float[] { 2f, 8f, 8f, 7f, 6f, 6f });

                    Wpcell = new PdfPCell(new Phrase(new Chunk("6", font8)));
                    Wpcell.BackgroundColor = new BaseColor(System.Drawing.Color.LightGray);
                    PSSRClose.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("PSSR", font8)));
                    Wpcell.BackgroundColor = new BaseColor(System.Drawing.Color.LightGray);
                    Wpcell.Colspan = 5;
                    PSSRClose.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("S.NO", font8)));
                    PSSRClose.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Role", font8)));
                    PSSRClose.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name", font8)));
                    PSSRClose.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Remarks", font8)));
                    PSSRClose.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Target Date", font8)));
                    PSSRClose.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Approval Date", font8)));
                    PSSRClose.AddCell(Wpcell);
                    if (dataSet.Tables[1].Rows.Count > 0)
                    {
                        Wpcell = new PdfPCell(new Phrase(new Chunk("6.1", font9)));
                        PSSRClose.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("PSSR Lead", font9)));
                        PSSRClose.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[9].Rows[0][0].ToString(), font9)));
                        PSSRClose.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[9].Rows[0][1].ToString(), font9)));
                        PSSRClose.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[9].Rows[0][2].ToString(), font9)));
                        PSSRClose.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[9].Rows[0][3].ToString(), font9)));
                        PSSRClose.AddCell(Wpcell);

                        Wpcell = new PdfPCell(new Phrase(new Chunk("6.2", font9)));
                        PSSRClose.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("PSSR Sign off", font9)));
                        PSSRClose.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[10].Rows[0][0].ToString(), font9)));
                        PSSRClose.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[10].Rows[0][1].ToString(), font9)));
                        PSSRClose.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[10].Rows[0][2].ToString(), font9)));
                        PSSRClose.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[10].Rows[0][3].ToString(), font9)));
                        PSSRClose.AddCell(Wpcell);

                    }
                    else
                    {
                        
                            Wpcell = new PdfPCell(new Phrase(new Chunk("Data Not Available", font9)));
                             Wpcell.Colspan = 6;
                            PSSRClose.AddCell(Wpcell);
                        

                    }

                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                    Wpcell = new PdfPCell(new Phrase(new Chunk("7", font8)));
                    Wpcell.BackgroundColor = new BaseColor(System.Drawing.Color.LightGray);
                    PSSRClose.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Close out", font8)));
                    Wpcell.Colspan = 5;
                    Wpcell.BackgroundColor = new BaseColor(System.Drawing.Color.LightGray);
                    PSSRClose.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("7.1", font9)));
                    PSSRClose.AddCell(Wpcell);
                    PSSRClose.AddCell(dataSet.Tables[0].Rows[0][33].ToString() == "1" ? CheckedCheckbox : uncheckbox);

                   
                    Wpcell = new PdfPCell(new Phrase(new Chunk("MOC implemented. All implementation action items listed and PSSR have all been completed. MOC Request can be closed", font9)));
                    Wpcell.Colspan = 4;
                    PSSRClose.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                    PSSRClose.AddCell(Wpcell);
                    PSSRClose.AddCell(dataSet.Tables[0].Rows[0][33].ToString() == "2" ? CheckedCheckbox : uncheckbox);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("MOC Cancelled. Reason for cancellation", font9)));
                    Wpcell.Colspan = 4;
                    PSSRClose.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("7.2", font9)));
                    PSSRClose.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("MOC Owner", font8)));
                    PSSRClose.AddCell(Wpcell);
                    if (dataSet.Tables[14].Rows.Count > 0)
                    {
                        Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[14].Rows[0][0].ToString(), font9)));
                     
                    }
                    else
                    {
                        Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                      
                    }
                    PSSRClose.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("MOC Priority", font8)));
                    PSSRClose.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[0].Rows[0][18].ToString(), font9)));
                    Wpcell.Colspan = 2;
                    PSSRClose.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("7.3", font9)));
                    PSSRClose.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Closed By", font8)));
                    PSSRClose.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[0].Rows[0][20].ToString(), font9)));
                    PSSRClose.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Closed Date", font8)));
                    PSSRClose.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[0].Rows[0][21].ToString(), font9)));
                    Wpcell.Colspan = 2;
                    PSSRClose.AddCell(Wpcell);
                    pdfDoc.Add(PSSRClose);

                    //PdfPTable closurelist = new PdfPTable(6);
                    //closurelist.TotalWidth = 750f;
                    //closurelist.LockedWidth = true;
                    //closurelist.SetWidths(new float[] { 2f, 14f, 2f, 2f, 2f, 8f });
                    
                    //    Wpcell = new PdfPCell(new Phrase(new Chunk("6", font8)));
                    //    closurelist.AddCell(Wpcell);
                    //    Wpcell = new PdfPCell(new Phrase(new Chunk("Closure Details", font8)));
                    //    Wpcell.Colspan = 5;
                    //    closurelist.AddCell(Wpcell);
                    //    Wpcell = new PdfPCell(new Phrase(new Chunk("6.1", font8)));
                    //    closurelist.AddCell(Wpcell);
                    //    Wpcell = new PdfPCell(new Phrase(new Chunk("Closure CheckList", font8)));
                    //    Wpcell.Colspan = 5;
                    //    closurelist.AddCell(Wpcell);
                   
                    //Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    //closurelist.AddCell(Wpcell);
                    //Wpcell = new PdfPCell(new Phrase(new Chunk("Section", font8)));
                    //closurelist.AddCell(Wpcell);
                    //Wpcell = new PdfPCell(new Phrase(new Chunk("Yes", font8)));
                    //closurelist.AddCell(Wpcell);
                    //Wpcell = new PdfPCell(new Phrase(new Chunk("No", font8)));
                    //closurelist.AddCell(Wpcell);
                    //Wpcell = new PdfPCell(new Phrase(new Chunk("N/A", font8)));
                    //closurelist.AddCell(Wpcell);
                    //Wpcell = new PdfPCell(new Phrase(new Chunk("Remarks", font8)));
                    //closurelist.AddCell(Wpcell);
                    //if (dataSet.Tables[12].Rows.Count > 0)
                    //{
                    //    for (int rows = 0; rows < dataSet.Tables[12].Rows.Count; rows++)
                    //    {

                    //        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[12].Rows[rows][0].ToString(), font9)));
                    //        closurelist.AddCell(Wpcell);
                    //        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[12].Rows[rows][1].ToString(), font9)));
                    //        closurelist.AddCell(Wpcell);
                    //        closurelist.AddCell(dataSet.Tables[12].Rows[rows][2].ToString() == "1" ? CheckedCheckbox : uncheckbox);
                    //        closurelist.AddCell(dataSet.Tables[12].Rows[rows][2].ToString() == "2" ? CheckedCheckbox : uncheckbox);
                    //        closurelist.AddCell(dataSet.Tables[12].Rows[rows][2].ToString() == "3" ? CheckedCheckbox : uncheckbox);

                    //        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[12].Rows[rows][3].ToString(), font9)));
                    //        closurelist.AddCell(Wpcell);

                    //    }
                    //}

                    //else
                    //{
                    //    for (int column = 0; column < dataSet.Tables[12].Columns.Count; column++)
                    //    {
                    //        Wpcell = new PdfPCell(new Phrase(new Chunk("Data Not Available", font9)));

                    //        closurelist.AddCell(Wpcell);

                    //    }

                    //    Wpcell = new PdfPCell(new Phrase(new Chunk("Data Not Available", font9)));

                    //    closurelist.AddCell(Wpcell);

                    //    Wpcell = new PdfPCell(new Phrase(new Chunk("Data Not Available", font9)));

                    //    closurelist.AddCell(Wpcell);
                        

                    //}
                   

                    //    Wpcell = new PdfPCell(new Phrase(new Chunk("6.2", font8)));
                    //    closurelist.AddCell(Wpcell);
                        
                    
                    
                
                 
                  
                    //pdfDoc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());

                    //pdfDoc.Add(closurelist);
                   
                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));
                    pdfDoc.Close();
                    Response.Write(pdfDoc);
                    Response.End();


                }
            }

            catch (Exception objException)
            {
                // LogManager.Instance.Error(objException);
                throw new Exception(objException.Message);
            }

            return View();
        }

        private static PdfPCell PhraseCell(Phrase phrase, int align)
        {
            PdfPCell cell = new PdfPCell(phrase);
            //  cell.BorderColor = Color.;
            //cell.VerticalAlignment = PdfCell.ALIGN_TOP;
            cell.HorizontalAlignment = align;
            cell.PaddingBottom = 2f;
            cell.PaddingTop = 0f;
            return cell;
        }

    }
}