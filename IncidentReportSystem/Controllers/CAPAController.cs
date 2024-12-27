using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MonitPro.Validations;
using MonitPro.Models;
using MonitPro.Models.Incident;
using MonitPro.Models.Account;
using MonitPro.Models.IncidentViewModels;
using MonitPro.BLL;
using IncidentReportSystem.Models;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Net.Mail;
using MonitPro.Common.Library;
using System.IO;
using MonitPro.Models.CAPA;
using MonitPro.Models.CAPAViewModel;
using ClosedXML.Excel;
using MonitPro.Models.MOC;
using System.Text;
using System.Net;
using MonitPro.Models.Common;
using MonitPro.DAL;


namespace WorkPermitSystem.Controllers
{
    [ValidateSession]
    public class CAPAController : Controller
    {

        CAPABLL CapaBLL = new CAPABLL();
        CAPAListViewModel CapaList1 = new CAPAListViewModel();
        List<Role> UserRoles = new List<Role>();
        List<Plants> IncidentPlants = new List<Plants>();
        List<CAPAPlants> capaPlants = new List<CAPAPlants>();
        List<AuditType> AuditType = new List<AuditType>();
        List<CAPAObservationStatus> capaobservationstatus = new List<CAPAObservationStatus>();
        List<UserProfile> UserProfiles = new List<UserProfile>();
        List<CAPASource> CAPASource = new List<CAPASource>();
        List<Status> CAPAStatus = new List<Status>();
        List<CAPACategory> CAPACategory = new List<CAPACategory>();
        List<CAPAPriority> CAPAPriority = new List<CAPAPriority>();
        CreateCAPA NewCAPA = new CreateCAPA();
        List<ContractorEmp> contractoremp = new List<ContractorEmp>();
        SessionDetails sess = new SessionDetails();

        public CAPAController()
        {
            IncidentPlants = CapaBLL.GetPlants();
            capaPlants = CapaBLL.GetcapaPlants();
            AuditType = CapaBLL.GetAuditType();
            CAPASource = CapaBLL.GetCAPASource();
            CAPACategory = CapaBLL.GetCAPACategory();
            CAPAPriority = CapaBLL.GetCAPAPriority();
            CAPAStatus = CapaBLL.GetStatus();
            capaobservationstatus = CapaBLL.GetCAPAObservationStatus();
            contractoremp = CapaBLL.GetContractorEmp();
            UserProfiles = CapaBLL.GetActionList();
            sess = CapaBLL.GetSession(CurrentUser.UserID);
        }

        public ActionResult CAPADashboard()
        {
            CapaList1.CurrentSessionID = CurrentUser.CurrentSessionID;
            CapaList1.PrevoiusSessionID = sess.SessionActive;
            if (CapaList1.CurrentSessionID == CapaList1.PrevoiusSessionID)
            {

            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";

            }
            CapaList1.Roles = CurrentUser.Roles;
            CapaList1.UserFullName = CurrentUser.FullName;
            CapaList1.UserID = CurrentUser.UserID;
            CapaList1.ProfileImage = CurrentUser.ProfileImage;
            CapaList1.IsRestrict = CurrentUser.IsRestrict;
            CapaList1.CapaList.RemoveAll(x => x.CAPAID > 0);
            return View(CapaList1);

        }
        

        public  ActionResult  Sample()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult CreateCAPANew(int capaID = 0)
        {
            NewCAPAModel newcapa = new NewCAPAModel();
            CreateCAPA CreateCAPA = new CreateCAPA();
            CreateCAPA.AuditTime = DateTime.Now.ToString("dd/MM/yyyy");
            newcapa.CurrentSessionID = CurrentUser.CurrentSessionID;
            newcapa.PrevoiusSessionID = sess.SessionActive;
            if (newcapa.CurrentSessionID == newcapa.PrevoiusSessionID)
            {

            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";

            }
            if (capaID > 0)
            {
                CreateCAPA = CapaBLL.GetCAPADetails(capaID);
                newcapa.AuditTime = CreateCAPA.AuditTime;
            }
            var Audit = AuditType.Where(y => y.ID != -1 && y.ID != 0).ToList();
            var sourceList = CAPASource.Where(y => y.ID != -1).ToList();
            var plantsList = IncidentPlants.Where(y => y.ID != -1).ToList();
            var CAPAPlant= capaPlants.Where(y => y.ID != -1 && y.ID != 0).ToList();

            newcapa.CreateCAPA = CreateCAPA;
            newcapa.capaplants = CAPAPlant;
            newcapa.AuditType = Audit;
            newcapa.PlantList = plantsList;
            newcapa.contractorEmp = contractoremp;
            newcapa.CAPASource = sourceList;
            newcapa.Roles = CurrentUser.Roles;
            newcapa.UserFullName = CurrentUser.FullName;
            newcapa.ProfileImage = CurrentUser.ProfileImage;
            newcapa.IsRestrict = CurrentUser.IsRestrict;
            return View(newcapa);
        }

        [HttpPost]
        public ActionResult CreateCAPANew(NewCAPAModel NewCapa)
        {
            NewCapa.CurrentUserID = CurrentUser.UserID;

            CreateCAPA createnew = new CreateCAPA();
            try
            {

                if (NewCapa.CreateCAPA.ImageFile != null)
                {
                    if (NewCapa.CreateCAPA.ImageFile.ContentLength < 5242880)
                    {
                        var fileName = Path.GetFileName(NewCapa.CreateCAPA.ImageFile.FileName);
                        var path = Path.Combine(Server.MapPath("~/CAPAAttachments/"), fileName);
                        NewCapa.CreateCAPA.ImageFile.SaveAs(path);
                        createnew.CAPAID = CapaBLL.CAPAUpdate(NewCapa, CurrentUser.UserID);
                    }
                    else
                    {
                        ViewBag.error = "File size exceeds maximum limit 5 MB.";
                    }
                }

                if (NewCapa.CreateCAPA.ImageFile == null)
                {
                    createnew.CAPAID = CapaBLL.CAPAUpdate(NewCapa, CurrentUser.UserID);
                }
            }
            catch (Exception ex)
            {
                ViewBag.IsValidationFailed = true;
                ModelState.AddModelError("ValidationError", ex.Message);
            }
            var Audit = AuditType.Where(y => y.ID != -1 && y.ID != 0).ToList();
            var sourceList = CAPASource.Where(y => y.ID != -1).ToList();
            var plantsList = IncidentPlants.Where(y => y.ID != -1).ToList();

            if (createnew.CAPAID > 0)
            {
                ViewBag.Message = string.Format("Capa ID_ {0} is Created Successfully.Click Ok to Add Observation", createnew.CAPAID);
            }
            createnew.AuditTime = DateTime.Now.ToString("dd/MM/yyyy");

            NewCapa.CreateCAPA = createnew;
            NewCapa.PlantList = plantsList;
            NewCapa.AuditType = Audit;
            NewCapa.statusList = CAPAStatus;
            NewCapa.contractorEmp = contractoremp;
            NewCapa.CAPASource = sourceList;
            NewCapa.Roles = CurrentUser.Roles;
            NewCapa.UserFullName = CurrentUser.FullName;
            NewCapa.ProfileImage = CurrentUser.ProfileImage;
            NewCapa.DesigID = CurrentUser.Designation;

            return View(NewCapa);

        }
      
            public ActionResult MailCAPA(int capaID)
            {
                try
                {
                    CAPAMailer Mailer = new CAPAMailer();

                    
                    MonitProEmail monitProEmail = new MonitProEmail();
                    
                    List<CAPAEmail> EmailList = new List<CAPAEmail>();
                    EmailList = CapaBLL.GetActionerForMailing(capaID);
                    var CapaData = CapaBLL.GetObservations(capaID, 0);
                    var mailBody = BuildMailBody(CapaData);
                    foreach (var a in EmailList)
                    {
                        StringBuilder CCaddressbuild = new StringBuilder();
                        string[] ccaddress1 = { a.FunctionalMgr, a.HSEMgrEmail };
                        string[] ccaddress = ccaddress1.Distinct().ToArray();
                        foreach (string cc in ccaddress)
                        {
                            if (!string.IsNullOrEmpty(cc))
                            {
                                CCaddressbuild.Append(cc);
                                CCaddressbuild.Append(',');
                            }
                        }
                        if (CCaddressbuild.Length > 0)
                        {
                            CCaddressbuild.Length--;
                        }

                        monitProEmail.ToAddress = a.ActionerEmail;
                        if (monitProEmail.ToAddress != "")
                        {
                            monitProEmail.CC = CCaddressbuild.ToString();
                        }
                        else
                        {
                            monitProEmail.ToAddress = CCaddressbuild.ToString();
                        }
                    }

                    monitProEmail.Subject = " CAPA Bulk Observation Mail  for "+capaID;
                    monitProEmail.Body = mailBody.ToString();
                                                    //monitProEmail.ToAddress = "safetrac.monitpro@gmail.com";      // <--uncomment this for test
                                                    //monitProEmail.CC = "";


                    Mailer.SendCAPAEmail(monitProEmail);

                return Json(new { success = true, message = "Email sent successfully." },JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Error sending email: " + ex.Message },JsonRequestBehavior.AllowGet);
                }
            }

        private string BuildMailBody(List<ObservationViewModelCapa> capaData)
        {
            var body = "<html><body>";
            body += "<h2 style='font-family: Arial, sans-serif; color: #333;'>Dear Sir,</h2>";
            body += "<p style='font-family: Arial, sans-serif; color: #333;'>CAPA recommendation is due for action. After completing the action, please log in to the SAFETRAC application, fill the details, and complete the action.</p>";

            // Table with styling for borders and neat design
            body += "<table style='border-collapse: collapse; width: 100%; margin-top: 20px;'>";
            body += "<tr style='background-color: #f2f2f2;'>";
            body += "<th style='border: 1px solid #ddd; padding: 8px; text-align: left; font-family: Arial, sans-serif; color: #333;'>Capa ID</th>";
            body += "<th style='border: 1px solid #ddd; padding: 8px; text-align: left; font-family: Arial, sans-serif; color: #333;'>Status</th>";
            body += "<th style='border: 1px solid #ddd; padding: 8px; text-align: left; font-family: Arial, sans-serif; color: #333;'>Target Date</th>";
            body += "<th style='border: 1px solid #ddd; padding: 8px; text-align: left; font-family: Arial, sans-serif; color: #333;'>Source</th>";
            body += "<th style='border: 1px solid #ddd; padding: 8px; text-align: left; font-family: Arial, sans-serif; color: #333;'>Recom ID</th>";
            body += "<th style='border: 1px solid #ddd; padding: 8px; text-align: left; font-family: Arial, sans-serif; color: #333;'>Recommendation</th>";
            body += "<th style='border: 1px solid #ddd; padding: 8px; text-align: left; font-family: Arial, sans-serif; color: #333;'>Priority</th>";
            body += "</tr>";

            foreach (var a in capaData)
            {
                // Add CAPA data to the table with alternating row colors for readability
                body += "<tr style='background-color: #ffffff;'>";
                body += "<td style='border: 1px solid #ddd; padding: 8px; text-align: left; font-family: Arial, sans-serif; color: #333;'>" + a.CAPAID + "</td>";
                body += "<td style='border: 1px solid #ddd; padding: 8px; text-align: left; font-family: Arial, sans-serif; color: #333;'>" + a.CurrentStatus + "</td>";
                body += "<td style='border: 1px solid #ddd; padding: 8px; text-align: left; font-family: Arial, sans-serif; color: #333;'>" + a.TargetDate + "</td>";
                body += "<td style='border: 1px solid #ddd; padding: 8px; text-align: left; font-family: Arial, sans-serif; color: #333;'>" + a.CAPASourceName + "</td>";
                body += "<td style='border: 1px solid #ddd; padding: 8px; text-align: left; font-family: Arial, sans-serif; color: #333;'>" + a.ObservationID + "</td>";
                body += "<td style='border: 1px solid #ddd; padding: 8px; text-align: left; font-family: Arial, sans-serif; color: #333;'>" + a.Recommendation + "</td>";
                body += "<td style='border: 1px solid #ddd; padding: 8px; text-align: left; font-family: Arial, sans-serif; color: #333;'>" + a.PriorityName + "</td>";
                body += "</tr>";
            }

            body += "</table>";
            var link = System.Configuration.ConfigurationManager.AppSettings["Link"];
            body += "<br><a href="+link+" style='font-family: Arial, sans-serif; color: #007bff;'>Click here to login and complete the details</a><br><br>";
            body += "<br><br>Regards,<br/>CAPA Advisor<br/>Note: This is a system generated email.";
            body += "</body></html>";

            return body;
        }





        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetAuditCAPAsource(int? AuditID)
        {

            var cs = CapaBLL.GetAuditCAPAsource(AuditID).Select(m => new SelectListItem()
            {
                Value = m.AuditCSID.ToString(),
                Text = m.Name
            });
            return Json(cs, JsonRequestBehavior.AllowGet);
        }

       //[AcceptVerbs(HttpVerbs.Get)]
        [HttpPost]
        public JsonResult UpdateTargetDate(int ObservationId, DateTime TargetDate1, DateTime TargetDate2, string Comments)
        {
            try
            {
                // Add your logic to update the database with the provided parameters
                // Example:
                // bool isUpdated = _service.UpdateObservation(ObservationId, TargetDate1, TargetDate2, Comments);
                bool isUpdated = true; // Simulate success for now

                if (isUpdated)
                {
                    return Json(new { success = true, message = "Update successful." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "Update failed. Please try again." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine("Error: " + ex.Message);
                return Json(new { success = false, message = "An error occurred. Please try again later." }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CAPAIndex()
        {
            CapaList1.CurrentSessionID = CurrentUser.CurrentSessionID;
            CapaList1.PrevoiusSessionID = sess.SessionActive;
            if (CapaList1.CurrentSessionID == CapaList1.PrevoiusSessionID)
            {

            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";

            }
            CapaList1.CapaList = CapaBLL.GetOpenCapa();
            CapaList1.CurrentUser = CurrentUser.UserID;
            CapaList1.statusList = CapaBLL.GetStatus();
            var status = CapaBLL.GetStatus();
            var ActionStatus = status.Where(y => y.ID < 3).ToList();
            CapaList1.statusList = ActionStatus;
            CapaList1.actionermodel = CapaBLL.GetAllCAPAObservations();

            ViewBag.AuditType = new SelectList(AuditType, "ID", "Name");
            ViewBag.CAPAPlants = new SelectList(capaPlants, "ID", "Name");
            ViewBag.CAPASource = new SelectList(CAPASource, "ID", "Name");

            CAPASearchViewModel capasearchviewmodel = new CAPASearchViewModel();
            ViewBag.fromdate = capasearchviewmodel.CAPAFromDate;
            ViewBag.Todate = capasearchviewmodel.CAPAToDate;
            ViewBag.PlantID = capasearchviewmodel.CAPAPlant;
            ViewBag.Audit = capasearchviewmodel.AuditType;
            ViewBag.Status = capasearchviewmodel.CAPAStatus;
            ViewBag.Source = capasearchviewmodel.CAPASource;

            CapaList1.Roles = CurrentUser.Roles;
            CapaList1.UserFullName = CurrentUser.FullName;
            CapaList1.ProfileImage = CurrentUser.ProfileImage;
            CapaList1.IsRestrict = CurrentUser.IsRestrict;
            return View(CapaList1);
        }

        [HttpPost]
        public ActionResult CAPAIndex(CAPASearchViewModel capasearchviewmodel)
        {
            List<CAPAViewModel> capalist = new List<CAPAViewModel>();
            CapaList1.CapaList = CapaBLL.SearchOpenCapa(capasearchviewmodel);
            CapaList1.actionermodel = CapaBLL.GetAllCAPAObservations();
            CapaList1.CurrentUser = CurrentUser.UserID;

            ViewBag.fromdate = capasearchviewmodel.CAPAFromDate;
            ViewBag.Todate = capasearchviewmodel.CAPAToDate;
            ViewBag.PlantID = capasearchviewmodel.CAPAPlant;
            ViewBag.Audit = capasearchviewmodel.AuditType;
            ViewBag.Status = capasearchviewmodel.CAPAStatus;
            ViewBag.Source = capasearchviewmodel.CAPASource;
            CapaList1.CAPASearch = capasearchviewmodel;
            CapaList1.statusList = CapaBLL.GetStatus();
            var status = CapaBLL.GetStatus();
            var ActionStatus = status.Where(y => y.ID < 3).ToList();
            CapaList1.statusList = ActionStatus;

            CapaList1.Roles = CurrentUser.Roles;
            CapaList1.UserFullName = CurrentUser.FullName;
            CapaList1.ProfileImage = CurrentUser.ProfileImage;
            CapaList1.IsRestrict = CurrentUser.IsRestrict;
            ViewBag.AuditType = new SelectList(AuditType, "ID", "Name");
            ViewBag.CAPAPlants = new SelectList(capaPlants, "ID", "Name");
            ViewBag.CAPASource = new SelectList(CAPASource, "ID", "Name");


            return PartialView(CapaList1);
        }
        public ActionResult ExportCapaList(string currentFromDate, string currentEndDate, int capasourceID, int IncidentPlantID, int audittype, int capastatusID)
        {


            using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "ExportCAPAList";
                objCom.CommandType = CommandType.StoredProcedure;
                objCom.Parameters.AddWithValue("@FromDate", currentFromDate == null ? string.Empty : currentFromDate);
                objCom.Parameters.AddWithValue("@ToDate", currentEndDate == null ? string.Empty : currentEndDate);
                objCom.Parameters.AddWithValue("@AuditType ", audittype);
                objCom.Parameters.AddWithValue("@CAPASource", capasourceID);
                objCom.Parameters.AddWithValue("@CAPAPlant ", IncidentPlantID);
                objCom.Parameters.AddWithValue("@CAPAStatus ", capastatusID);

                objCon.Open();
                objCom.Connection = objCon;
                SqlDataAdapter Adapter = new SqlDataAdapter();
                Adapter.SelectCommand = objCom;
                DataSet dataSet = new DataSet();
                Adapter.Fill(dataSet);
                objCon.Close();
                var wb = new XLWorkbook(Server.MapPath("~/Template/Incident.xlsx"));
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
                Response.AddHeader("content-disposition", "attachment;filename=IncidentManagementList.xlsx");
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
        public ActionResult CAPAHistory()
        {
            CapaList1.CurrentSessionID = CurrentUser.CurrentSessionID;
            CapaList1.PrevoiusSessionID = sess.SessionActive;
            if (CapaList1.CurrentSessionID == CapaList1.PrevoiusSessionID)
            {

            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";

            }
            CapaList1.Roles = CurrentUser.Roles;
            CapaList1.UserFullName = CurrentUser.FullName;
            CapaList1.ProfileImage = CurrentUser.ProfileImage;
            CapaList1.IsRestrict = CurrentUser.IsRestrict;
            CapaList1.statusList = CAPAStatus;
            ViewBag.AuditType = new SelectList(AuditType, "ID", "Name");
            ViewBag.CAPAPlants = new SelectList(capaPlants, "ID", "Name");
            ViewBag.CAPASources = new SelectList(CAPASource, "ID", "Name");
            CAPASearchViewModel capasearchviewmodel = new CAPASearchViewModel();
            ViewBag.fromdate = capasearchviewmodel.CAPAFromDate;
            ViewBag.Todate = capasearchviewmodel.CAPAToDate;
            ViewBag.PlantID = capasearchviewmodel.CAPAPlant;
            ViewBag.CAPAAudit = capasearchviewmodel.AuditType;
            ViewBag.CapaSource = capasearchviewmodel.CAPASource;
            CapaList1.CapaList = CapaBLL.GetAllClosedCapa();
            return View(CapaList1);
        }

        [HttpPost]
        public ActionResult CAPAHistory(CAPASearchViewModel capasearchviewmodel)
        {
            capasearchviewmodel.CAPAStatus = 3;
            CapaList1.statusList = CAPAStatus;
            ViewBag.AuditType = new SelectList(AuditType, "ID", "Name");
            ViewBag.CAPAPlants = new SelectList(capaPlants, "ID", "Name");
            ViewBag.CAPASources = new SelectList(CAPASource, "ID", "Name");

            ViewBag.fromdate = capasearchviewmodel.CAPAFromDate;
            ViewBag.Todate = capasearchviewmodel.CAPAToDate;
            ViewBag.PlantID = capasearchviewmodel.CAPAPlant;
            ViewBag.CAPAAudit = capasearchviewmodel.AuditType;
            ViewBag.CapaSource = capasearchviewmodel.CAPASource;
            CapaList1.Roles = CurrentUser.Roles;
            CapaList1.UserFullName = CurrentUser.FullName;
            CapaList1.ProfileImage = CurrentUser.ProfileImage;
            CapaList1.IsRestrict = CurrentUser.IsRestrict;
            CapaList1.CapaList = CapaBLL.SearchOpenCapa(capasearchviewmodel);

            return View(CapaList1);
        }
        public ActionResult ExportCAPAHistory(string currentFromDate, string currentEndDate, int currentPlantID, int currentAuditType, int currentCAPASource)
        {


            using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "ExportCAPAHistory";
                objCom.CommandType = CommandType.StoredProcedure;
                objCom.Parameters.AddWithValue("@FromDate", currentFromDate == null ? string.Empty : currentFromDate);
                objCom.Parameters.AddWithValue("@ToDate", currentEndDate == null ? string.Empty : currentEndDate);
                objCom.Parameters.AddWithValue("@AuditType", currentAuditType);
                objCom.Parameters.AddWithValue("@CAPAPlant", currentPlantID);
                objCom.Parameters.AddWithValue("@CAPASource", currentCAPASource);

                objCon.Open();
                objCom.Connection = objCon;
                SqlDataAdapter Adapter = new SqlDataAdapter();
                Adapter.SelectCommand = objCom;
                DataSet dataSet = new DataSet();
                Adapter.Fill(dataSet);
                objCon.Close();
                var wb = new XLWorkbook(Server.MapPath("~/Template/CAPAHistory.xlsx"));
                var worksheet = wb.Worksheet("CAPA List");
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
                Response.AddHeader("content-disposition", "attachment;filename= ClosedCAPAHistoryRecord.xlsx");
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
        public ActionResult CAPAUpdateStatus(int capaID)
        {

            NewCAPAModel newcapa = new NewCAPAModel();
            CreateCAPA createcapa = new CreateCAPA();

            createcapa = CapaBLL.GetCAPADetails(capaID);
            newcapa.statusList = CAPAStatus;
            newcapa.CreateCAPA = createcapa;

            return View(newcapa);

        }

        [HttpPost]
        public ActionResult UpdateCAPAStatus(int capaID, int StatusID, string Comments)
        {
            string strMessage = String.Empty;
            try
            {
                CapaBLL.UpdateCAPAStatus(capaID, StatusID, Comments, CurrentUser.UserID);
                strMessage = "Data Saved Successfully";
            }
            catch (Exception ex)
            {
                strMessage = ex.Message;
            }
            return Json(new { strMessage });
        }
       
        public ActionResult CAPAObservations(int capaID = 0, int OBID = 0)
        {
            CAPAObservationViewModel insObservationVM = new CAPAObservationViewModel();
            insObservationVM.CurrentSessionID = CurrentUser.CurrentSessionID;
            insObservationVM.PrevoiusSessionID = sess.SessionActive;
            if (insObservationVM.CurrentSessionID == insObservationVM.PrevoiusSessionID)
            {

            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";

            }
            List<ObservationViewModelCapa> obsVM = new List<ObservationViewModelCapa>();

            CAPAObservation insObservation = new CAPAObservation();

            CreateCAPA createcapa = new CreateCAPA();
            createcapa = CapaBLL.GetCAPADetails(capaID);

            var plantsList = IncidentPlants.Where(y => y.ID != -1).ToList();

            var dept = CapaBLL.GetAllManager();
            //var DeptManager = dept.Where(y => y.ID != 0).ToList();

            var action = CapaBLL.GetActionList();
            // Action = action.Where(y => y.UserID != 0).ToList();

            var cppriority = CAPAPriority.Where(y => y.ID != -1).ToList();

            obsVM = CapaBLL.GetObservations(capaID,OBID);

            var status = CapaBLL.GetCAPAObservationStatus();
            var capstatus = status.Where(y => (y.ID == -1 || y.ID == 4 || y.ID == 1)).ToList();

            insObservation = CapaBLL.GetCAPAPlantObservations(capaID);
            var capacategory = CAPACategory.Where(y => y.ID != -1).ToList();
            insObservationVM.Roles = CurrentUser.Roles;
            foreach (var item in insObservationVM.Roles)
            {
                insObservationVM.RoleId = item.RoleID;
            }
            insObservationVM.UserFullName = CurrentUser.FullName;
            insObservationVM.ProfileImage = CurrentUser.ProfileImage;

            insObservationVM.CurrentUser = CurrentUser.UserID;
            insObservationVM.IsRestrict =  CurrentUser.IsRestrict;
            insObservation.ActionList = action;

            //insObservation.CAPAID = capaID;
            insObservation.TargetDate = DateTime.Today.AddDays(30).ToString("dd/MM/yyyy"); 
            insObservation.CompletedDate = null;
            insObservation.ExtendedTargetDate= DateTime.Today.AddDays(0).ToString("dd/MM/yyyy");
            insObservationVM.observationstatuslist = capstatus;
            insObservationVM.ObservationViewModelList1 = obsVM;
            insObservationVM.plantlist = plantsList;
            insObservationVM.capacategory = capacategory;
            insObservationVM.capapriority = cppriority;
            insObservationVM.DeptManagerList = dept;
            insObservationVM.CAPAObservation = insObservation;
            insObservationVM.createCapa = createcapa;
            foreach (var i in insObservationVM.ObservationViewModelList1)
            {
                if (CurrentUser.UserID == i.CompletedBy)
                {
                    insObservationVM.CompletedBy = i.CompletedBy;
                }
                if (CurrentUser.UserID == i.DptID)
                {
                    insObservationVM.DeptID = i.DptID;
                }

            }

            return PartialView(insObservationVM);
        }
       

        public ActionResult SaveCAPAObservations(CAPAObservation cpObservation)
        {
            cpObservation.CurrentUser = CurrentUser.UserID;
            if (Request.Files.Count > 0)
            {
                HttpFileCollectionBase files = Request.Files;

                cpObservation.CAPAObAttachment = files[0];
                string fileName = cpObservation.CAPAObAttachment.FileName;

                // create the uploads folder if it doesn't exist

                string path = Path.Combine(Server.MapPath("~/CAPAObservation/"), fileName);

                // save the file
                cpObservation.CAPAObAttachment.SaveAs(path);

                CapaBLL.SaveCAPAObservation(cpObservation);
            }
            else
            {
                CapaBLL.SaveCAPAObservation(cpObservation);
            }
            return View();

        }
        [HttpPost]
        public ActionResult DeleteOBImage(int obid)
        {

            CapaBLL.DeleteOBImage(obid);
            string strMessage = "Deleted Successfully";
            return Json(new { strMessage });
        }

        [HttpPost]
        public ActionResult EditCAPAObservation(int observationID)
        {
            cpObservationViewModel cpobservation = new cpObservationViewModel();

            cpobservation = CapaBLL.EditCAPAObservation(observationID);

            if (cpobservation.capaobs.CompletedBy == CurrentUser.UserID)
            {
                cpobservation.capaobs.CompletedDate = DateTime.Now.ToString("dd/MM/yyyy");
            }
            cpobservation.capaobs.ExtendedTargetDate = DateTime.Now.ToString("dd/MM/yyyy");

            return Json(new { cpobservation });
            //return View(insObservation);
        }

        [HttpPost]
        public ActionResult DeleteCAPAObservation(int observationID)
        {
            string strMessage = String.Empty;
            try
            {
                CapaBLL.DeleteCAPAObservation(observationID);
                strMessage = "Successfully Deleted";
            }
            catch (Exception ex)
            {
                strMessage = ex.Message;
            }

            return Json(new { strMessage });
        }

        public ActionResult PlantOwner()
        {
            CAPAObservationViewModel insObservationVM = new CAPAObservationViewModel();
            insObservationVM.CurrentSessionID = CurrentUser.CurrentSessionID;
            insObservationVM.PrevoiusSessionID = sess.SessionActive;
            if (insObservationVM.CurrentSessionID == insObservationVM.PrevoiusSessionID)
            {

            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";

            }
            // List<ObservationViewModelCapa> obsVM = new List<ObservationViewModelCapa>();

            CAPAObservation insObservation = new CAPAObservation();

            var plantsList = IncidentPlants.Where(y => y.ID != -1).ToList();

            var dept = CapaBLL.GetAllManager();
            var DeptManager = dept.Where(y => y.ID != 0).ToList();

            insObservationVM.Roles = CurrentUser.Roles;
            foreach (var item in insObservationVM.Roles)
            {
                insObservationVM.RoleId = item.RoleID;
            }
            insObservationVM.UserFullName = CurrentUser.FullName;
            insObservationVM.ProfileImage = CurrentUser.ProfileImage;
           
            insObservationVM.CurrentUser = CurrentUser.UserID;
            
            //insObservation.ActionList = Action;
            //insObservationVM.Designation = CurrentUser.Designation;
            //insObservationVM.factorylist = CurrentUser.FactoryLists;
            //insObservation.CAPAID = capaID;
            //insObservation.TargetDate = DateTime.Today.AddDays(30).ToString("dd/MM/yyyy"); 
            insObservation.CompletedDate = null;

            //insObservationVM.observationstatuslist = capstatus;
            //insObservationVM.ObservationViewModelList1 = obsVM;
            insObservationVM.plantlist = plantsList;
            //insObservationVM.capacategory = capacategory;
            //insObservationVM.capapriority = cppriority;
            insObservationVM.DeptManagerList = DeptManager;
            insObservationVM.CAPAObservation = insObservation;
            //insObservationVM.createCapa = createcapa;
            //foreach (var i in insObservationVM.ObservationViewModelList1)
            //{
            //    if (CurrentUser.UserID == i.CompletedBy)
            //    {
            //        insObservationVM.CompletedBy = i.CompletedBy;
            //    }
            //    if (CurrentUser.UserID == i.DptID)
            //    {
            //        insObservationVM.DeptID = i.DptID;
            //    }

            //}
            //return PartialView(insObservationVM);
            return View(insObservationVM);
        }

        [HttpPost]
        public ActionResult SavePlantOwner(List<PlantOwnerModel> PlantOwners)
        {
            //foreach (var plantOwner in PlantOwners)
            //{
            //    // Process each PlantID and DeptManagerID
            //    var plantId = plantOwner.PlantID;
            //    var deptManagerId = plantOwner.DeptManagerID;

            //    // Save to database or perform any other operation
            //}
            var plantIds = string.Join(",", PlantOwners.Select(po => po.PlantID.ToString()));

            var deptManagerIds = string.Join(",", PlantOwners.Select(po => po.DeptManagerID.ToString()));
            CapaBLL.UpdateAreaOwnerBLL(plantIds, deptManagerIds);
            // Redirect or return a view
            return RedirectToAction("PlantOwner");
        }


        public ActionResult ActionStatusChart()
        {
            ActionsStatusChartViewModel actionstatusChartViewModel = new ActionsStatusChartViewModel()
            {
                Roles = UserRoles,
                ActionCounts = CapaBLL.GetActionStatusCount()
            };

            return View(actionstatusChartViewModel);
        }

        public ActionResult CapaSourceCount()
        {
            ActionsStatusChartViewModel capasourcecount = new ActionsStatusChartViewModel
            {
                Roles = UserRoles,
                CapaSourceCount = CapaBLL.GetCapaSourceCount()
            };

            return View(capasourcecount);
        }

        public ActionResult ObservationCount()
        {
            ActionsStatusChartViewModel observationcount = new ActionsStatusChartViewModel
            {
                Roles = UserRoles,
              observationcount= CapaBLL.GetObservationCount()
            };

            return View(observationcount);
        }

        public ActionResult CategoryCount()
        {
            ActionsStatusChartViewModel categorycount= new ActionsStatusChartViewModel
            {
                Roles = UserRoles,
               categorycount = CapaBLL.GetCategoryCount()
            };

            return View(categorycount);
        }



        public ActionResult CapaPriorityCount()
        {
            ActionsStatusChartViewModel priority = new ActionsStatusChartViewModel
            {
                Roles = UserRoles,
               prioritycount = CapaBLL.GetCapaPriorityCount()
            };

            return View(priority);
        }
        //Get: last month observationCount
        public ActionResult LastMonthObservationCount()
        {
            ActionsStatusChartViewModel observationcount = new ActionsStatusChartViewModel
            {
                Roles = UserRoles,
                observationcount = CapaBLL.GetLastMonthObservationCount()
            };
            return View(observationcount);
        }

        //Get: last month CapaSourceCount
        public ActionResult LastMonthCapaSourceCount()
        {
            ActionsStatusChartViewModel capasourcecount = new ActionsStatusChartViewModel
            {
                Roles = UserRoles,
                CapaSourceCount = CapaBLL.GetLastMonthCapaSourceCount()
            };

            return View(capasourcecount);
        }

        //Get: last month CategoryCount
        public ActionResult LastMonthCategoryCount()
        {
            ActionsStatusChartViewModel categorycount = new ActionsStatusChartViewModel
            {
                Roles = UserRoles,
                categorycount = CapaBLL.GetLastMonthCategoryCount()
            };

            return View(categorycount);
        }

        //Get: last month ActionStatusChart
        public ActionResult LastMonthActionStatusChart()
        {
            ActionsStatusChartViewModel actionstatusChartViewModel = new ActionsStatusChartViewModel()
            {
                Roles = UserRoles,
                ActionCounts = CapaBLL.GetLastMonthActionStatusCount()
            };

            return View(actionstatusChartViewModel);
        }

        //Get: last month CapaPriorityCount
        public ActionResult LastMonthCapaPriorityCount()
        {
            ActionsStatusChartViewModel priority = new ActionsStatusChartViewModel
            {
                Roles = UserRoles,
                prioritycount = CapaBLL.GetLastMonthCapaPriorityCount()
            };

            return View(priority);
        }


        public ActionResult AllCAPAObservation()
        {
            AllCAPAObservationListModel allcapaobservationlistmodel = new AllCAPAObservationListModel();
            allcapaobservationlistmodel.CurrentSessionID = CurrentUser.CurrentSessionID;
            allcapaobservationlistmodel.PrevoiusSessionID = sess.SessionActive;
            if (allcapaobservationlistmodel.CurrentSessionID == allcapaobservationlistmodel.PrevoiusSessionID)
            {

            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";

            }
            List<ObservationViewModelCapa> obsVM = new List<ObservationViewModelCapa>();

            List<Employee> DeptManager = CapaBLL.GetAllManager();

            var trackStatus = capaobservationstatus.Where(y => y.ID != 5).ToList();
            allcapaobservationlistmodel.observationstatuslist = trackStatus;

            ViewBag.AuditType = new SelectList(AuditType, "ID", "Name");
            ViewBag.IncidentPlant = new SelectList(IncidentPlants, "ID", "Name");
            ViewBag.CAPASources = new SelectList(CAPASource, "ID", "Name");
            ViewBag.UserList = new SelectList(UserProfiles, "UserID", "DisplayUserName");
            allcapaobservationlistmodel.capapriority = CAPAPriority;
            allcapaobservationlistmodel.DeptManagerList = DeptManager;
            allcapaobservationlistmodel.Roles = CurrentUser.Roles;
            allcapaobservationlistmodel.UserFullName = CurrentUser.FullName;
            allcapaobservationlistmodel.ProfileImage = CurrentUser.ProfileImage;
            allcapaobservationlistmodel.capacategory = CAPACategory;
            allcapaobservationlistmodel.CurrentUser = CurrentUser.UserID;
            allcapaobservationlistmodel.IsRestrict = CurrentUser.IsRestrict;
            obsVM = CapaBLL.GetAllCAPAObservation();
            allcapaobservationlistmodel.ObservationViewModelList1 = obsVM;
            CAPASearchViewModel capasearchviewmodel = new CAPASearchViewModel();
            ViewBag.fromdate = capasearchviewmodel.CAPAFromDate;
            ViewBag.Todate = capasearchviewmodel.CAPAToDate;
            ViewBag.PlantID = capasearchviewmodel.CAPAPlant;
            ViewBag.DptManager = capasearchviewmodel.DeptManager;
            ViewBag.Category = capasearchviewmodel.CategoryID;
            ViewBag.Priority = capasearchviewmodel.PriorityID;
            ViewBag.Actioner = capasearchviewmodel.ActionerID;
            ViewBag.CapaSource = capasearchviewmodel.CAPASource;
            ViewBag.CAPAStatus = capasearchviewmodel.CAPAStatus;

            return View(allcapaobservationlistmodel);

        }

        [HttpPost]
        public ActionResult AllCAPAObservation(CAPASearchViewModel capasearchviewmodel)
        {
            AllCAPAObservationListModel allcapaobservationlistmodel = new AllCAPAObservationListModel();
            List<CAPAViewModel> capalist = new List<CAPAViewModel>();
            List<Employee> DeptManager = CapaBLL.GetAllManager();
            allcapaobservationlistmodel.ObservationViewModelList1 = CapaBLL.SearchOpenCapaForObservation(capasearchviewmodel);
            allcapaobservationlistmodel.capapriority = CAPAPriority;
            allcapaobservationlistmodel.CurrentUser = CurrentUser.UserID;
            allcapaobservationlistmodel.CAPASearch = capasearchviewmodel;
            var trackStatus = capaobservationstatus.Where(y => y.ID != 5).ToList();
            allcapaobservationlistmodel.observationstatuslist = trackStatus;
            allcapaobservationlistmodel.DeptManagerList = DeptManager;
            allcapaobservationlistmodel.Roles = CurrentUser.Roles;
            allcapaobservationlistmodel.UserFullName = CurrentUser.FullName;
            allcapaobservationlistmodel.ProfileImage = CurrentUser.ProfileImage;
            allcapaobservationlistmodel.IsRestrict = CurrentUser.IsRestrict;
            allcapaobservationlistmodel.capacategory = CAPACategory;
            ViewBag.AuditType = new SelectList(AuditType, "ID", "Name");
            ViewBag.IncidentPlant = new SelectList(IncidentPlants, "ID", "Name");
            ViewBag.CAPASources = new SelectList(CAPASource, "ID", "Name");
            ViewBag.UserList = new SelectList(UserProfiles, "UserID", "DisplayUserName");
            ViewBag.fromdate = capasearchviewmodel.CAPAFromDate;
            ViewBag.Todate = capasearchviewmodel.CAPAToDate;
            ViewBag.PlantID = capasearchviewmodel.CAPAPlant;
            ViewBag.DptManager = capasearchviewmodel.DeptManager;
            ViewBag.Category = capasearchviewmodel.CategoryID;
            ViewBag.Priority = capasearchviewmodel.PriorityID;
            ViewBag.Actioner = capasearchviewmodel.ActionerID;
            ViewBag.CapaSource = capasearchviewmodel.CAPASource;
            ViewBag.CAPAStatus = capasearchviewmodel.CAPAStatus;
            return View(allcapaobservationlistmodel);
        }

        public ActionResult ExportAllCapaObservation(string CAPAFromDate, string CAPAToDate, int CAPAPlant, int DeptManager, int CategoryID, int ActionerID, int PriorityID, int CAPAStatus, int CAPASource)
        {


            using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "ExportAllCAPA";
                objCom.CommandType = CommandType.StoredProcedure;
                objCom.Parameters.AddWithValue("@CAPAPlant", CAPAPlant);
                objCom.Parameters.AddWithValue("@FromDate", CAPAFromDate == null ? string.Empty : CAPAFromDate);
                objCom.Parameters.AddWithValue("@ToDate", CAPAToDate == null ? string.Empty : CAPAToDate);
                objCom.Parameters.AddWithValue("@DptManager", DeptManager);
                objCom.Parameters.AddWithValue("@Category", CategoryID);
                objCom.Parameters.AddWithValue("@ActionBy", ActionerID);
                objCom.Parameters.AddWithValue("@Priority", PriorityID);
                objCom.Parameters.AddWithValue("@CAPAStatus", CAPAStatus);
                objCom.Parameters.AddWithValue("@CAPASource", CAPASource);

                objCon.Open();
                objCom.Connection = objCon;
                SqlDataAdapter Adapter = new SqlDataAdapter();
                Adapter.SelectCommand = objCom;
                DataSet dataSet = new DataSet();
                Adapter.Fill(dataSet);
                objCon.Close();
                var wb = new XLWorkbook(Server.MapPath("~/Template/TrackActionsCAPA.xlsx"));
                var worksheet = wb.Worksheet("CAPA observations");
                worksheet.Cell("C4").Value = "Report Generated by : " + CurrentUser.FullName;
                //worksheet.Cell("D4").Value = CurrentUser.FullName;
                worksheet.Cell("C5").Value = "Report Duration : " + CAPAFromDate + " to  " + CAPAToDate;
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
                Response.AddHeader("content-disposition", "attachment;filename=TrackActions.xlsx");
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


        public ActionResult MyActionStatus()
        {
            AllCAPAObservationListModel MyActionStatusmodel = new AllCAPAObservationListModel();
           MyactionDashboardCount myactionDashboardCount = new MyactionDashboardCount();
            MyApprovalCount myApprovalCount = new MyApprovalCount();
            MyActionStatusmodel.CurrentSessionID = CurrentUser.CurrentSessionID;
            MyActionStatusmodel.PrevoiusSessionID = sess.SessionActive;
            if (MyActionStatusmodel.CurrentSessionID == MyActionStatusmodel.PrevoiusSessionID)
            {

            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";

            }
            List<ObservationViewModelCapa> obsVM = new List<ObservationViewModelCapa>();
            List<ActionsCount> actionlist = new List<ActionsCount>();
            actionlist = CapaBLL.GetMyActionStatusCount(CurrentUser.UserID);
            obsVM = CapaBLL.GetMyActionStatus(CurrentUser.UserID);
            myactionDashboardCount = CapaBLL.GetDashboardOverallCount();
            myApprovalCount = CapaBLL.GetDashboardApprovalCount(CurrentUser.UserID);
            MyActionStatusmodel.GetDashboardOVerallStatusCount = myactionDashboardCount;
            MyActionStatusmodel.GetMyApprovalCount = myApprovalCount;
            MyActionStatusmodel.observationstatuslist = capaobservationstatus;
            MyActionStatusmodel.Roles = CurrentUser.Roles;
            MyActionStatusmodel.UserFullName = CurrentUser.FullName;
            MyActionStatusmodel.ProfileImage = CurrentUser.ProfileImage;
            MyActionStatusmodel.IsRestrict = CurrentUser.IsRestrict;
            MyActionStatusmodel.CurrentUser = CurrentUser.UserID;
            MyActionStatusmodel.ObservationViewModelList1 = obsVM;
            MyActionStatusmodel.ActionCounts = actionlist;

            return View(MyActionStatusmodel);

        }
        public ActionResult CAPAPrintPDF(int id)
        {
            try
            {
                using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[CAPAPDF]";
                    objCom.Parameters.AddWithValue("@CAPAID", id);
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCon.Open();
                    objCom.Connection = objCon;
                    SqlDataAdapter Adapter = new SqlDataAdapter();
                    Adapter.SelectCommand = objCom;
                    DataSet dataSet = new DataSet();
                    Adapter.Fill(dataSet);
                    objCon.Close();

                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition",
                        "filename=CAPAPDF.pdf");
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);

                    iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate());
                   // pdfDoc.SetPageSize(PageSize.A4.Rotate());
                    HTMLWorker htmlparsers = new HTMLWorker(pdfDoc);
                    PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                    pdfDoc.Open();
                    PdfPCell cell = null;


                    PdfPTable TitleTable = new PdfPTable(3);
                    TitleTable.LockedWidth = true;
                    TitleTable.SetWidths(new float[] { 8f, 14f, 8f });
                    //TitleTable.SpacingBefore = 10f;
                    //TitleTable.SpacingAfter = 1f;
                    TitleTable.TotalWidth = 760f;
                    PdfPCell Wpcell = new PdfPCell();
                    string imageURL = Server.MapPath("~/Images/SASALogo.png");
                    iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(imageURL);
                    gif.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
                    gif.SpacingBefore = 10f;

                    gif.ScaleAbsolute(140f, 45f);
                    Wpcell = new PdfPCell(gif);

                    TitleTable.AddCell(Wpcell);

                    var phrase = new Phrase(new Chunk("\n AUDIT OBSERVATIONS/CAPA REPORT" + "\n\n", FontFactory.GetFont("Times New Roman", 14, iTextSharp.text.Font.BOLD)));

                    TitleTable.AddCell(PhraseCell(phrase, PdfPCell.ALIGN_CENTER));

                    Wpcell = new PdfPCell(new Phrase("\n CAPA ID:".PadRight(5) + dataSet.Tables[0].Rows[0][10].ToString(), FontFactory.GetFont("Times New Roman", 13, iTextSharp.text.Font.BOLD)));
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

                    PdfPTable Details = new PdfPTable(1);
                    Details.TotalWidth = 760f;
                    Details.LockedWidth = true;
                    Details.SetWidths(new float[] { 30f });
                    Details.AddCell(PhraseCell(new Phrase("CAPA Details", FontFactory.GetFont("Times New Roman", 12, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_CENTER));

                    pdfDoc.Add(Details);

                    PdfPTable tablesender = new PdfPTable(5);
                    tablesender.TotalWidth = 760f;
                    tablesender.LockedWidth = true;
                    tablesender.SetWidths(new float[] { 1f, 6.5f, 13.5f,6.5f,13.5f });


                    tablesender.AddCell(PhraseCell(new Phrase("1.", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("Type Of Audit", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][2].ToString(), FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("CAPA Source", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][3].ToString(), FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    tablesender.AddCell(PhraseCell(new Phrase("2.", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("Audit Date", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][4].ToString(), FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("Reported by", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][9], FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    tablesender.AddCell(PhraseCell(new Phrase("3.", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("Remarks", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][5].ToString(), FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("Created by", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][8].ToString(), FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));


                    tablesender.AddCell(PhraseCell(new Phrase("4.", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("Created Date", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][6].ToString(), FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("Status", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][7].ToString(), FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    pdfDoc.Add(tablesender);
                   // pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                    using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("[CapaObserverListPDf]", sqlcon))
                        {
                            cmd.Parameters.AddWithValue("@CAPAID", id);
                            cmd.CommandType = CommandType.StoredProcedure;

                            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                            {

                                DataSet ds = new DataSet();

                                da.Fill(ds);

                                pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));
                                PdfPTable ObserversD = new PdfPTable(2);
                                ObserversD.TotalWidth = 760f;
                                ObserversD.LockedWidth = true;
                                ObserversD.SetWidths(new float[] { 1.02f, 29f });
                                ObserversD.AddCell(PhraseCell(new Phrase("5.", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));

                                ObserversD.AddCell(PhraseCell(new Phrase("Observations/Recommendations Details", FontFactory.GetFont("Times New Roman", 12, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_CENTER));

                                pdfDoc.Add(ObserversD);

                                if (ds != null)
                                {
                                    //Craete instance of the pdf table and set the number of column in that table
                                    PdfPTable PdfTable = new PdfPTable(14);
                                    PdfTable.TotalWidth = 760f;
                                    PdfTable.LockedWidth = true;
                                    PdfTable.SetWidths(new float[] { 2f, 3f,5.5f, 5f, 7.2f, 3.9f, 3.4f, 4.5f , 3.3f, 4.5f, 3f, 4.2f, 3.8f, 3.7f });
                                    PdfPCell PdfPCell = null;
                                    var font8 = FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD);
                                    var font9 = FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL);

                                    //Add Header of the pdf table


                                    PdfPCell = new PdfPCell(new Phrase(new Chunk("S.No", FontFactory.GetFont("Times New Roman", 9, iTextSharp.text.Font.BOLD))));
                                    PdfTable.AddCell(PdfPCell);

                                    PdfPCell = new PdfPCell(new Phrase(new Chunk("Recom ID", font8)));
                                    PdfTable.AddCell(PdfPCell);

                                    PdfPCell = new PdfPCell(new Phrase(new Chunk("Plant/Area", font8)));
                                    PdfTable.AddCell(PdfPCell);

                                    PdfPCell = new PdfPCell(new Phrase(new Chunk("Observation/Description", font8)));
                                    PdfTable.AddCell(PdfPCell);

                                    PdfPCell = new PdfPCell(new Phrase(new Chunk("Recommendation", font8)));
                                    PdfTable.AddCell(PdfPCell);

                                    PdfPCell = new PdfPCell(new Phrase(new Chunk("Category", font8)));
                                    PdfTable.AddCell(PdfPCell);

                                    PdfPCell = new PdfPCell(new Phrase(new Chunk("Priority", font8)));
                                    PdfTable.AddCell(PdfPCell);
                                    PdfPCell = new PdfPCell(new Phrase(new Chunk("Functional Manager", font8)));
                                    PdfTable.AddCell(PdfPCell);
                                    PdfPCell = new PdfPCell(new Phrase(new Chunk("Action by", font8)));
                                    PdfTable.AddCell(PdfPCell);

                                    PdfPCell = new PdfPCell(new Phrase(new Chunk("Action Taken", font8)));
                                    PdfTable.AddCell(PdfPCell);

                                    
                                    
                                    PdfPCell = new PdfPCell(new Phrase(new Chunk("Target Date", font8)));
                                    PdfTable.AddCell(PdfPCell);

                                    PdfPCell = new PdfPCell(new Phrase(new Chunk("Completed Date", font8)));
                                    PdfTable.AddCell(PdfPCell);

                                    PdfPCell = new PdfPCell(new Phrase(new Chunk("Status", font8)));
                                    PdfTable.AddCell(PdfPCell);

                                    PdfPCell = new PdfPCell(new Phrase(new Chunk("Remarks", font8)));
                                    PdfTable.AddCell(PdfPCell);

                                    //How add the data from datatable to pdf table
                                    if (ds.Tables[0].Rows.Count > 0)
                                    {
                                        for (int rows = 0; rows < ds.Tables[0].Rows.Count; rows++)
                                        {
                                            for (int column = 0; column < ds.Tables[0].Columns.Count; column++)
                                            {
                                                PdfPCell = new PdfPCell(new Phrase(new Chunk((ds.Tables[0].Rows[rows][column].ToString() != "") ? ds.Tables[0].Rows[rows][column].ToString() : "Data Not Available", font9)));
                                                PdfPCell.SetLeading(5.0f, 1.0f);
                                                PdfTable.AddCell(PdfPCell);
                                            }
                                        }

                                    }
                                    else
                                    {
                                        for (int column = 0; column < ds.Tables[0].Columns.Count; column++)
                                        {
                                            PdfPCell = new PdfPCell(new Phrase(new Chunk("Data Not Available", font9)));
                                            PdfTable.AddCell(PdfPCell);
                                        }

                                    }
                                    pdfDoc.Add(PdfTable); // add pdf table to the document

                                }
                                pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                            }
                        }
                    }

                    PdfPTable img = new PdfPTable(2);
                    img.TotalWidth = 760f;
                    img.LockedWidth = true;
                    img.KeepTogether = true;
                    img.SetWidths(new float[] { 3f, 27f });
                    Wpcell = new PdfPCell(new Phrase(new Chunk("6.", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD))));
                    img.AddCell(Wpcell);
                  
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Picture Attachment", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD))));
                    img.AddCell(Wpcell);
                    pdfDoc.Add(img);
                    if (dataSet != null)
                    {
                        PdfPTable uploadimage = new PdfPTable(2);
                        uploadimage.TotalWidth = 760f;
                        uploadimage.LockedWidth = true;
                        uploadimage.KeepTogether = true;
                        uploadimage.SetWidths(new float[] { 3f, 27f });
                        var font8 = FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD);
                        var font9 = FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("Recom ID", font8)));
                        uploadimage.AddCell(Wpcell);


                        Wpcell = new PdfPCell(new Phrase(new Chunk("Image", font8)));
                        uploadimage.AddCell(Wpcell);
                        if (dataSet.Tables[2].Rows.Count > 0)
                        {
                            for (int rows = 0; rows < dataSet.Tables[2].Rows.Count; rows++)
                            {
                                Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[2].Rows[rows][0].ToString(), font9)));
                                uploadimage.AddCell(Wpcell);
                                if (dataSet.Tables[2].Rows[rows][1].ToString() != "")
                                {
                                    string imagePath = Server.MapPath("~/CAPAObservation/") + Path.GetFileName(dataSet.Tables[2].Rows[rows][1].ToString());

                                    iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imagePath);
                                    image.ScaleAbsolute(500f, 180f);
                                    Wpcell = new PdfPCell(image);
                                    Wpcell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;

                                    uploadimage.AddCell(Wpcell);

                                }

                                else
                                {

                                    Wpcell = new PdfPCell(new Phrase(new Chunk("Data Not Available", font9)));

                                    uploadimage.AddCell(Wpcell);

                                }
                            }
                        }

                        pdfDoc.Add(uploadimage);
                    }
                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                    using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("[CAPAPDF]", sqlcon))
                        {
                            cmd.Parameters.AddWithValue("@CAPAID", id);
                            cmd.CommandType = CommandType.StoredProcedure;

                            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                            {

                                PdfPTable Details1 = new PdfPTable(2);
                                Details1.TotalWidth = 760f;
                                Details1.LockedWidth = true;
                                Details1.KeepTogether = true;
                                Details1.SetWidths(new float[] { 1.45f, 29f });
                                Details1.AddCell(PhraseCell(new Phrase("7.", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                                Details1.AddCell(PhraseCell(new Phrase("Closure Comments", FontFactory.GetFont("Times New Roman", 12, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_CENTER));

                                pdfDoc.Add(Details1);
                                PdfPTable closer = new PdfPTable(3);
                                closer.TotalWidth = 760f;
                                closer.LockedWidth = true;
                                closer.SetWidths(new float[] { 1f, 6.5f, 13.5f });


                                closer.AddCell(PhraseCell(new Phrase("7.1.", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                                closer.AddCell(PhraseCell(new Phrase("Closure Comments", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                                closer.AddCell(PhraseCell(new Phrase(("" + dataSet.Tables[1].Rows[0][0].ToString() != "") ? dataSet.Tables[1].Rows[0][0].ToString() : "Data Not Available", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));


                                closer.AddCell(PhraseCell(new Phrase("7.2.", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                                closer.AddCell(PhraseCell(new Phrase("Closed By", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                                closer.AddCell(PhraseCell(new Phrase(("" + dataSet.Tables[1].Rows[0][3].ToString() != "") ? dataSet.Tables[1].Rows[0][3].ToString() : "Data Not Available", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                                closer.AddCell(PhraseCell(new Phrase("7.3.", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                                closer.AddCell(PhraseCell(new Phrase("Closed Date", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                                closer.AddCell(PhraseCell(new Phrase(("" + dataSet.Tables[1].Rows[0][2].ToString() != "") ? dataSet.Tables[1].Rows[0][2].ToString() : "Data Not Available", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                                pdfDoc.Add(closer);

                                //if (dataSet.Tables[0].Rows[0][7].ToString() != "Closed")
                                //{
                                //    string imagePath = Server.MapPath("~/Images/watermark.png");
                                //    iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imagePath);

                                //    image.ScalePercent(200f);
                                //    image.RotationDegrees = 45f;

                                //    image.SetAbsolutePosition(0f, 200f);

                                //    pdfDoc.Add(image);
                                //}
                                //else
                                //{
                                //    string imagePath = Server.MapPath("~/Images/final.png");
                                //    iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imagePath);

                                //    image.ScalePercent(200f);
                                //    image.RotationDegrees = 45f;

                                //    image.SetAbsolutePosition(0f, 150f);

                                //    pdfDoc.Add(image);
                                //}

                                pdfDoc.Close();
                                //pdfDoc.Close();
                                Response.Write(pdfDoc);
                                Response.End();

                            }
                        }
                    }

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

