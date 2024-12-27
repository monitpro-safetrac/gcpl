using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MonitPro.Validations;
using MonitPro.Models;
using MonitPro.BLL;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.IO;
using MonitPro.Models.PSSR;
using MonitPro.DAL;

namespace WorkPermitSystem.Controllers
{
    [ValidateSession]
    public class PSSRController : Controller
    {
        PSSRBLL pssrBLL = new PSSRBLL();
        CAPABLL CapaBLL = new CAPABLL();
        List<Employee> EmployeeList = new List<Employee>();

        public List<Role> UserRoles { get; private set; }

        public PSSRController()
        {
            EmployeeList = pssrBLL.GetAllEmployees();
        }
        public ActionResult PSSRChart()
        {
            PSSRDashboard pssrdash = new PSSRDashboard();
            pssrdash.Roles = CurrentUser.Roles;
            pssrdash.UserFullName = CurrentUser.FullName;
            pssrdash.UserID = CurrentUser.UserID;
            pssrdash.ProfileImage = CurrentUser.ProfileImage;
            pssrdash.IsRestrict = CurrentUser.IsRestrict;
            return View(pssrdash);
        }

        public ActionResult ActionByCount(DateTime startDate, DateTime endDate)
        {
            PSSRDashboard dashboard = new PSSRDashboard
            {
                Roles = UserRoles,
                actionbystatusCountList = pssrBLL.GetActionByRecomStatusCount(startDate, endDate)
            };

            return Json(dashboard.actionbystatusCountList.ToList(), JsonRequestBehavior.AllowGet);
        }

        //public ActionResult PriorityRecommCount(DateTime startDate, DateTime endDate)
        //{
        //    PSSRDashboard dash = new PSSRDashboard
        //    {
        //        Roles = UserRoles,
        //        priorityCountList = pssrBLL.GetpriorityRecommCount(startDate, endDate)
        //    };
        //    return Json(dash.priorityCountList.ToList(), JsonRequestBehavior.AllowGet);
        //}
        public ActionResult MonthwiseStatusCount(DateTime startDate, DateTime endDate)
        {
            PSSRDashboard dash = new PSSRDashboard()
            {
                Roles = UserRoles,
                MonthStatusCountList = pssrBLL.GetMonthwiseStatusCount(startDate, endDate)
            };
            return Json(dash.MonthStatusCountList.ToList(), JsonRequestBehavior.AllowGet);
        }






        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetPlantMOCNumber(int? PlantID)
        {

            var cs = pssrBLL.GetMocNumberList(PlantID).Select(m => new SelectListItem()
            {
                Value = m.ID.ToString(),
                Text = m.MOCNo
            });
            return Json(cs, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CreatePSSR(int PSSRID = 0)
        {
            CreatePSSRModel crPSSR = new CreatePSSRModel();
            if (crPSSR.CurrentSessionID == crPSSR.PrevoiusSessionID)
            {
            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";
            }
            crPSSR = pssrBLL.GetPSSR(PSSRID);
            crPSSR.GetCheckLists = pssrBLL.GetCheckListDD();
            crPSSR.PSSRCategoryList = pssrBLL.GetPSSRCatory();
            crPSSR.PSSRPlantList = pssrBLL.GetPlants();
            crPSSR.PSSRTypeList = pssrBLL.GetPSSRType();
            List<MOCNumberListModel> list = new List<MOCNumberListModel>();
            crPSSR.MOCList = list;
            crPSSR.Roles = CurrentUser.Roles;
            crPSSR.UserFullName = CurrentUser.FullName;
            crPSSR.UserID = CurrentUser.UserID;
            crPSSR.ProfileImage = CurrentUser.ProfileImage;
            crPSSR.IsRestrict = CurrentUser.IsRestrict;
            return View(crPSSR);
        }
        [HttpPost]
        public ActionResult CreatePSSR(CreatePSSRModel createPSSRModel)
        {
            if (createPSSRModel.CurrentSessionID == createPSSRModel.PrevoiusSessionID)
            {
            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";
            }
            if (createPSSRModel.PSSRID == 0)
            {
                createPSSRModel.GetCheckLists = pssrBLL.GetCheckListDD();
            }
            createPSSRModel.UserID = CurrentUser.UserID;
            createPSSRModel.PSSRCategoryList = pssrBLL.GetPSSRCatory();
            createPSSRModel.PSSRPlantList = pssrBLL.GetPlants();
            createPSSRModel.PSSRTypeList = pssrBLL.GetPSSRType();
            List<MOCNumberListModel> list = new List<MOCNumberListModel>();
            createPSSRModel.MOCList = list;
            createPSSRModel.CreatedByID = CurrentUser.UserID;
            createPSSRModel.PSSRID = pssrBLL.PSSRInsertUpdate(createPSSRModel);
            if (createPSSRModel.SaveButton == 1)
            {
                ViewBag.savemessege = string.Format("PSSR ID {0} Attached Successfully", createPSSRModel.PSSRID);
            }
            else if (createPSSRModel.PSSRID > 0)
            {
                ViewBag.Message = string.Format("PSSR {0} saved Successfully", createPSSRModel.PSSRID);
            }
            createPSSRModel.Roles = CurrentUser.Roles;
            createPSSRModel.UserFullName = CurrentUser.FullName;
          
            createPSSRModel.ProfileImage = CurrentUser.ProfileImage;
            createPSSRModel.IsRestrict = CurrentUser.IsRestrict;
            return View(createPSSRModel);
        }
        public ActionResult EditPSSR(int PSSRID = 0)
        {
            CreatePSSRModel crPSSR = new CreatePSSRModel();
            crPSSR = pssrBLL.GetPSSR(PSSRID);
            crPSSR.IdentityChecklist = pssrBLL.GetOverallCheckList(PSSRID);
            crPSSR.GetCheckLists = pssrBLL.GetAssignedChecklist(PSSRID);
            crPSSR.PSSRCategoryList = pssrBLL.GetPSSRCatory();
            crPSSR.PSSRPlantList = pssrBLL.GetPlants();
            crPSSR.PSSRTypeList = pssrBLL.GetPSSRType();
            crPSSR.GetAssignTeams = pssrBLL.GetPSSRAssign(PSSRID);
            crPSSR.AllPSSRObservation = pssrBLL.GetAllPSSRObservation();
            List<MOCNumberListModel> list = new List<MOCNumberListModel>();
            crPSSR.MOCList = list;
            crPSSR.Roles = CurrentUser.Roles;
            crPSSR.UserFullName = CurrentUser.FullName;
            crPSSR.UserID = CurrentUser.UserID;
            crPSSR.ProfileImage = CurrentUser.ProfileImage;
            crPSSR.IsRestrict = CurrentUser.IsRestrict;
            foreach (var item in crPSSR.Roles)
            {
                crPSSR.RoleID = item.RoleID;
            }
            return View(crPSSR);
        }
        [HttpPost]
        public ActionResult EditPSSR(CreatePSSRModel createPSSRModel)
        {
            createPSSRModel.CreatedByID = CurrentUser.UserID;
            createPSSRModel.PSSRCategoryList = pssrBLL.GetPSSRCatory();
            createPSSRModel.PSSRPlantList = pssrBLL.GetPlants();
            createPSSRModel.AllPSSRObservation = pssrBLL.GetAllPSSRObservation();
            createPSSRModel.UserID = CurrentUser.UserID;
            //  createPSSRModel.GetCheckLists = pssrBLL.GetCheckListDD();
            if (createPSSRModel.PSSRAttach != null)
            {


                var fileName = Path.GetFileName(createPSSRModel.PSSRAttach.FileName);
                var path = Path.Combine(Server.MapPath("~/PSSRAttachment/"), fileName);
                createPSSRModel.PSSRAttach.SaveAs(path);
                createPSSRModel.PSSRID = pssrBLL.PSSRInsertUpdate(createPSSRModel);

            }
            else
            {
                createPSSRModel.PSSRID = pssrBLL.PSSRInsertUpdate(createPSSRModel);
            }
            createPSSRModel.GetAssignTeams = pssrBLL.GetPSSRAssign(createPSSRModel.PSSRID);

            createPSSRModel.PSSRTypeList = pssrBLL.GetPSSRType();
            List<MOCNumberListModel> list = new List<MOCNumberListModel>();
            createPSSRModel.MOCList = list;
            if (createPSSRModel.SaveButton == 1)
            {
                ViewBag.savemessege = string.Format("PSSR ID {0} Attached Successfully", createPSSRModel.PSSRID);
            }
            else if ((createPSSRModel.PSSRID > 0 && createPSSRModel.PSSRStatus != 3 && createPSSRModel.SaveButton == 2))
            {
                ViewBag.messege = string.Format("PSSR ID {0} Data Saved Successfully", createPSSRModel.PSSRID);
            }
            else if (createPSSRModel.PSSRID > 0 && createPSSRModel.PSSRStatus == 3)
            {
                ViewBag.subappromessage = string.Format("PSSR ID {0} is submitted for Approval ", createPSSRModel.PSSRID);
            }
           
            createPSSRModel.GetCheckLists = pssrBLL.GetAssignedChecklist(createPSSRModel.PSSRID);
            createPSSRModel.Roles = CurrentUser.Roles;
            createPSSRModel.UserFullName = CurrentUser.FullName;
         
            createPSSRModel.ProfileImage = CurrentUser.ProfileImage;
            createPSSRModel.IsRestrict = CurrentUser.IsRestrict;
            foreach (var item in createPSSRModel.Roles)
            {
                createPSSRModel.RoleID = item.RoleID;
            }
            return View(createPSSRModel);
        }

        public ActionResult AssignTeam(int PSSRID)
        {
            AssignTeamViewModel assignTeamView = new AssignTeamViewModel();
            if (assignTeamView.CurrentSessionID == assignTeamView.PrevoiusSessionID)
            {
            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";
            }
            assignTeamView = pssrBLL.GetPSSRAssign(PSSRID);
            List<Employee> ObserverList = pssrBLL.GetAssignTeamMembers(PSSRID);
            assignTeamView.createModel = pssrBLL.GetPSSR(PSSRID);
            assignTeamView.ChairPersonList = EmployeeList;
            assignTeamView.CoordinatorList = EmployeeList;
            assignTeamView.EnggLeadList = EmployeeList;
            assignTeamView.HSELeadList = EmployeeList;
            assignTeamView.OperationHeadList = EmployeeList;
            assignTeamView.PSSRLeadList = EmployeeList;
            ViewBag.TeamMembersList = new SelectList(EmployeeList, "UserID", "FullName");
            ViewBag.ObserList = new SelectList(ObserverList, "UserID", "FullName");
            assignTeamView.DepartmentList = pssrBLL.GetDepartmentList();

            assignTeamView.Roles = CurrentUser.Roles;
            assignTeamView.UserFullName = CurrentUser.FullName;
            assignTeamView.UserID = CurrentUser.UserID;
            assignTeamView.ProfileImage = CurrentUser.ProfileImage;
            assignTeamView.IsRestrict = CurrentUser.IsRestrict;
            return View(assignTeamView);
        }
        [HttpPost]
        public ActionResult SaveAssignTeam(int PSSRID, int Coordinator, int ChairPerson, int OL, int HSELead, int EnggLead, int PSSRLead, string MemberList)
        {
            string strMessage = String.Empty;
            try
            {
                int UserID = CurrentUser.UserID;
                pssrBLL.PSSRAssignTeamInsert(PSSRID, Coordinator, ChairPerson, OL, HSELead, EnggLead, PSSRLead, MemberList, UserID);
                strMessage = "Data Saved Successfully";
            }
            catch (Exception ex)
            {
                strMessage = ex.Message;
            }
            return Json(new { strMessage });
        }
        public ActionResult PSSRList()
        {
            CreatePSSRModel createPSSR = new CreatePSSRModel();
            if (createPSSR.CurrentSessionID == createPSSR.PrevoiusSessionID)
            {
            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";
            }
            createPSSR.PSSRCategoryList = pssrBLL.GetPSSRCatory();
            createPSSR.PSSRPlantList = pssrBLL.GetPlants();
            createPSSR.PSSRListView = pssrBLL.GetAllPSSRList();
            createPSSR.PSSRTypeList = pssrBLL.GetPSSRType();
            createPSSR.PSSRStatusList = pssrBLL.GetPSSRStatus();
            createPSSR.GetAllAssignTeams = pssrBLL.GetAllPSSRAssign();
            createPSSR.AllPSSRObservation = pssrBLL.GetAllPSSRObservation();

            createPSSR.Roles = CurrentUser.Roles;
            createPSSR.UserFullName = CurrentUser.FullName;
            createPSSR.UserID = CurrentUser.UserID;
            createPSSR.ProfileImage = CurrentUser.ProfileImage;
            createPSSR.IsRestrict = CurrentUser.IsRestrict;
            return View(createPSSR);
        }
        [HttpPost]
        public ActionResult PSSRList(SearchList searchList)
        {
           CreatePSSRModel createPSSR = new CreatePSSRModel();
            if (createPSSR.CurrentSessionID == createPSSR.PrevoiusSessionID)
            {
            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";
            }
            string SNO = Request["Sno"];
            if(SNO != null)
            {
                try
                {
                    createPSSR.PSSRListView = pssrBLL.GetAllPSSRList();

                    var PSSRList = createPSSR.PSSRListView.Find(x => x.SNO == int.Parse(SNO));

                    var Attachment = Request.Files[int.Parse(SNO)-1] as HttpPostedFileBase;

                    var fileName = Path.GetFileName(Attachment.FileName);
                    if (Attachment != null && Attachment.ContentLength > 0)
                    {

                        PSSRList.Attachment = Attachment.FileName;
                        var path = Path.Combine(Server.MapPath("~/PSSRAttachment/"), fileName);
                        Attachment.SaveAs(path);

                        using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
                        {
                            SqlCommand objCom = new SqlCommand();
                            objCom.CommandText = "PSSRAttachmentInsert";
                            objCom.CommandType = CommandType.StoredProcedure;
                            objCom.Parameters.AddWithValue("@PSSRID", (object)PSSRList.PSSRID);
                            objCom.Parameters.AddWithValue("@fileName ", fileName);
                            objCon.Open();
                            objCom.Connection = objCon;
                            objCom.ExecuteNonQuery();
                            objCon.Close();

                            ViewBag.issave = "Y";

                        }
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            createPSSR.PSSRCategoryList = pssrBLL.GetPSSRCatory();
            createPSSR.PSSRPlantList = pssrBLL.GetPlants();
            createPSSR.PSSRListView = pssrBLL.SearchPSSRList(searchList);
            createPSSR.PSSRTypeList = pssrBLL.GetPSSRType();
            createPSSR.PSSRStatusList = pssrBLL.GetPSSRStatus();
            createPSSR.GetAllAssignTeams = pssrBLL.GetAllPSSRAssign();
            createPSSR.AllPSSRObservation = pssrBLL.GetAllPSSRObservation();

            createPSSR.Roles = CurrentUser.Roles;
            createPSSR.UserFullName = CurrentUser.FullName;
            createPSSR.UserID = CurrentUser.UserID;
            createPSSR.ProfileImage = CurrentUser.ProfileImage;
            createPSSR.IsRestrict = CurrentUser.IsRestrict;
            return View(createPSSR);
        }
        public ActionResult PSSRObservation(int PSSRID=0, int OBID =0)
        {
            PSSR_Observation pSSR_Observation = new PSSR_Observation();
            pSSR_Observation.EmployeeList = pssrBLL.GetAllEmployees();
            pSSR_Observation.PSSRCategoryList = pssrBLL.GetPSSRCatory();
            pSSR_Observation.PSSRModel = pssrBLL.GetPSSR(PSSRID);
            pSSR_Observation.PriorityList = pssrBLL.GetPriority();
            pSSR_Observation.RecommStatusList = pssrBLL.RecommStatusListDD();

            pSSR_Observation.TargetDate = DateTime.Today.AddDays(30).ToString("dd/MM/yyyy");

            pSSR_Observation.OBlist = pssrBLL.GetOBList(PSSRID,OBID);

            foreach (var a in pSSR_Observation.OBlist)
            {
                if (CurrentUser.FullName == a.ActionByName)
                {
                   pSSR_Observation.ActionByName = a.ActionByName;
                }
            }

            pSSR_Observation.Roles = CurrentUser.Roles;
            foreach (var item in pSSR_Observation.Roles)
            {
                pSSR_Observation.RoleID = item.RoleID;
            }
            pSSR_Observation.PSSRID = pSSR_Observation.PSSRModel.PSSRID;
            pSSR_Observation.UserFullName = CurrentUser.FullName;
            pSSR_Observation.UserID = CurrentUser.UserID;
            pSSR_Observation.ProfileImage = CurrentUser.ProfileImage;
            pSSR_Observation.IsRestrict = CurrentUser.IsRestrict;
            return View(pSSR_Observation);
        }
       
        public ActionResult SavePSSRObservation(PSSR_Observation model)
        {
            model.RecommUserID = CurrentUser.UserID;
            if (Request.Files.Count > 0)
            {
                HttpFileCollectionBase files = Request.Files;

                model.PSSRObAttachment = files[0];
                string fileName = model.PSSRObAttachment.FileName;

                // create the uploads folder if it doesn't exist

                string path = Path.Combine(Server.MapPath("~/PSSRRecomAttachment/"), fileName);

                // save the file
                model.PSSRObAttachment.SaveAs(path);
                model.PSSRObAttachmentName = fileName;
                var affect = pssrBLL.PSSRObservationInsertUpdate(model);

            }
            else
            {
                var affect = pssrBLL.PSSRObservationInsertUpdate(model);

            }

            model.Roles = CurrentUser.Roles;
            model.UserFullName = CurrentUser.FullName;
            model.UserID = CurrentUser.UserID;
            model.ProfileImage = CurrentUser.ProfileImage;
            model.IsRestrict = CurrentUser.IsRestrict;
            return View();
        }



        [HttpPost]
        public ActionResult Editbservation(int RecommID)
        {
            PSSR_Observation Editob = new PSSR_Observation();

            Editob = pssrBLL.EditObservation(RecommID);

            if (Editob.ActionBy == CurrentUser.UserID)
            {
                Editob.CompletedDate = DateTime.Now.ToString("dd/MM/yyyy");
            }

            return Json(new { Editob });
            //return View(insObservation);
        }
        [HttpPost]
        public ActionResult DeleteOBImage(int obid)
        {

            pssrBLL.DeletePSSROBImage(obid);
            string strMessage = "Deleted Successfully";
            return Json(new { strMessage });
        }
        public ActionResult CheckList(int pcmID = 0, int PSSRID = 0)
        {
            MainCheckListModel model = new MainCheckListModel();
            model.PSSRID = PSSRID;
           var checkDD= pssrBLL.GetAssignedChecklist(PSSRID);
            model.checkListDD = checkDD.Where(x => x.yesNo == true).ToList();
            var getcheck = pssrBLL.GetCheckList(pcmID);
            var savecheck = pssrBLL.PSSRSaveCheckList(pcmID,PSSRID);
            if(savecheck.Count > 0)
            {
                model.CheckLists = savecheck;
            }
            else
            {
                model.CheckLists = getcheck;
            }
            model.CheckCheckLists = pssrBLL.GetOverallCheckList(PSSRID);
          
            model.PCMID = pcmID;
            model.Roles = CurrentUser.Roles;
            model.UserFullName = CurrentUser.FullName;
            model.UserID = CurrentUser.UserID;
            model.ProfileImage = CurrentUser.ProfileImage;
            model.IsRestrict = CurrentUser.IsRestrict;
            return View(model);
        }
        [HttpPost]
        public ActionResult CheckList(MainCheckListModel ch,int pcmID =0, int PSSRID =0)
        {
            MainCheckListModel main = new MainCheckListModel();
            ch.UserID = CurrentUser.UserID;
            ch.UserFullName = CurrentUser.FullName;
            main.PSSRID = pssrBLL.CheckListInsert(ch);
            if(PSSRID > 0)
            {
                ViewBag.checklistMessage = string.Format("PSSR ID {0} Data Saved Successfully",PSSRID);
            }
            main.PSSRID = PSSRID;
            var checkDD = pssrBLL.GetAssignedChecklist(PSSRID);
            main.checkListDD = checkDD.Where(x => x.yesNo == true).ToList();
            var getcheck = pssrBLL.GetCheckList(pcmID);
            var savecheck = pssrBLL.PSSRSaveCheckList(pcmID, PSSRID);
            if (savecheck.Count > 0)
            {
                main.CheckLists = savecheck;
            }
            else
            {
                main.CheckLists = getcheck;
            }
            main.CheckCheckLists = pssrBLL.GetOverallCheckList(PSSRID);

            main.Roles = CurrentUser.Roles;
            main.UserFullName = CurrentUser.FullName;
            main.UserID = CurrentUser.UserID;
            main.ProfileImage = CurrentUser.ProfileImage;
            main.IsRestrict = CurrentUser.IsRestrict;
            return View(main);
        }



        public ActionResult AllPSSRObservation()
        {
            PSSR_Observation observations = new PSSR_Observation();
            List<AllPSSR_Observation> AllOB = new List<AllPSSR_Observation>();
            if (observations.CurrentSessionID == observations.PrevoiusSessionID)
            {
            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";
            }
            observations.PlantList = pssrBLL.GetPlants();
            observations.PSSRCategoryList = pssrBLL.GetPSSRCatory();
            observations.EmployeeList = pssrBLL.GetAllEmployees();
            observations.PriorityList = pssrBLL.GetPriority();
            AllOB = pssrBLL.GetAllPSSRObservation();
            observations.OBlist = AllOB;
            observations.Roles = CurrentUser.Roles;
            observations.UserFullName = CurrentUser.FullName;
            observations.UserID = CurrentUser.UserID;
            observations.ProfileImage = CurrentUser.ProfileImage;
            observations.IsRestrict = CurrentUser.IsRestrict;
            return View(observations);
        }
        [HttpPost]
        public ActionResult AllPSSRObservation(SearchPSSRObservation searchModel)
        {
            PSSR_Observation observations = new PSSR_Observation();
            List<AllPSSR_Observation> AllOB = new List<AllPSSR_Observation>();
            if (observations.CurrentSessionID == observations.PrevoiusSessionID)
            {
            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";
            }
            observations.PlantList = pssrBLL.GetPlants();
            observations.PSSRCategoryList = pssrBLL.GetPSSRCatory();
            observations.EmployeeList = pssrBLL.GetAllEmployees();
            observations.PriorityList = pssrBLL.GetPriority();
            observations.OBlist = pssrBLL.SearchPSSRObservation(searchModel);
            observations.Roles = CurrentUser.Roles;
            observations.UserFullName = CurrentUser.FullName;
            observations.UserID = CurrentUser.UserID;
            observations.ProfileImage = CurrentUser.ProfileImage;
            observations.IsRestrict = CurrentUser.IsRestrict;
            return View(observations);
        }
       public ActionResult PSSRHistory()
        {
            PSSRHistoryMainModel historyMainModel = new PSSRHistoryMainModel();
            if (historyMainModel.CurrentSessionID == historyMainModel.PrevoiusSessionID)
            {
            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";
            }
            historyMainModel.PlantList = pssrBLL.GetPlants();
            historyMainModel.HistoryList = pssrBLL.GetAllPSSRHistoryList();
            historyMainModel.TypeList = pssrBLL.GetPSSRType();
            historyMainModel.Roles = CurrentUser.Roles;
            historyMainModel.UserFullName = CurrentUser.FullName;
            historyMainModel.UserID = CurrentUser.UserID;
            historyMainModel.ProfileImage = CurrentUser.ProfileImage;
            historyMainModel.IsRestrict = CurrentUser.IsRestrict;
            return View(historyMainModel);
        }
        [HttpPost]
        public ActionResult PSSRHistory(PSSRSearchHistory searchModel)
        {
            PSSRHistoryMainModel historyMainModel = new PSSRHistoryMainModel();
            if (historyMainModel.CurrentSessionID == historyMainModel.PrevoiusSessionID)
            {
            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";
            }
            historyMainModel.PlantList = pssrBLL.GetPlants();
            historyMainModel.TypeList = pssrBLL.GetPSSRType();
            historyMainModel.HistoryList = pssrBLL.SearchPSSRHistoryList(searchModel);
            historyMainModel.Roles = CurrentUser.Roles;
            historyMainModel.UserFullName = CurrentUser.FullName;
            historyMainModel.UserID = CurrentUser.UserID;
            historyMainModel.ProfileImage = CurrentUser.ProfileImage;
            historyMainModel.IsRestrict = CurrentUser.IsRestrict;
            return View(historyMainModel);
        }


        [HttpPost]
        public ActionResult PSSRUpdateStatus(int PSSRID)
        {
            UpdatePSSRStatus pssr = new UpdatePSSRStatus();
            pssr.PSSRID = PSSRID;
            return PartialView("PSSRUpdateStatus", pssr);
        }
        [HttpPost]
        public ActionResult UpdatePSSRStatus(int PSSRID, int StatusID, string Comments)
        {
            string Message = String.Empty;
            try
            {
                pssrBLL.UpdatePSSRStatus(PSSRID, StatusID, Comments, CurrentUser.UserID);
                Message = "Data Saved Successfully";
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
            return Json(new { Message });
        }

        [HttpPost]
        public ActionResult PSSRTargetDateRequest(int RecomID)
        {
            PSSRRequestTargetDate pssr = new PSSRRequestTargetDate();
            pssr.RecomID = RecomID;
            return PartialView("PSSRTargetDateRequest", pssr);
        }
        [HttpPost]
        public ActionResult PSSRTargetDateApprove(int Recomid)
        {
            TargetDateApprove approve = new TargetDateApprove();
            approve = pssrBLL.GetRequestedTargetDateDetails(Recomid);
            return View(approve);
        }
            [HttpPost]
    public ActionResult UpdatePSSRTargetDateRequest(int Recomid, int Identiy, string Comments=null,string RevisedTargetDate=null)
    {
        string Message = String.Empty;
        try
        {
            pssrBLL.UpdateTargetDateRequest(Recomid, Identiy, Comments, RevisedTargetDate);
            Message = "Data Saved Successfully";
        }
        catch (Exception ex)
        {
            Message = ex.Message;
        }
        return Json(new { Message });
    }
    public  ActionResult PSSRPdf(int id)
        {
            try
            {
                using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[PSSRPDF]";
                    objCom.Parameters.AddWithValue("@PSSRID", id);
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCon.Open();
                    objCom.Connection = objCon;
                    SqlDataAdapter Adapter = new SqlDataAdapter();
                    Adapter.SelectCommand = objCom;
                    DataSet dataSet = new DataSet();
                    Adapter.Fill(dataSet);
                    objCon.Close();

                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", "filename=PSSRPDF.pdf");
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
                    string imageURL = Server.MapPath("~/Images/SASALogo.png");
                    iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(imageURL);
                    gif.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
                    gif.SpacingBefore = 10f;

                    gif.ScaleAbsolute(140f, 45f);
                    Wpcell = new PdfPCell(gif);

                    TitleTable.AddCell(Wpcell);

                    var phrase = new Phrase(new Chunk("\nPre-Startup Safety Review (PSSR)" + "\n\n\n", FontFactory.GetFont("Times New Roman", 14, iTextSharp.text.Font.BOLD)));

                    TitleTable.AddCell(PhraseCell(phrase, PdfPCell.ALIGN_CENTER));

                    Wpcell = new PdfPCell(new Phrase("\n \t \t \t PSSR ID \t ".PadRight(5) + dataSet.Tables[0].Rows[0][0].ToString(), FontFactory.GetFont("Times New Roman", 13, iTextSharp.text.Font.BOLD)));
                    Wpcell.HorizontalAlignment = Element.ALIGN_LEFT;

                    TitleTable.AddCell(Wpcell);

                    String FONT = "C:/Windows/Fonts/wingding.ttf";
                    String CheckedCheckboxText = "\u00fe";
                    String BlankCheckboxText = "o";
                    BaseFont bf = BaseFont.CreateFont(FONT, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    iTextSharp.text.Font f = new iTextSharp.text.Font(bf, 10);
                    Paragraph CheckedCheckbox = new Paragraph(CheckedCheckboxText, f);
                    Paragraph uncheckbox = new Paragraph(BlankCheckboxText, f);

                    pdfDoc.Add(TitleTable);
                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));
                    var font8 = FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD);
                    var font9 = FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL);
                    var font7 = FontFactory.GetFont("Times New Roman", 11, iTextSharp.text.Font.BOLD);
                    PdfPTable Details = new PdfPTable(4);
                    Details.TotalWidth = 555f;
                    Details.LockedWidth = true;
                    Details.SetWidths(new float[] { 2f, 14f, 7f, 7f });
                    Details.AddCell(PhraseCell(new Phrase("1 ", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Details.AddCell(PhraseCell(new Phrase("PSSR Details ", FontFactory.GetFont("Times New Roman", 11, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Details.AddCell(PhraseCell(new Phrase("Status ", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Details.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][8].ToString(), FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    pdfDoc.Add(Details);

                    PdfPTable DetailsContent = new PdfPTable(5);
                    DetailsContent.TotalWidth = 555f;
                    DetailsContent.LockedWidth = true;
                    DetailsContent.SetWidths(new float[] { 2f, 6f, 8f, 7f, 7f });

                    DetailsContent.AddCell(PhraseCell(new Phrase("1.1 ", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    DetailsContent.AddCell(PhraseCell(new Phrase("Plant/Area ", FontFactory.GetFont("Times New Roman", 11, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    DetailsContent.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][1].ToString(), FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    DetailsContent.AddCell(PhraseCell(new Phrase("System Description ", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    DetailsContent.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][2].ToString(), FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    DetailsContent.AddCell(PhraseCell(new Phrase("1.2 ", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    DetailsContent.AddCell(PhraseCell(new Phrase("PSSR Category ", FontFactory.GetFont("Times New Roman", 11, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    DetailsContent.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][3].ToString(), FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    DetailsContent.AddCell(PhraseCell(new Phrase("If MOC, MOC No", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    DetailsContent.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][13].ToString(), FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    DetailsContent.AddCell(PhraseCell(new Phrase("1.3", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    //DetailsContent.AddCell(PhraseCell(new Phrase("PSSR Type", FontFactory.GetFont("Times New Roman", 11, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                     //DetailsContent.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][4].ToString(), FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    DetailsContent.AddCell(PhraseCell(new Phrase("PSSR Scheduled Date", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[0].Rows[0][5].ToString(), font9)));
                    Wpcell.Colspan = 3;
                    DetailsContent.AddCell(Wpcell);
                    DetailsContent.AddCell(PhraseCell(new Phrase("1.4", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    DetailsContent.AddCell(PhraseCell(new Phrase("Created By", FontFactory.GetFont("Times New Roman", 11, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    DetailsContent.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][6].ToString(), FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    DetailsContent.AddCell(PhraseCell(new Phrase("Created Date", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    DetailsContent.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][7].ToString(), FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    pdfDoc.Add(DetailsContent);

                    PdfPTable attach = new PdfPTable(2);
                    attach.TotalWidth = 555f;
                    attach.LockedWidth = true;
                    attach.KeepTogether = true;
                    attach.SetWidths(new float[] { 2f, 28f });
                    Wpcell = new PdfPCell(new Phrase(new Chunk("1.5", font8)));
                    attach.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Attachment", font8)));
                    attach.AddCell(Wpcell);
                    var attachcheck = dataSet.Tables[0].Rows[0][9].ToString();

                    if (attachcheck != "")
                    {
                        

                            string imagePath = Server.MapPath("~/PSSRAttachment/") + Path.GetFileName(dataSet.Tables[0].Rows[0][9].ToString());

                            iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imagePath);
                            //image.ScaleAbsolute(400f, 400f);
                            // image.ScaleToFit(400f, 400f);
                            float fixedHeight = 200f; // set the desired height in points
                            float aspectRatio = image.Width / image.Height;
                            float fixedWidth = fixedHeight * aspectRatio;
                            image.ScaleAbsolute(fixedWidth, fixedHeight);
                            image.SpacingBefore = 10f;
                            Wpcell = new PdfPCell(image);

                            Wpcell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            Wpcell.Colspan = 2;
                            attach.AddCell(Wpcell);

                            pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                            Wpcell = new PdfPCell(new Phrase(new Chunk("Picture " + dataSet.Tables[0].Rows[0][9].ToString(), font9)));
                            Wpcell.Colspan = 2;
                            attach.AddCell(Wpcell);
                        
                    }
                    else
                    {


                        Wpcell = new PdfPCell(new Phrase(new Chunk("Data Not Available", font9)));
                        Wpcell.Colspan = 2;
                        attach.AddCell(Wpcell);

                    }
                    //   Supimg.SpacingBefore = 0;
                    Wpcell.Colspan = 2;
                    pdfDoc.Add(attach);

                   pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                    PdfPTable approvers = new PdfPTable(5);
                    approvers.TotalWidth = 555f;
                    approvers.LockedWidth = true;
                    approvers.SetWidths(new float[] { 2f, 5.5f, 7f, 10f, 4.5f });

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2", font8)));
                    approvers.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("PSSR Approvals", font8)));

                    Wpcell.Colspan = 5;
                    approvers.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                    approvers.AddCell(Wpcell);
            
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Role" ,font8)));
                    approvers.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name", font8)));
                    approvers.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Remarks", font8)));
                    approvers.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Approval Date", font8)));
                    approvers.AddCell(Wpcell);
                    
                    if(dataSet.Tables[1].Rows.Count >0)
                    {
                        Wpcell = new PdfPCell(new Phrase(new Chunk("2.1", font9)));
                        approvers.AddCell(Wpcell);
                        
                        Wpcell = new PdfPCell(new Phrase(new Chunk("PSSR Lead", font9)));
                        approvers.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[1].Rows[0][0].ToString(), font9)));
                        approvers.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[1].Rows[0][1].ToString() == null ?string.Empty : dataSet.Tables[1].Rows[0][1].ToString(), font9)));
                        approvers.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[1].Rows[0][2].ToString() == null ? string.Empty : dataSet.Tables[1].Rows[0][2].ToString(), font9)));
                        approvers.AddCell(Wpcell);

                        Wpcell = new PdfPCell(new Phrase(new Chunk("2.2", font9)));
                        approvers.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("Engineering Lead", font9)));
                        approvers.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[1].Rows[0][12].ToString(), font9)));
                        approvers.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[1].Rows[0][13].ToString() == null ? string.Empty : dataSet.Tables[1].Rows[0][13].ToString(), font9)));
                        approvers.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[1].Rows[0][14].ToString() == null ? string.Empty : dataSet.Tables[1].Rows[0][14].ToString(), font9)));
                        approvers.AddCell(Wpcell);

                        Wpcell = new PdfPCell(new Phrase(new Chunk("2.3", font9)));
                        approvers.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("HSE Lead", font9)));
                        approvers.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[1].Rows[0][9].ToString(), font9)));
                        approvers.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[1].Rows[0][10].ToString() == null ? string.Empty : dataSet.Tables[1].Rows[0][10].ToString(), font9)));
                        approvers.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[1].Rows[0][11].ToString() == null ? string.Empty : dataSet.Tables[1].Rows[0][11].ToString(), font9)));
                        approvers.AddCell(Wpcell);

                        Wpcell = new PdfPCell(new Phrase(new Chunk("2.4", font9)));
                        approvers.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("PSSR Chairperson", font9)));
                        approvers.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[1].Rows[0][3].ToString(), font9)));
                        approvers.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[1].Rows[0][4].ToString() == null ? string.Empty : dataSet.Tables[1].Rows[0][4].ToString(), font9)));
                        approvers.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[1].Rows[0][5].ToString() == null ? string.Empty : dataSet.Tables[1].Rows[0][5].ToString(), font9)));
                        approvers.AddCell(Wpcell);

                        Wpcell = new PdfPCell(new Phrase(new Chunk("2.5", font9)));
                        approvers.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("Operation Head", font9)));
                        approvers.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[1].Rows[0][6].ToString(), font9)));
                        approvers.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[1].Rows[0][7].ToString() == null ? string.Empty : dataSet.Tables[1].Rows[0][7].ToString(), font9)));
                        approvers.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[1].Rows[0][8].ToString() == null ? string.Empty : dataSet.Tables[1].Rows[0][8].ToString(), font9)));
                        approvers.AddCell(Wpcell);

                       

                       
                    }
                    else
                    {


                        Wpcell = new PdfPCell(new Phrase(new Chunk("Data Not Available", font9)));
                        Wpcell.Colspan = 2;
                        approvers.AddCell(Wpcell);

                    }
                    pdfDoc.Add(approvers);

                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                    PdfPTable Checklist = new PdfPTable(6);
                    Checklist.TotalWidth = 555f;
                    Checklist.LockedWidth = true;
                    Checklist.SetWidths(new float[] { 2f, 14f, 2.5f, 4f, 5f,4f });

                    Wpcell = new PdfPCell(new Phrase(new Chunk("3", font8)));
                    Checklist.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("PSSR Check List", font8)));

                    Wpcell.Colspan = 5;
                    Checklist.AddCell(Wpcell);
                    var CheckHeader = "";
                    var prevCheckHeader = "";
                    int rowno = 0;
                    if (dataSet.Tables[3].Rows.Count > 0)
                    {
                        for (int rows = 0; rows < dataSet.Tables[3].Rows.Count; rows++)
                        {
                            CheckHeader = dataSet.Tables[3].Rows[rows][2].ToString();
                           
                            if (CheckHeader != prevCheckHeader)
                            {
                                rowno = 1;
                               Wpcell = new PdfPCell(new Phrase(new Chunk(" ", font8)));
                                Checklist.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk(CheckHeader, font8)));

                                Wpcell.Colspan = 5;
                                Checklist.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("S.NO", font8)));
                                Checklist.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("Description", font8)));
                                Checklist.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("Yes/No/N/A", font8)));
                                Checklist.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("Remarks", font8)));
                                Checklist.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("Edited by", font8)));
                                Checklist.AddCell(Wpcell);
                                Wpcell = new PdfPCell(new Phrase(new Chunk("Edited Date Time", font8)));
                                Checklist.AddCell(Wpcell);

                                prevCheckHeader = CheckHeader;
                            }
                            for (int column = 0; column < dataSet.Tables[3].Columns.Count; column++)
                                {
                                    if (column != 1 && column != 2 && column !=0)
                                    {
                                        Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[3].Rows[rows][column].ToString(), font9)));
                                        Wpcell.SetLeading(3.0f, 1.0f);
                                        Checklist.AddCell(Wpcell);
                                    }
                                    if(column ==0)
                                {

                                    Wpcell = new PdfPCell(new Phrase(new Chunk(rowno.ToString(), font9)));
                                    Wpcell.SetLeading(3.0f, 1.0f);
                                    Checklist.AddCell(Wpcell);

                                }
                               
                                }
                            rowno++;
                           

                            }
                    }
                    else
                    {
                       

                            Wpcell = new PdfPCell(new Phrase(new Chunk("Data Not Available", font9)));
                            Wpcell.Colspan = 6;
                            Checklist.AddCell(Wpcell);
                        
                    }
                    pdfDoc.Add(Checklist);
                    PdfPTable recommend = new PdfPTable(10);
                    recommend.TotalWidth = 750f;

                    recommend.LockedWidth = true;
                    recommend.KeepTogether = true;

                    recommend.SetWidths(new float[] { 2f, 3.5f, 9f, 9f, 5f, 6f, 5f, 6f, 5f,5f });
                    Wpcell = new PdfPCell(new Phrase(new Chunk("3", font8)));
                    recommend.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Recommendations ", font7)));
                    Wpcell.Colspan = 10;
                    recommend.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    recommend.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Recom ID", font8)));
                    recommend.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Recommendation", font8)));
                    recommend.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Action Taken", font8)));
                    recommend.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Action by", font8)));
                    recommend.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Target Date", font8)));
                    recommend.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Completed Date", font8)));
                    recommend.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Priority", font8)));
                    recommend.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Remarks", font8)));
                    recommend.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Status", font8)));
                    recommend.AddCell(Wpcell);

                    if (dataSet.Tables[2].Rows.Count > 0)
                    {
                        for (int rows = 0; rows < dataSet.Tables[2].Rows.Count; rows++)
                        {
                            for (int column = 0; column < dataSet.Tables[2].Columns.Count; column++)
                            {
                                Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[2].Rows[rows][column].ToString(), font9)));
                                Wpcell.SetLeading(3.0f, 1.0f);
                                recommend.AddCell(Wpcell);
                            }
                        }
                    }
                    else
                    {
                        for (int column = 0; column < dataSet.Tables[2].Columns.Count; column++)
                        {

                            Wpcell = new PdfPCell(new Phrase(new Chunk("Data Not Available", font9)));
                            recommend.AddCell(Wpcell);
                        }
                    }
                    pdfDoc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
                    pdfDoc.NewPage();
                    pdfDoc.Add(recommend);

                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(3)));

                    PdfPTable Closure = new PdfPTable(5);
                    Closure.TotalWidth = 750f;
                    Closure.LockedWidth = true;
                    Closure.SetWidths(new float[] { 2f, 8f, 7f, 6f, 6f });

                    Wpcell = new PdfPCell(new Phrase(new Chunk("4", font8)));
                    Closure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Closure Details", font8)));
                    Wpcell.Colspan = 5;
                    Closure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("4.1", font8)));
                    Closure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Closure Comments", font8)));
                    Closure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[0].Rows[0][11].ToString(), font9)));
                    Wpcell.Colspan = 3;
                    Closure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("4.1", font8)));
                    Closure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Closed By", font8)));
                    Closure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[0].Rows[0][10].ToString(), font9)));
                    Closure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Closed Date Time", font8)));
                    Closure.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[0].Rows[0][12].ToString(), font9)));
                    Closure.AddCell(Wpcell);
                    pdfDoc.Add(Closure);

                    pdfDoc.Close();
                    Response.Write(pdfDoc);
                    Response.End();
                }

                }
            catch (Exception ex)
            {
               // LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
            return View();
        }

        //public ActionResult ConvertPdfToWord()
        //{
        //    // Path to the input PDF file
        //    string pdfFilePath = Server.MapPath("~/path/to/your/pdf.pdf");

        //    // Path to the output Word file
        //    string wordFilePath = Server.MapPath("~/path/to/your/output.docx");

        //    // Create a StringBuilder to store the extracted text
        //    StringBuilder text = new StringBuilder();

        //    // Open the PDF file and extract text
        //    using (PdfReader pdfReader = new PdfReader(pdfFilePath))
        //    {
        //        for (int page = 1; page <= pdfReader.NumberOfPages; page++)
        //        {
        //            ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
        //            string currentText = PdfTextExtractor.GetTextFromPage(pdfReader, page, strategy);
        //            text.Append(currentText);
        //        }
        //    }

        //    // Save the extracted text as a Word document
        //    using (StreamWriter streamWriter = new StreamWriter(wordFilePath, false, Encoding.UTF8))
        //    {
        //        streamWriter.Write(text.ToString());
        //    }

        //    return Content("PDF converted to Word successfully!");
        //}


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