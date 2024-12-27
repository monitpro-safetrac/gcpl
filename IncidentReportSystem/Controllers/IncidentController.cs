
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
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.IO;
using ClosedXML.Excel;
using MonitPro.Models.CAPA;
using System.Xml.Serialization;
using MonitPro.Common.Library;
using DocumentFormat.OpenXml.Drawing.Wordprocessing;
using System.Web.UI.WebControls;
using SelectPdf;
using System.Drawing.Imaging;
using System.Collections;
using System.Runtime.Remoting.Lifetime;

namespace WorkPermitSystem.Controllers
{

    [ValidateSession]
    public class IncidentController : Controller
    {
        IncidentReportBLL IncidentBLL = new IncidentReportBLL();
        CAPABLL capabll = new CAPABLL();
        IncidentListViewModel Incidents = new IncidentListViewModel();
        List<UserProfile> UserProfiles = new List<UserProfile>();
        List<Role> UserRoles = new List<Role>();
        List<IncidentType> IncidentTypes = new List<IncidentType>();
        List<Status> IncidentStatus = new List<Status>();
        List<IncidentClassfication> IncidentClassfications = new List<IncidentClassfication>();
        List<Priority> IncidentPriorities = new List<Priority>();
        List<Plants> IncidentPlants = new List<Plants>();
        List<InjuryType> InjuryTypes = new List<InjuryType>();
        List<CAPAObservationStatus> TrackActionStatus = new List<CAPAObservationStatus>();
        List<RootCause> RootCause = new List<RootCause>();
        List<Contractor> contractor = new List<Contractor>();
        List<Gender> gender = new List<Gender>();
        List<ContractorEmp> contractoremp = new List<ContractorEmp>();
        Incident NewIncident = new Incident();
        List<TenetsList> tenets = new List<TenetsList>();
        List<Tenets4> tenets4 = new List<Tenets4>();
        SessionDetails sess = new SessionDetails();
        public IncidentController()
        {
            IncidentTypes = IncidentBLL.GetIncidentTypes();
            IncidentStatus = IncidentBLL.GetIncidentStatus();
            IncidentClassfications = IncidentBLL.GetIncidentClassfication();
            IncidentPlants = IncidentBLL.GetPlants();
            IncidentPriorities = IncidentBLL.GetPriorities();

            contractor = IncidentBLL.GetContractor();
            gender = IncidentBLL.GetGender();
            contractoremp = IncidentBLL.GetContractorEmp();
            InjuryTypes = IncidentBLL.GetInjuryTypes();
            TrackActionStatus = IncidentBLL.GetCAPAObservationStatus();
            UserProfiles = IncidentBLL.GetActionList();
            sess = IncidentBLL.GetSession(CurrentUser.UserID);
        }

        // GET: Incident
        public ActionResult Dashboard()
        {
            Incidents.Roles = CurrentUser.Roles;
            Incidents.UserFullName = CurrentUser.FullName;
            Incidents.UserID = CurrentUser.UserID;
            Incidents.ProfileImage = CurrentUser.ProfileImage;
            Incidents.IsRestrict = CurrentUser.IsRestrict;
            Incidents.IncidentList.RemoveAll(x => x.IncidentID > 0);

            return View(Incidents);

        }

        public ActionResult HomePage()
        {
            NewIncidentViewModel incidentVM = new NewIncidentViewModel();
            incidentVM.CurrentSessionID = CurrentUser.CurrentSessionID;
            incidentVM.PrevoiusSessionID = sess.SessionActive;
            if (incidentVM.CurrentSessionID == incidentVM.PrevoiusSessionID)
            {

            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";

            }
            incidentVM.Roles = CurrentUser.Roles;
            incidentVM.UserFullName = CurrentUser.FullName;
            incidentVM.UserID = CurrentUser.UserID;
            incidentVM.IsRestrict = CurrentUser.IsRestrict;
            incidentVM.ProfileImage = CurrentUser.ProfileImage;

            return View(incidentVM);
        }
        [HttpGet]
        public ActionResult CreateNew(int incidentID = 0)
        {
            NewIncidentViewModel incidentVM = new NewIncidentViewModel();
            Incident incident = new Incident();
            incident.CreatedByName = CurrentUser.FullName;
            incidentVM.CurrentSessionID = CurrentUser.CurrentSessionID;
            incidentVM.PrevoiusSessionID = sess.SessionActive;
            if (incidentVM.CurrentSessionID == incidentVM.PrevoiusSessionID)
            {

            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";

            }
            if (incidentID > 0)
                incident = IncidentBLL.GetIncident(incidentID);
            var plants = IncidentPlants.Where(y => y.ID != -1).ToList();
            var incidenttype = IncidentTypes.Where(y => y.ID != -1).ToList();
            var priorityList = IncidentPriorities.Where(y => y.ID != "").ToList();

            incident.IncidentTime = incident.IncidentTime == null ? DateTime.Now.ToString("dd/MM/yyyy HH:mm") : incident.IncidentTime;
            incident.ReportedDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            incident.InvestigationBegan = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            incidentVM.Incident = incident;
            incidentVM.Incident.IncidentID = incidentID;
            incidentVM.IncidentTypeList = incidenttype;
            incidentVM.IncidentClassficationList = IncidentClassfications;
            incidentVM.PlantList = plants;
            incidentVM.prioritiesList = priorityList;
            incidentVM.statusList = IncidentStatus;
            incidentVM.InjuryTypesList = InjuryTypes;
            incidentVM.Roles = CurrentUser.Roles;
            incidentVM.UserFullName = CurrentUser.FullName;
            incidentVM.ProfileImage = CurrentUser.ProfileImage;
            incidentVM.IsRestrict = CurrentUser.IsRestrict;


            return View(incidentVM);
        }

        [HttpPost]
        public ActionResult CreateNew(NewIncidentViewModel incname)
        {
            incname.CurrentUserID = CurrentUser.UserID;

            Incident incident = new Incident();
            if (incname.Incident.ImageFile != null)
            {

                var fileName = Path.GetFileName(incname.Incident.ImageFile.FileName);
                var path = Path.Combine(Server.MapPath("~/IncidentAttachments/"), fileName);
                incname.Incident.ImageFile.SaveAs(path);
            }

            incident.IncidentID = IncidentBLL.IncidentReportUpdate(incname, CurrentUser.UserID);
            incident = IncidentBLL.GetIncident(incident.IncidentID);
            if (incident.IncidentID > 0)
            {
                ViewBag.Message = string.Format("Incident_ {0} created successfully", incident.IncidentNO);
            }
            incident.CreatedByName = CurrentUser.FullName;
            incident.IncidentTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            incident.ReportedDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            incident.InvestigationBegan = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            incname.Incident = incident;
            incname.IncidentTypeList = IncidentTypes;
            incname.IncidentClassficationList = IncidentClassfications;
            incname.PlantList = IncidentPlants;
            incname.prioritiesList = IncidentPriorities;
            incname.statusList = IncidentStatus;
            incname.InjuryTypesList = InjuryTypes;
            incname.Roles = CurrentUser.Roles;
            incname.UserFullName = CurrentUser.FullName;
            incname.ProfileImage = CurrentUser.ProfileImage;

            incname.IsRestrict = CurrentUser.IsRestrict;


            return View(incname);
        }
        [HttpPost]
        public ActionResult DeleteObservation(int observationID)
        {
            string strMessage = String.Empty;
            try
            {
                IncidentBLL.DeleteIncidentObservation(observationID);
                strMessage = "Successfully Deleted";
            }
            catch (Exception ex)
            {
                strMessage = ex.Message;
            }

            return Json(new { strMessage });
        }
        [HttpPost]
        public ActionResult DeleteOBDOC(int obid)
        {

            IncidentBLL.DeleteOBDOC(obid);
            string strMessage = "Successfully Deleted";
            return Json(new { strMessage });
        }

        [HttpGet]
     
        public JsonResult WhyForm(int incidentID = 0)
        {
           
                // Populate the view model
                NewIncidentViewModel incidentVM = new NewIncidentViewModel
                {
                    CurrentSessionID = CurrentUser.CurrentSessionID,
                    PrevoiusSessionID = sess.SessionActive,
                    IncidentID = incidentID,
                    Roles = CurrentUser.Roles,
                    UserFullName = CurrentUser.FullName,
                    ProfileImage = CurrentUser.ProfileImage,
                    IsRestrict = CurrentUser.IsRestrict
                };

                // Fetch WhyForm data
                incidentVM.WhyForm = IncidentBLL.GetWhyForm(incidentID);

                return Json(incidentVM); // Ensure WhyForm.cshtml exists
            
           
        }



        [HttpPost]
        public ActionResult WhyForm(NewIncidentViewModel incidentVM)
        {
            int recordinsert = 0;
            recordinsert = IncidentBLL.Savewhyform(incidentVM);
            if (recordinsert > 0)
            {
                ViewBag.Message = string.Format("Five Why is Saved Successfully");
            }
            incidentVM.Roles = CurrentUser.Roles;
            incidentVM.UserFullName = CurrentUser.FullName;
            incidentVM.ProfileImage = CurrentUser.ProfileImage;

            incidentVM.IsRestrict = CurrentUser.IsRestrict;
            return RedirectToAction("EditIncident", new { incidentID = incidentVM.IncidentID });

        }
        [HttpGet]
        public ActionResult Tenets(int incidentID = 0)
        {
            NewIncidentViewModel incidentVM = new NewIncidentViewModel();
            incidentVM.CurrentSessionID = CurrentUser.CurrentSessionID;
            incidentVM.PrevoiusSessionID = sess.SessionActive;
            if (incidentVM.CurrentSessionID == incidentVM.PrevoiusSessionID)
            {
            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";
            }
            Incident incident = new Incident();
            incident.IncidentID = incidentID;
            incidentVM = IncidentBLL.GetTenets(incidentID);
            incidentVM.IncidentID = incidentID;
            incidentVM.Roles = CurrentUser.Roles;
            incidentVM.UserFullName = CurrentUser.FullName;
            incidentVM.ProfileImage = CurrentUser.ProfileImage;

            incidentVM.IsRestrict = CurrentUser.IsRestrict;
            return View(incidentVM);
        }
        [HttpPost]
        public ActionResult Tenets(NewIncidentViewModel incidentVM, List<TenetsList> Tenets, List<Tenets4> Tenets4)
        {
            foreach (var item in Tenets)
            {
                IncidentBLL.SaveTenets(incidentVM, Tenets);
                break;

            }

            foreach (var item1 in Tenets4)
            {
                IncidentBLL.Save4Tenets(incidentVM, Tenets4);
                break;

            }
            incidentVM.Roles = CurrentUser.Roles;
            incidentVM.UserFullName = CurrentUser.FullName;
            incidentVM.ProfileImage = CurrentUser.ProfileImage;

            incidentVM.IsRestrict = CurrentUser.IsRestrict;
            return RedirectToAction("Tenets", new { incidentID = incidentVM.IncidentID });

        }
        [HttpGet]
        public ActionResult RootCauseCategories(int incidentID = 0)
        {

            NewIncidentViewModel incidentVM = new NewIncidentViewModel();
            incidentVM.CurrentSessionID = CurrentUser.CurrentSessionID;
            incidentVM.PrevoiusSessionID = sess.SessionActive;
            if (incidentVM.CurrentSessionID == incidentVM.PrevoiusSessionID)
            {
            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";
            }
            incidentVM = IncidentBLL.GetSelectedRootCause(incidentID);

            incidentVM.IncidentID = incidentID;
            incidentVM.Roles = CurrentUser.Roles;
            incidentVM.UserFullName = CurrentUser.FullName;
            incidentVM.ProfileImage = CurrentUser.ProfileImage;

            incidentVM.IsRestrict = CurrentUser.IsRestrict;
            return View(incidentVM);
        }

        [HttpPost]
        public ActionResult RootCauseCategories(NewIncidentViewModel incidentVM)
        {

            string RootCauseString = string.Empty;

            List<RootCauseXMLList> rootCauseSubList = new List<RootCauseXMLList>();

            foreach (var list in incidentVM.RootCauseMasterList)
            {

                foreach (var data in list.SubList)
                {
                    if (data.subcheck == true)
                    {
                        var list1 = new RootCauseXMLList
                        {
                            SubsectionID = data.SubsectionID,
                            RootCauseID = list.RootCauseID,
                        };
                        rootCauseSubList.Add(list1);
                    }
                }

            }
            XmlSerializer xmlSerializer = new XmlSerializer(rootCauseSubList.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, rootCauseSubList);

                RootCauseString = textWriter.ToString();
            }
            using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
            {
                int affectedRecordCount = 0;
                SqlCommand objCom = new SqlCommand();
                objCom.CommandText = "RootCauseListSave";
                objCom.CommandType = CommandType.StoredProcedure;
                objCom.Parameters.AddWithValue("@IncidentID", incidentVM.IncidentID);
                objCom.Parameters.AddWithValue("@RootCauseSave", RootCauseString);
                objCon.Open();
                objCom.Connection = objCon;
                affectedRecordCount = objCom.ExecuteNonQuery();
                if (affectedRecordCount > 0)
                {
                    TempData["Message"] = "Data Saved Successfully";

                }
                objCom.Parameters.Clear();
                objCon.Close();
            }
            return RedirectToAction("EditIncident", new { incidentID = incidentVM.IncidentID });
        }
        [HttpGet]
        public ActionResult InjuredPeoples(int incidentID = 0)
        {
            NewIncidentViewModel incidentVM = new NewIncidentViewModel();
            Incident incident = new Incident();
            InjuredPeoples InjuredPeoples = new InjuredPeoples();
            List<InjureList> injurelist = new List<InjureList>();

            if (incidentID > 0)
                incident = IncidentBLL.GetIncident(incidentID);

            incidentVM.IncidentID = incident.IncidentID;
            incidentVM.Title = incident.Title;
            injurelist = IncidentBLL.GetAllInjuredPersonList(incidentID);
            incidentVM.Gender = gender;
            incidentVM.contractorEmp = contractoremp;
            incidentVM.Contractor = contractor;
            incidentVM.InjureList = injurelist;
            incidentVM.Roles = CurrentUser.Roles;
            incidentVM.UserFullName = CurrentUser.FullName;
            incidentVM.ProfileImage = CurrentUser.ProfileImage;

            incidentVM.IsRestrict = CurrentUser.IsRestrict;


            return View(incidentVM);
        }
        [HttpPost]
        public ActionResult InjuredPeoples(NewIncidentViewModel incidentVM)
        {
            Incident incident = new Incident();

            InjuredPeoples InjuredPeoples = new InjuredPeoples();

            IncidentBLL.InsertInjuryPersonDetails(incidentVM);
            incidentVM.Title = incident.Title;
            incidentVM.Contractor = contractor;
            incidentVM.Gender = gender;
            incidentVM.contractorEmp = contractoremp;
            incidentVM.Injuredpeoples = InjuredPeoples;
            incidentVM.Roles = CurrentUser.Roles;
            incidentVM.UserFullName = CurrentUser.FullName;
            incidentVM.ProfileImage = CurrentUser.ProfileImage;

            incidentVM.IsRestrict = CurrentUser.IsRestrict;
            return RedirectToAction("InjuredPeoples", new { incidentID = incidentVM.IncidentID });
        }
        [HttpGet]
        public ActionResult EditIncident(int incidentID = 0)
        {
            NewIncidentViewModel incidentVM = new NewIncidentViewModel();
            List<RootCause> rootcause = new List<RootCause>();
            Incident incident = new Incident();
            incidentVM.CurrentSessionID = CurrentUser.CurrentSessionID;
            incidentVM.PrevoiusSessionID = sess.SessionActive;
            if (incidentVM.CurrentSessionID == incidentVM.PrevoiusSessionID)
            {
            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";
            }
            if (incidentID > 0)
                incident = IncidentBLL.GetIncident(incidentID);
            incidentVM.ObserverTeamList = IncidentBLL.GetAllObservations();

            if (incident.IncidentTypeID == 0)
            {
                incident.IncidentTypeID = 4;
            }

            var incidenttype = IncidentTypes.Where(y => y.ID != -1).ToList();
            var plants = IncidentPlants.Where(y => y.ID != -1).ToList();
            var priorityList = IncidentPriorities.Where(y => y.ID != "").ToList();
            incidentVM.Incident = incident;
            incidentVM.CurrentUserID = CurrentUser.UserID;


            //insObservationVM.RootCauseList = RootCause;

            incidentVM.rootcause = rootcause;
            incidentVM.IncidentTypeList = incidenttype;
            incidentVM.IncidentClassficationList = IncidentClassfications;
            incidentVM.PlantList = plants;
            incidentVM.prioritiesList = priorityList;
            incidentVM.statusList = IncidentStatus;
            incidentVM.InjuryTypesList = InjuryTypes;
            incidentVM.Roles = CurrentUser.Roles;
            foreach (var item in incidentVM.Roles)
            {
                incidentVM.RoleID = item.RoleID;
            }

            IncidentBLL.IncidentCategoryOverallCalculation(incidentID);

            incidentVM.UserFullName = CurrentUser.FullName;
            incidentVM.ProfileImage = CurrentUser.ProfileImage;

            incidentVM.IsRestrict = CurrentUser.IsRestrict;
            return View(incidentVM);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditIncident(NewIncidentViewModel incname)
        {
            incname.CurrentUserID = CurrentUser.UserID;
            var status = incname.Incident.StatusID;
            var IncidentNO = incname.Incident.IncidentNO;
            List<RootCause> rootcause = new List<RootCause>();
            Incident incident = new Incident();
            if (incname.Incident.ImageFile != null)
            {
                if (incname.Incident.ImageFile.ContentLength < 5242880)
                {

                    var fileName = Path.GetFileName(incname.Incident.ImageFile.FileName);
                    var path = Path.Combine(Server.MapPath("~/IncidentAttachments/"), fileName);
                    incname.Incident.ImageFile.SaveAs(path);
                    incident.IncidentID = IncidentBLL.IncidentReportUpdate(incname, CurrentUser.UserID);
                }
                else
                {
                    ViewBag.error = "File size exceeds maximum limit 5 MB.";
                }
            }
            if (incname.Incident.InvesAttachment != null)
            {
                if (incname.Incident.InvesAttachment.ContentLength < 5242880)
                {
                    var fileName2 = Path.GetFileName(incname.Incident.InvesAttachment.FileName);
                    var path = Path.Combine(Server.MapPath("~/InvestigationAttachments/"), fileName2);
                    incname.Incident.InvesAttachment.SaveAs(path);
                    incident.IncidentID = IncidentBLL.IncidentReportUpdate(incname, CurrentUser.UserID);

                }
                else
                {
                    ViewBag.error = "File size exceeds maximum limit 5 MB.";
                }
            }
            if (incname.Incident.ImageFile == null && incname.Incident.InvesAttachment == null)
            {
                incident.IncidentID = IncidentBLL.IncidentReportUpdate(incname, CurrentUser.UserID);
            }
            if (incident.IncidentID > 0 && status == 4)
            {
                ViewBag.Message = string.Format("Incident ID_ {0} has been submitted for approval", IncidentNO);
            }
            else
            {
                ViewBag.Message = string.Format("Incident ID_ {0} is Saved Successfully", IncidentNO);

            }
            var incidenttype = IncidentTypes.Where(y => y.ID != -1).ToList();
            var plants = IncidentPlants.Where(y => y.ID != -1).ToList();
            var priorityList = IncidentPriorities.Where(y => y.ID != "").ToList();
            Incidents.ObserverTeamList = IncidentBLL.GetAllObservations();
            incname.Incident = incident;
            incname.rootcause = rootcause;
            incname.IncidentTypeList = incidenttype;
            incname.IncidentClassficationList = IncidentClassfications;
            incname.PlantList = plants;
            incname.prioritiesList = priorityList;
            incname.statusList = IncidentStatus;
            incname.InjuryTypesList = InjuryTypes;

            IncidentBLL.IncidentCategoryOverallCalculation(incident.IncidentID);

            incname.Roles = CurrentUser.Roles;
            incname.UserFullName = CurrentUser.FullName;
            incname.ProfileImage = CurrentUser.ProfileImage;

            incname.IsRestrict = CurrentUser.IsRestrict;

            return View(incname);
        }
        public ActionResult CategoryDesicion(int incidentid, int DecisionID = 0)
        {
            Incident inci = new Incident();
            IncidentMaincategoryModel maindecidion = new IncidentMaincategoryModel();
            maindecidion.DecisionTypeList = IncidentBLL.GetDecisionTypesDD(incidentid);

            maindecidion.decisionlist = IncidentBLL.GetIncidentCategoryDecisions(DecisionID, incidentid);
            maindecidion.calculationResult = IncidentBLL.GetCategoryCalculation(incidentid, DecisionID);
            if (DecisionID == 5)
            {


                maindecidion.Api754List = IncidentBLL.GetAPI754Details(incidentid);
                var temp = maindecidion.Api754List.Where(x => x.UserValue != 0).ToList();
                maindecidion.tempValue = temp.Count;
                maindecidion.ChemicalList = new List<ChemicalQTY> { };

            }
            else if (DecisionID == 6)
            {
                maindecidion.ChemicalList = IncidentBLL.GetIncidentChemicalQTYDetails(incidentid);
                var temp = maindecidion.ChemicalList.Where(x => x.UserValue == 0).ToList();
                maindecidion.tempValue = temp.Count;
                maindecidion.Api754List = new List<API754Details> { };
            }
            else
            {
                maindecidion.ChemicalList = new List<ChemicalQTY> { };
                maindecidion.Api754List = new List<API754Details> { };
            }
            inci = IncidentBLL.GetIncident(incidentid);
            maindecidion.IncidentTitle = inci.Title;
            maindecidion.IncidentNO = inci.IncidentNO;
            maindecidion.PlantName = inci.PlantName;
            maindecidion.IncidentChemicalQTYType = inci.IncidentChemicalQTY;
            maindecidion.IncidentID = incidentid;
            maindecidion.DecisionTypeID = DecisionID;
            maindecidion.CurrentUserID = CurrentUser.UserID;
            maindecidion.Roles = CurrentUser.Roles;
            maindecidion.UserFullName = CurrentUser.FullName;
            maindecidion.ProfileImage = CurrentUser.ProfileImage;

            maindecidion.IsRestrict = CurrentUser.IsRestrict;
            return View(maindecidion);
        }


        [HttpPost]
        public ActionResult CategoryDesicion(IncidentMaincategoryModel maindecidion, int DecisionID = 0)
        {
            int affect = 0;
            affect = IncidentBLL.IncidentCategoryInsert(maindecidion);
            if (affect > 0)
            {
                ViewBag.CategoryMessage = string.Format("Data Saved Successfully!");
            }
            maindecidion.DecisionTypeList = IncidentBLL.GetDecisionTypesDD(maindecidion.IncidentID);
            maindecidion.decisionlist = IncidentBLL.GetIncidentCategoryDecisions(DecisionID, maindecidion.IncidentID);
            maindecidion.calculationResult = IncidentBLL.GetCategoryCalculation(maindecidion.IncidentID, DecisionID);
            maindecidion.Api754List = new List<API754Details> { };
            maindecidion.ChemicalList = new List<ChemicalQTY> { };
            maindecidion.Roles = CurrentUser.Roles;
            maindecidion.UserFullName = CurrentUser.FullName;
            maindecidion.ProfileImage = CurrentUser.ProfileImage;

            maindecidion.CurrentUserID = CurrentUser.UserID;
            maindecidion.IsRestrict = CurrentUser.IsRestrict;
            return View(maindecidion);
        }
        public ActionResult FishBoneDiagram(int IncidentID)
        {
            FishBone fish = new FishBone();
            Incident inci = new Incident();
            fish = IncidentBLL.GetFishBoneDetails(IncidentID);
            if (fish.Header1 == null)
            {
                fish.Header1 = "Man";
                fish.Header2 = "Machine";
                fish.Header3 = "Method";
                fish.Header4 = "Material";
                fish.Header5 = "Measurement";
                fish.Header6 = "Environment";
            }
            fish.IncidentID = IncidentID;
            inci = IncidentBLL.GetIncident(IncidentID);
            fish.Title = inci.Title;
            fish.IncidentNo = inci.IncidentNO;
            fish.PlantName = inci.PlantName;
            fish.Roles = CurrentUser.Roles;
            fish.UserFullName = CurrentUser.FullName;
            fish.ProfileImage = CurrentUser.ProfileImage;

            fish.CurrentUserID = CurrentUser.UserID;
            fish.IsRestrict = CurrentUser.IsRestrict;
            return View(fish);
        }
        [HttpPost]
        public ActionResult FishBoneDiagram(FishBone fish)
        {
            fish.CurrentUserID = CurrentUser.UserID;

            if (fish.ButtonValue == 1)
            {
                fish.FishImage = HtmlToImgConverter(fish);
            }
            int affect = IncidentBLL.FishBoneInsert(fish);
            if (affect > 0)
                ViewBag.FishMessage = string.Format("Data Saved Successfully!");
            fish.Roles = CurrentUser.Roles;
            fish.UserFullName = CurrentUser.FullName;
            fish.ProfileImage = CurrentUser.ProfileImage;

            fish.CurrentUserID = CurrentUser.UserID;
            fish.IsRestrict = CurrentUser.IsRestrict;
            return View(fish);
        }
        public string HtmlToImgConverter(FishBone fish)
        {
            string htmlString = "";
            htmlString = $@" <html lang=""en"">
    <head>
         <style>
            body {{
                font-family: Arial, sans-serif;
                margin: 0;
                padding: 0;
                display: flex;
                justify-content: center;
                align-items: center;
                height: 100vh;
                flex-direction: column;
                justify-content: flex-end;
            }}


 
            .fishbone {{
                width: 1500px;
                height: 750px;
                align-content: center;
                background-color: #fff;
                border: 2px solid #333;
                border-radius: 8px;
                padding: 20px;
                box-shadow: 0 0 10px rgba(0, 0, 0, 0.2);
            }}

            .head {{
                text-align: center;
                font-weight: bold;
                font-size: 20px;
                margin-bottom: 20px;
            }}

            .branches {{
                display: flex;
                justify-content: space-around;
                align-items: center;
            }}

            .Nextbranch {{
                display: flex;
                justify-content: space-around;
                align-items: center;
            }}

            .branch {{
                text-align: center;
            }}

            .label {{
                font-weight: bold;
                font-size: 14px;
                color: black;
                margin-bottom: 5px;
            }}

            .input-sm {{
                background-color: antiquewhite;
            }}

            input[type=""text""] {{
                border: none;
                background: none;
                outline: none;
                border-bottom: 1px solid #333;
                font-size: 14px;
                width: 80%;
                padding: 5px;
                margin: 2px;
            }}

            .triangle-line-container {{
                display: flex;
                align-items: center;
            }}

            .triangle {{
                width: 0;
                height: 0;
                border-top: 50px solid transparent;
                border-bottom: 50px solid transparent;
                border-left: 87px solid #3498db; /* Change this color as needed */
                transform: rotate(120deg); /* Rotate 120 degrees */
                margin-right: 40px; /* Adjust the margin as needed to make them touch */
                margin-top: 20px;
            }}
            .diamond {{
                 width: 152px; /* Set the width of the diamond */
                height: 152px; /* Set the height of the diamond */
                background-color: #3498db;
                transform: rotate(45deg);
                margin-right: 10px;
                margin-top: 10px;
                display: flex;
                justify-content: center;
                align-items: center;
                position: relative;
                overflow: hidden;
                color:white;
                 white-space: pre-wrap;
            }}

            .rotated-text {{text - align: center;
                transform: rotate(-45deg); /* Rotate the text inside the diamond */
                white-space: pre-wrap; /* Allow the text to wrap to the next line */
                overflow: hidden; /* Hide overflowing text */
            }}



            .line {{
                flex: 2;
                height: 2px;
                background-color: #000;
                margin-bottom: -10px; /* Adjust the margin to move the line up */
            }}

            .slanted-line {{
                width: 300px;
                height: 2px; /* Adjust the thickness of the line as needed */
                background-color: #333;
                margin-bottom: 20px; /* Add spacing between lines */
                transform-origin: 0% 0%; /* Set the rotation origin to the top-left corner */
            }}

                /* Customize each slanted line */
                .slanted-line:nth-child(1) {{
                    width: 100px; /* Decrease the width of the first line */
                    transform: skewY(-30deg) translateY(127px) translateX(120px); /* Skew and move the first line down */
                }}

                .slanted-line:nth-child(2) {{
                    width: 100px; /* Decrease the width of the second line */
                    transform: skewY(-30deg) translateY(382px) translateX(600px); /* Skew and move the second line down */
                }}

                .slanted-line:nth-child(3) {{
                    width: 100px; /* Decrease the width of the third line */
                    transform: skewY(-30deg) translateY(620px) translateX(1050px); /* Skew and move the third line down */
                }}

                .slanted-line:nth-child(4) {{
                    width: 100px; /* Decrease the width of the first line */
                    transform: skewY(30deg) translateY(-192px) translateX(120px); /* Skew and move the first line down */
                }}

                .slanted-line:nth-child(5) {{
                    width: 100px; /* Decrease the width of the second line */
                    transform: skewY(30deg) translateY(-493px) translateX(600px); /* Skew and move the second line down */
                }}

                .slanted-line:nth-child(6) {{
                    width: 100px; /* Decrease the width of the third line */
                    transform: skewY(30deg) translateY(-775px) translateX(1050px); /* Skew and move the third line down */
                }}


            .double-arrow {{
                width: 0;
                height: 0;
                border-left: 80px solid transparent; /* Adjust the size of the arrow */
                border-right: 10px solid transparent; /* Adjust the size of the arrow */
                border-top: 70px solid #333; /* Color of the arrow */
                border-bottom: 70px solid #333; /* Color of the arrow */
            }}
        </style>
        <meta charset=""UTF-8"">
        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
        <title>Fishbone Diagram</title>
    </head>
    <body>
       
            <div class=""fishbone"" >
                <div class=""branches"">
                    <div class=""branch"">
                        <textarea type=""text"" class=""form-control input-sm"" maxlength=""200"" style=""width: 300px; height:50px; max-width: 100%;font-weight: bold;"">{fish.Header1}</textarea>
                        <input type=""text""   value=""{fish.ManSub1}"">
                        <input type=""text"" value=""{fish.ManSub2}"">
                        <input type=""text""  value=""{fish.ManSub3}"">
                        <input type=""text""   value=""{fish.ManSub4}"">
                        <input type=""text""  value=""{fish.ManSub5}"">
                    </div>
                    <div class=""branch"">
                        <textarea type=""text"" class=""form-control input-sm"" maxlength=""200"" style=""width: 300px; height:50px; max-width: 100%;font-weight: bold;"">{fish.Header2}</textarea>
                        
                        <input type=""text""  value=""{fish.MachiSub1}"">
                        <input type = ""text"" value = ""{fish.MachiSub2}"">
                        <input type = ""text""  value=""{fish.MachiSub3}  "">
                        <input type = ""text"" value=""{fish.MachiSub4} "">
                        <input type=""text""  value=""{fish.MachiSub5}"">
                    </div>
                    <div class=""branch"">
                        <textarea type=""text"" class=""form-control input-sm"" maxlength=""200"" style=""width: 300px; height:50px; max-width: 100%;font-weight: bold;"">{fish.Header3}</textarea>
                        
                        <input type=""text""  value=""{fish.MethodSub1}"">
                        <input type=""text""  value=""{fish.MethodSub2}"">
                        <input type = ""text"" value = ""{fish.MethodSub3}"">
                        <input type = ""text""   value=""{fish.MethodSub4}"">
                        <input type = ""text""   value=""{fish.MethodSub5} "">
                    </div>
                </div><br/>
                <div class=""triangle-line-container"">
                    <div class=""double-arrow""></div>
                    <div class=""line "">
                        <div class=""slanted-line""></div>
                        <div class=""slanted-line""></div>
                        <div class=""slanted-line""></div>
                        <div class=""slanted-line""></div>
                        <div class=""slanted-line""></div>
                        <div class=""slanted-line""></div>
                    </div>
                   <div class=""diamond"">
                        <p class=""rotated-text""> {fish.Title}</p>
                    </div>
                </div>
                <div class=""Nextbranch"">
                    <div class=""branch"">

                        <input type=""text"" value=""{fish.MaterialSub1}"">
                        <input type=""text"" value=""{fish.MaterialSub2}"">
                        <input type=""text"" value=""{fish.MaterialSub3}"">
                        <input type=""text""  value=""{fish.MaterialSub4}"">
                        <input type=""text""  value=""{fish.MaterialSub5}""><br /><br />
                      <textarea type=""text"" class=""form-control input-sm""  maxlength=""200"" style=""width: 300px; height:50px; max-width: 100%;font-weight: bold;"">{fish.Header4}</textarea>
                    </div>
                    <div class=""branch"">

                        <input type=""text""  value=""{fish.MeasureSub1}"">
                        <input type=""text""  value=""{fish.MeasureSub2}"">
                        <input type=""text""  value=""{fish.MeasureSub3}"">
                        <input type=""text""  value=""{fish.MeasureSub4}"" />
                        <input type=""text""  value=""{fish.MeasureSub5}"" /><br /><br />
                       <textarea type=""text"" class=""form-control input-sm""  maxlength=""200"" style=""width: 300px; height:50px; max-width: 100%;font-weight: bold;"">{fish.Header5}</textarea>
                    </div>
                    <div class=""branch"">

                        <input type=""text""  value=""{fish.EnviSub1}"">
                        <input type=""text""  value=""{fish.EnviSub2}"">
                        <input type=""text""  value=""{fish.EnviSub3}"">
                        <input type=""text""  value=""{fish.EnviSub4}"">
                        <input type=""text""  value=""{fish.EnviSub5}""><br /><br />
                      <textarea type=""text"" class=""form-control input-sm""  maxlength=""200"" style=""width: 300px; height:50px; max-width: 100%;font-weight: bold;"">{fish.Header6}</textarea>
                    </div>
                </div>
            </div
           
       </body>
</html>";
            // var path = Path.Combine(Server.MapPath("~/UploadImages/"), fileName);
            string baseUrl = "~/FishBoneDiagram/";

            ImageFormat imageFormat = ImageFormat.Png;

            int webPageWidth = 1800;
            int webPageHeight = 800;
            // instantiate a html to image converter object
            HtmlToImage imgConverter = new HtmlToImage();

            // set converter options
            imgConverter.WebPageWidth = webPageWidth;
            imgConverter.WebPageHeight = webPageHeight;

            System.Diagnostics.Debug.WriteLine(htmlString);
            // create a new image converting an url
            System.Drawing.Image image =
                imgConverter.ConvertHtmlString(htmlString, baseUrl);

            // get image bytes
            byte[] img = ImageToByteArray(image, imageFormat);

            // return resulted image

            MemoryStream ms = new MemoryStream(img, 0, img.Length);
            // Convert byte[] to Image
            ms.Write(img, 0, img.Length);
            System.Drawing.Image image1 = System.Drawing.Image.FromStream(ms, true);
            //save image
            string temp = Guid.NewGuid().ToString() + ".png";
            image.Save(Server.MapPath("~/FishBoneDiagram/" + temp), ImageFormat.Png);

            return temp;
        }
        private byte[] ImageToByteArray(System.Drawing.Image imageIn, ImageFormat imageFormat)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, imageFormat);
            return ms.ToArray();
        }
        public ActionResult AddAttachments()
        {
            AttachmentsViewModel attachmentsVM = new AttachmentsViewModel();
            if (System.Web.HttpContext.Current.Session["Attachments"] != null)
            {
                attachmentsVM = (AttachmentsViewModel)System.Web.HttpContext.Current.Session["Attachments"];
            }

            return PartialView(attachmentsVM);
        }

        [HttpPost]
        public ActionResult AddAttachments(string FilePath)
        {
            AttachmentsViewModel attachmentsVM = new AttachmentsViewModel();

            if (System.Web.HttpContext.Current.Session["Attachments"] != null)
            {
                attachmentsVM = (AttachmentsViewModel)System.Web.HttpContext.Current.Session["Attachments"];
            }

            attachmentsVM.attachments.Add(IncidentBLL.AddTempAttachment(FilePath, System.Web.HttpContext.Current.Session.SessionID.ToString(), attachmentsVM.attachments.Count));
            System.Web.HttpContext.Current.Session["Attachments"] = attachmentsVM;

            return PartialView(attachmentsVM);
        }
        // GET: Incident
        [HttpGet]
        public ActionResult Index()
        {
            Incidents.CurrentSessionID = CurrentUser.CurrentSessionID;
            Incidents.PrevoiusSessionID = sess.SessionActive;
            if (Incidents.CurrentSessionID == Incidents.PrevoiusSessionID)
            {

            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";

            }
            Incidents.IncidentList = IncidentBLL.GetOpenIncidents();
            Incidents.CurrentUser = CurrentUser.UserID;
            Incidents.statusList = IncidentStatus;
            Incidents.IncidentClassficationList = IncidentClassfications;
            Incidents.ObserverTeamList = IncidentBLL.GetAllObservations();
            Incidents.statusList = IncidentBLL.GetIncidentStatus();
            var status = IncidentBLL.GetIncidentStatus();
            var ActionStatus = status.Where(y => y.ID != 3 && y.ID != 7).ToList();
            Incidents.statusList = ActionStatus;

            ViewBag.IncidentTypes = new SelectList(IncidentTypes, "ID", "Name");
            ViewBag.IncidentPlant = new SelectList(IncidentPlants, "ID", "Name");
            IncidentSearchViewModel incidentSearchViewModel = new IncidentSearchViewModel();

            ViewBag.fromdate = incidentSearchViewModel.IncidentFromDate;
            ViewBag.Todate = incidentSearchViewModel.IncidentToDate;
            ViewBag.PlantID = incidentSearchViewModel.IncidentPlant;
            ViewBag.IncidentType = incidentSearchViewModel.IncidentType;
            ViewBag.IncidentStatus = incidentSearchViewModel.IncidentStatus;
            ViewBag.Title1 = incidentSearchViewModel.IncidentTitle;
            ViewBag.InciClass = incidentSearchViewModel.InciClass;
            Incidents.Roles = CurrentUser.Roles;
            Incidents.UserFullName = CurrentUser.FullName;
            Incidents.ProfileImage = CurrentUser.ProfileImage;

            Incidents.IsRestrict = CurrentUser.IsRestrict;
            return View(Incidents);
        }

        [HttpPost]
        public ActionResult Index(IncidentSearchViewModel incidentSearchViewModel)
        {
            List<IncidentViewModel> incidentLists = new List<IncidentViewModel>();
            Incidents.IncidentClassficationList = IncidentClassfications;
            Incidents.ObserverTeamList = IncidentBLL.GetAllObservations();
            Incidents.IncidentList = IncidentBLL.SearchOpenIncidents(incidentSearchViewModel);

            Incidents.CurrentUser = CurrentUser.UserID;

            Incidents.IncidentSearchVM = incidentSearchViewModel;
            Incidents.IncidentSearchVM.InciClass = incidentSearchViewModel.InciClass;
            Incidents.IncidentSearchVM.IncidentFromDate = incidentSearchViewModel.IncidentFromDate;
            Incidents.IncidentSearchVM.IncidentToDate = incidentSearchViewModel.IncidentToDate;
            Incidents.IncidentSearchVM.IncidentPlant = incidentSearchViewModel.IncidentPlant;
            Incidents.IncidentSearchVM.IncidentType = incidentSearchViewModel.IncidentType;
            Incidents.IncidentSearchVM.IncidentTitle = incidentSearchViewModel.IncidentTitle;
            Incidents.IncidentSearchVM.IncidentStatus = incidentSearchViewModel.IncidentStatus;
            Incidents.statusList = IncidentBLL.GetIncidentStatus();
            var status = IncidentBLL.GetIncidentStatus();
            var ActionStatus = status.Where(y => y.ID != 3 && y.ID != 7).ToList();
            Incidents.statusList = ActionStatus;
            Incidents.Roles = CurrentUser.Roles;
            Incidents.UserFullName = CurrentUser.FullName;
            Incidents.ProfileImage = CurrentUser.ProfileImage;

            Incidents.IsRestrict = CurrentUser.IsRestrict;
            ViewBag.IncidentTypes = new SelectList(IncidentTypes, "ID", "Name");
            ViewBag.IncidentPlant = new SelectList(IncidentPlants, "ID", "Name");
            ViewBag.fromdate = incidentSearchViewModel.IncidentFromDate;
            ViewBag.Todate = incidentSearchViewModel.IncidentToDate;
            ViewBag.PlantID = incidentSearchViewModel.IncidentPlant;
            ViewBag.IncidentType = incidentSearchViewModel.IncidentType;
            ViewBag.IncidentStatus = incidentSearchViewModel.IncidentStatus;
            ViewBag.Title1 = incidentSearchViewModel.IncidentTitle;
            ViewBag.InciClass = incidentSearchViewModel.InciClass;

            return View(Incidents);
        }

        public ActionResult ExportIncidentList(string currentFromDate, string currentEndDate, string title, int IncidentPlantID, int incidentstatus, int incidenttype, int inciclass)
        {


            using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "ExportIncidentList";
                objCom.CommandType = CommandType.StoredProcedure;
                objCom.Parameters.AddWithValue("@FromDate", currentFromDate == null ? string.Empty : currentFromDate);
                objCom.Parameters.AddWithValue("@ToDate", currentEndDate == null ? string.Empty : currentEndDate);
                objCom.Parameters.AddWithValue("@Title", title == null ? string.Empty : title);
                objCom.Parameters.AddWithValue("@IncidentStatus", incidentstatus);
                objCom.Parameters.AddWithValue("@IncidentType", incidenttype);
                objCom.Parameters.AddWithValue("@IncidentPlant", IncidentPlantID);
                objCom.Parameters.AddWithValue("@InciClass", inciclass);

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

        [HttpPost]
        public ActionResult SearchResults(IncidentSearchViewModel incidentSearchViewModel)
        {
            Incidents.Roles = UserRoles;
            Incidents.IncidentList = IncidentBLL.SearchOpenIncidents(incidentSearchViewModel);

            ViewBag.IncidentTypes = new SelectList(IncidentTypes, "ID", "Name");
            ViewBag.IncidentStatus = new SelectList(IncidentStatus, "ID", "Name");
            ViewBag.IncidentPlant = new SelectList(IncidentPlants, "ID", "Name");
            return View(Incidents);
        }

        // GET: Incident
        public ActionResult IncidentObserver()
        {
            Incidents.IncidentList.RemoveAll(x => x.IncidentType == "Fire");
            //Incidents.IncidentList.Add(new IncidentViewModel() { IncidentID = 1, Title = "Fire Incident", Description = "This occured during test run", IncidentTime = DateTime.Now.AddDays(-3), PlantArea = "Out Field", IncidentType = "Fire", CurrentStatus = "Pending" });
            return View(Incidents);
        }

        // GET: Incident
        [HttpGet]
        public ActionResult History()
        {

            Incidents.Roles = UserRoles;
            ViewBag.IncidentTypes = new SelectList(IncidentTypes, "ID", "Name");
            ViewBag.PlantsList = new SelectList(IncidentPlants, "ID", "Name");
            Incidents.IncidentList = IncidentBLL.GetAllClosedIncidents();
            Incidents.CurrentSessionID = CurrentUser.CurrentSessionID;
            Incidents.PrevoiusSessionID = sess.SessionActive;
            if (Incidents.CurrentSessionID == Incidents.PrevoiusSessionID)
            {
            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";
            }
            Incidents.IncidentClassficationList = IncidentClassfications;
            IncidentSearchViewModel incidentSearchViewModel = new IncidentSearchViewModel();
            ViewBag.fromdate = incidentSearchViewModel.IncidentFromDate;
            ViewBag.Todate = incidentSearchViewModel.IncidentToDate;
            ViewBag.PlantID = incidentSearchViewModel.IncidentPlant;
            ViewBag.IncidentType = incidentSearchViewModel.IncidentType;
            ViewBag.InciClass = incidentSearchViewModel.InciClass;
            Incidents.Roles = CurrentUser.Roles;
            Incidents.UserFullName = CurrentUser.UserName;
            Incidents.UserID = CurrentUser.UserID;

            Incidents.IsRestrict = CurrentUser.IsRestrict;
            return View(Incidents);
        }

        [HttpPost]
        public ActionResult History(IncidentSearchViewModel incidentSearchViewModel)
        {
            incidentSearchViewModel.IncidentStatus = 3;

            ViewBag.IncidentTypes = new SelectList(IncidentTypes, "ID", "Name");
            ViewBag.PlantsList = new SelectList(IncidentPlants, "ID", "Name");
            Incidents.IncidentClassficationList = IncidentClassfications;
            Incidents.IncidentList = IncidentBLL.SearchClosedIncidents(incidentSearchViewModel);
            Incidents.IncidentSearchVM.IncidentFromDate = incidentSearchViewModel.IncidentFromDate;
            Incidents.IncidentSearchVM.IncidentToDate = incidentSearchViewModel.IncidentToDate;
            Incidents.IncidentSearchVM.InciClass = incidentSearchViewModel.InciClass;
            Incidents.IncidentSearchVM.IncidentPlant = incidentSearchViewModel.IncidentPlant;
            Incidents.IncidentSearchVM.IncidentType = incidentSearchViewModel.IncidentType;
            ViewBag.fromdate = incidentSearchViewModel.IncidentFromDate;
            ViewBag.Todate = incidentSearchViewModel.IncidentToDate;
            ViewBag.PlantID = incidentSearchViewModel.IncidentPlant;
            ViewBag.IncidentType = incidentSearchViewModel.IncidentType;
            ViewBag.InciClass = incidentSearchViewModel.InciClass;
            Incidents.Roles = CurrentUser.Roles;
            Incidents.UserFullName = CurrentUser.UserName;
            Incidents.UserID = CurrentUser.UserID;

            Incidents.IsRestrict = CurrentUser.IsRestrict;
            return View(Incidents);
        }
        public ActionResult ExportIncidentHistory(string currentFromDate, string currentEndDate, int currentPlantID, int currentInciType, int inciClass)
        {


            using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "ExportIncidentsHistory";
                objCom.CommandType = CommandType.StoredProcedure;
                objCom.Parameters.AddWithValue("@FromDate", currentFromDate == null ? string.Empty : currentFromDate);
                objCom.Parameters.AddWithValue("@ToDate", currentEndDate == null ? string.Empty : currentEndDate);
                objCom.Parameters.AddWithValue("@IncidentType", currentInciType);
                objCom.Parameters.AddWithValue("@IncidentPlant ", currentPlantID);
                objCom.Parameters.AddWithValue("@InciClass", inciClass);


                objCon.Open();
                objCom.Connection = objCon;
                SqlDataAdapter Adapter = new SqlDataAdapter();
                Adapter.SelectCommand = objCom;
                DataSet dataSet = new DataSet();
                Adapter.Fill(dataSet);
                objCon.Close();
                var wb = new XLWorkbook(Server.MapPath("~/Template/IncidentHistory.xlsx"));
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
                Response.AddHeader("content-disposition", "attachment;filename= ClosedIncidentHistoryRecord.xlsx");
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
        //Incident Recommendation Chart
        public ActionResult IncidentRecommenChart()
        {
            ObservationStatusChart ObStatus = new ObservationStatusChart()
            {
                Roles = UserRoles,
                ActionCounts = IncidentBLL.GetRecommenStatusCount()
            };

            return View(ObStatus);
        }
        //Incident Category Chart
        public ActionResult IncidentCategoryChart()
        {
            IncidentTypeChart CategoryChart = new IncidentTypeChart()
            {
                Roles = UserRoles,
                CategoryCounts = IncidentBLL.GetIncidentCategoryCount()
            };

            return View(CategoryChart);
        }
        // GET: Chart Report
        public ActionResult CurrentStatusChart()
        {
            StatusChartViewModel statusChartViewModel = new StatusChartViewModel()
            {
                Roles = UserRoles,
                statusCounts = IncidentBLL.GetStatusCount()
            };

            return View(statusChartViewModel);
        }

        // GET: Chart Report
        public ActionResult CurrentYearChart()
        {
            ChartViewModel chartViewModel = new ChartViewModel
            {
                Roles = UserRoles,
                MonthlyCounts = IncidentBLL.GetMonthlyCount()
            };

            return View(chartViewModel);
        }

        public ActionResult MyActionStatusChart()
        {
            ActionsStatusChartViewModel MyactionstatusChartViewModel = new ActionsStatusChartViewModel()
            {
                Roles = UserRoles,
                ActionCounts = IncidentBLL.GetMyActionStatusCount(CurrentUser.UserID)
            };

            return View(MyactionstatusChartViewModel);
        }

        public ActionResult IncidentClassificationCount()
        {
            IncidentClassificationCount classificationcount = new IncidentClassificationCount
            {
                Roles = UserRoles,
                classificationCounts = IncidentBLL.GetClassificationCount()
            };

            return View(classificationcount);
        }
        public ActionResult RootCauseCount()
        {
            RootCauseChartViewModel rootcausecount = new RootCauseChartViewModel
            {
                Roles = UserRoles,
                rootCauseCounts = IncidentBLL.GetRootCauseCount()
            };

            return View(rootcausecount);
        }


        [HttpGet]
        public ActionResult UploadImages(int incidentID)
        {
            IncidentImageViewModel incidentImageVM = new IncidentImageViewModel();
            incidentImageVM.CurrentSessionID = CurrentUser.CurrentSessionID;
            incidentImageVM.PrevoiusSessionID = sess.SessionActive;
            if (incidentImageVM.CurrentSessionID == incidentImageVM.PrevoiusSessionID)
            {
            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";
            }
            Incident incident = new Incident();
            incident = IncidentBLL.GetIncident(incidentID);
            incidentImageVM.IncidentImage.IncidentId = incidentID;
            incidentImageVM.IncidentTitle = incident.Title;
            incidentImageVM.IncidentDetail = incident.Description;
            incidentImageVM.PlantArea = incident.PlantName;
            incidentImageVM.IncidentNO = incident.IncidentNO;
            incidentImageVM.IncidentImages = IncidentBLL.GetIncidentImages(incidentID);
            incidentImageVM.UserFullName = CurrentUser.UserName;
            incidentImageVM.UserID = CurrentUser.UserID;
            incidentImageVM.Roles = CurrentUser.Roles;

            incidentImageVM.IsRestrict = CurrentUser.IsRestrict;
            return PartialView(incidentImageVM);
        }
        [HttpPost]
        public ActionResult UploadImages(IncidentImage incidentImage)
        {

            IncidentImageViewModel incidentImageVM = new IncidentImageViewModel();
            Incident incident = new Incident();
            incident = IncidentBLL.GetIncident(incidentImage.IncidentId);

            incidentImageVM.IncidentID = incidentImage.IncidentId;

            if (incidentImage.ImageFile != null && incidentImage.ImageName != null)
            {
                var fileName = Path.GetFileName(incidentImage.ImageFile.FileName);
                var path = Path.Combine(Server.MapPath("~/UploadImages/"), fileName);
                incidentImage.ImageFile.SaveAs(path);


                bool statusFlag = IncidentBLL.UploadImages(incidentImage, CurrentUser.UserID);
                return RedirectToAction("UploadImages", new { incidentID = incidentImageVM.IncidentID });
            }


            incidentImageVM.Roles = CurrentUser.Roles;
            incidentImageVM.UserFullName = CurrentUser.FullName;
            incidentImageVM.ProfileImage = CurrentUser.ProfileImage;

            incidentImageVM.IsRestrict = CurrentUser.IsRestrict;
            incidentImageVM.IncidentImage.IncidentId = incidentImage.IncidentId;
            incidentImageVM.IncidentTitle = incident.Title;
            incidentImageVM.IncidentDetail = incident.Description;
            incidentImageVM.IncidentImages = IncidentBLL.GetIncidentImages(incidentImage.IncidentId);

            return PartialView(incidentImageVM);

        }


        [HttpPost]
        public ActionResult DeleteImage(int IncidentImageID)
        {

            IncidentBLL.DeleteImage(IncidentImageID);
            string strMessage = "Deleted Successfully";
            return Json(new { strMessage });
        }

        [HttpPost]
        public ActionResult IncidentDetail(int IncidentID)
        {
            NewIncidentViewModel incidentVM = new NewIncidentViewModel();
            Incident incident = new Incident();

            incidentVM.Roles = UserRoles;
            incidentVM.Incident = incident;

            return PartialView(incidentVM);
        }



        [HttpPost]
        public ActionResult DeleteIncident(int IncidentID)
        {
            return View();
        }

        [HttpPost]
        public ActionResult DeleteIncidentItem(int IncidentID)
        {
            return PartialView();
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetDeptEmployees(int? deptID)
        {


            var EmployeeList = IncidentBLL.GetDeptEmployees(deptID).Select(m => new SelectListItem()
            {
                Value = m.DeptEmpID.ToString(),
                Text = m.FullName
            });
            return Json(EmployeeList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AssignObservers(int incidentID)
        {
            Incident incident = new Incident();
            ObserversViewModel observers = new ObserversViewModel();
            observers.CurrentSessionID = CurrentUser.CurrentSessionID;
            observers.PrevoiusSessionID = sess.SessionActive;
            if (observers.CurrentSessionID == observers.PrevoiusSessionID)
            {
            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";
            }
            List<Employee> EmployeeList = IncidentBLL.GetAllEmployees();
            List<Employee> ObserversList = IncidentBLL.GetIncidentObservers(incidentID);
            List<Employee> Lead = IncidentBLL.GetAllEmployees();
            ViewBag.EmployeeList = new SelectList(EmployeeList, "ID", "FullName");
            ViewBag.ObserversList = new SelectList(ObserversList, "ID", "FullName");

            incident = IncidentBLL.GetIncident(incidentID);
            IncidentObserverViewModel inciObserver = new IncidentObserverViewModel();
            inciObserver = IncidentBLL.GetIncidentLead(incidentID);
            observers.DepartmentList = IncidentBLL.GetDepartmentList();
            observers.InvestigatorList = IncidentBLL.GetInvestigator();
            observers.IncidentTitle = incident.Title;
            observers.IncidentType = incident.IncidentType;
            observers.LeadList = Lead;
            observers.GeneralManagerList = IncidentBLL.GetAllGeneralManager();
            observers.Obsevers = inciObserver;
            observers.IncidentID = incidentID;
            observers.IncidentNO = incident.IncidentNO;
            observers.Roles = CurrentUser.Roles;
            foreach (var a in observers.Roles)
            {
                observers.roleid = a.RoleID;
            }
            observers.UserFullName = CurrentUser.FullName;
            observers.ProfileImage = CurrentUser.ProfileImage;

            observers.IsRestrict = CurrentUser.IsRestrict;
            return PartialView(observers);
        }



        [HttpPost]
        public ActionResult SaveObservers(int incidentID, string observerList, string lead, string deptManager, string investigator)
        {
            string strMessage = String.Empty;
            try
            {
                IncidentBLL.SaveObservers(incidentID, observerList, lead, deptManager, CurrentUser.UserID, investigator);
                strMessage = "Data Saved Successfully";
            }
            catch (Exception ex)
            {
                strMessage = ex.Message;
            }
            return Json(new { strMessage });
        }

        [HttpPost]
        public ActionResult UpdateStatus(int incidentID)
        {

            NewIncidentViewModel incidentVM = new NewIncidentViewModel();
            Incident incident = new Incident();
            incident = IncidentBLL.GetIncident(incidentID);
            incident.IncidentID = incidentID;
            incidentVM.Incident = incident;

            return View(incidentVM);


        }

        [HttpPost]
        public ActionResult UpdateIncidentStatus(int IncidentID, int StatusID, string Comments)
        {
            string strMessage = String.Empty;
            try
            {
                IncidentBLL.UpdateIncidentStatus(IncidentID, StatusID, Comments, CurrentUser.UserID);
                strMessage = "Data Saved Successfully";
            }
            catch (Exception ex)
            {
                strMessage = ex.Message;
            }
            return Json(new { strMessage });
        }


        [HttpGet]
        public ActionResult Observations(int incidentID = 0, int ObservationID = 0)
        {
            IncidentObservationViewModel insObservationVM = new IncidentObservationViewModel();
            List<ObservationViewModel> obsVM = new List<ObservationViewModel>();

            IncidentObserverViewModel observers = new IncidentObserverViewModel();
            insObservationVM.CurrentSessionID = CurrentUser.CurrentSessionID;
            insObservationVM.PrevoiusSessionID = sess.SessionActive;
            if (insObservationVM.CurrentSessionID == insObservationVM.PrevoiusSessionID)
            {
            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";
            }

            List<CAPAObservationStatus> obstatus = new List<CAPAObservationStatus>();
            List<ClassficationFactor> factorList = new List<ClassficationFactor>();

            List<CAPAPriority> priority = new List<CAPAPriority>();

            IncidentObservation insObservation = new IncidentObservation();

            Incident incident = new Incident();
            incident = IncidentBLL.GetIncident(incidentID);

            obstatus = capabll.GetCAPAObservationStatus();
            var capstatus = obstatus.Where(y => (y.ID == -1 || y.ID == 4 || y.ID == 1)).ToList();
            var dept = capabll.GetAllManager();
            //var DeptManager = dept.Where(y => y.ID != 0).ToList();

            var action = IncidentBLL.GetActionList();
            //var Action = action.Where(y => y.UserID != 0).ToList();
            insObservationVM.IncidentNo = incident.IncidentNO;
            insObservationVM.IncidentTitle = incident.Title;
            insObservationVM.IncidentDetail = incident.Description;
            insObservationVM.IncidentPlant = incident.PlantName;
            insObservationVM.StatusID = incident.StatusID;
            obsVM = IncidentBLL.GetObservations(incidentID, ObservationID);

            //ViewBag.RootCause = new SelectList(rootcause, "ID", "Name");

            insObservationVM.Roles = CurrentUser.Roles;
            foreach (var item in insObservationVM.Roles)
            {
                insObservationVM.RoleID = item.RoleID;
            }
            insObservationVM.UserFullName = CurrentUser.FullName;
            insObservationVM.ProfileImage = CurrentUser.ProfileImage;

            factorList = IncidentBLL.GetClassficationFactor();

            priority = IncidentBLL.GetCAPAPriorities();
            var cppriority = priority.Where(y => y.ID != -1).ToList();

            insObservation.PriorityList = cppriority;

            observers = IncidentBLL.GetIncidentLead(incidentID);

            insObservationVM.observers = observers;
            insObservation.observationstatuslist = capstatus;
            insObservation.ActionList = action;
            insObservation.IncidentID = incidentID;
            insObservation.TargetDate = DateTime.Today.AddDays(30).ToString("dd/MM/yyyy");
            insObservation.CompletedDate = null;

            insObservationVM.CurrentUser = CurrentUser.UserID;

            insObservationVM.IsRestrict = CurrentUser.IsRestrict;
            insObservationVM.DeptManagerList = dept;
            insObservationVM.ObservationViewModelList = obsVM;

            insObservationVM.Observation = insObservation;
            foreach (var a in obsVM)
            {
                if (CurrentUser.UserID == a.ActionBy)
                {
                    insObservationVM.actionerid = a.ActionBy;
                }

            }
            return PartialView(insObservationVM);
        }


        [HttpPost]
        public ActionResult EditObservation(int observationID)
        {
            IncObservationViewModel insObservation = new IncObservationViewModel();

            insObservation = IncidentBLL.GetObservation(observationID);
            if (insObservation.incidentObservation.CompletedBy == CurrentUser.UserID)
            {
                insObservation.incidentObservation.CompletedDate = DateTime.Now.ToString("dd/MM/yyyy");
            }

            return Json(new { insObservation });
            //return View(insObservation);
        }



        public ActionResult SaveObservations(IncidentObservation insObservation)
        {
            insObservation.CurrentUser = CurrentUser.UserID;
            if (Request.Files.Count > 0)
            {
                HttpFileCollectionBase files = Request.Files;

                insObservation.InciObAttachment = files[0];
                string fileName = insObservation.InciObAttachment.FileName;

                // create the uploads folder if it doesn't exist

                string path = Path.Combine(Server.MapPath("~/IncidentRecomAttachment/"), fileName);

                // save the file
                insObservation.InciObAttachment.SaveAs(path);

                IncidentBLL.SaveObservation(insObservation);
            }
            else
            {
                IncidentBLL.SaveObservation(insObservation);
            }

            return View();
        }

        public ActionResult AllIncidentObservation()
        {
            AllIncidentObservationListModel allincidentobservationlistmodel = new AllIncidentObservationListModel();
            allincidentobservationlistmodel.CurrentSessionID = CurrentUser.CurrentSessionID;
            allincidentobservationlistmodel.PrevoiusSessionID = sess.SessionActive;
            if (allincidentobservationlistmodel.CurrentSessionID == allincidentobservationlistmodel.PrevoiusSessionID)
            {
            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";
            }
            List<ObservationViewModel> obsVM = new List<ObservationViewModel>();
            obsVM = IncidentBLL.GetAllIncidentObservation();
            var trackStatus = TrackActionStatus.Where(y => y.ID != 5 && y.ID != 6 && y.ID != 4).ToList();
            ViewBag.IncidentStatus = new SelectList(trackStatus, "ID", "Name");
            ViewBag.IncidentTypes = new SelectList(IncidentTypes, "ID", "Name");
            ViewBag.IncidentPlant = new SelectList(IncidentPlants, "ID", "Name");
            ViewBag.UserList = new SelectList(UserProfiles, "UserID", "DisplayUserName");
            allincidentobservationlistmodel.Roles = CurrentUser.Roles;
            allincidentobservationlistmodel.UserFullName = CurrentUser.FullName;
            allincidentobservationlistmodel.ProfileImage = CurrentUser.ProfileImage;

            allincidentobservationlistmodel.IsRestrict = CurrentUser.IsRestrict;
            var DeptManag = capabll.GetAllManager();
            //allincidentobservationlistmodel.CurrentUser = CurrentUser.UserID;
            allincidentobservationlistmodel.ObservationViewModelList1 = obsVM;
            IncidentSearchViewModel incidentSearchViewModel = new IncidentSearchViewModel();
            ViewBag.fromdate = incidentSearchViewModel.IncidentFromDate;
            ViewBag.Todate = incidentSearchViewModel.IncidentToDate;
            ViewBag.PlantID = incidentSearchViewModel.IncidentPlant;
            ViewBag.IncidentType = incidentSearchViewModel.IncidentType;
            ViewBag.IncidentStatus1 = incidentSearchViewModel.IncidentStatus;
            ViewBag.Actioner = incidentSearchViewModel.ActionerID;
            ViewBag.DeptID = incidentSearchViewModel.DeptManger;
            allincidentobservationlistmodel.DeptManagerList = DeptManag;
            return View(allincidentobservationlistmodel);

        }
        [HttpPost]
        public ActionResult AllIncidentObservation(IncidentSearchViewModel incidentSearchViewModel)
        {
            AllIncidentObservationListModel allincidentobservationlistmodel = new AllIncidentObservationListModel();

            List<IncidentViewModel> incidentlist = new List<IncidentViewModel>();

            allincidentobservationlistmodel.ObservationViewModelList1 = IncidentBLL.SearchOpenIncidentsForObservation(incidentSearchViewModel);

            allincidentobservationlistmodel.IncidentSearchVM = incidentSearchViewModel;
            var trackStatus = TrackActionStatus.Where(y => y.ID != 5 && y.ID != 6 && y.ID != 4).ToList();
            ViewBag.IncidentStatus = new SelectList(trackStatus, "ID", "Name");
            ViewBag.IncidentTypes = new SelectList(IncidentTypes, "ID", "Name");
            ViewBag.IncidentPlant = new SelectList(IncidentPlants, "ID", "Name");
            ViewBag.UserList = new SelectList(UserProfiles, "UserID", "DisplayUserName");
            var DeptManag = capabll.GetAllManager();
            allincidentobservationlistmodel.Roles = CurrentUser.Roles;
            allincidentobservationlistmodel.UserFullName = CurrentUser.FullName;
            allincidentobservationlistmodel.ProfileImage = CurrentUser.ProfileImage;

            allincidentobservationlistmodel.IsRestrict = CurrentUser.IsRestrict;
            allincidentobservationlistmodel.IncidentSearchVM.IncidentFromDate = incidentSearchViewModel.IncidentFromDate;
            allincidentobservationlistmodel.IncidentSearchVM.IncidentToDate = incidentSearchViewModel.IncidentFromDate;
            allincidentobservationlistmodel.IncidentSearchVM.IncidentPlant = incidentSearchViewModel.IncidentPlant;
            allincidentobservationlistmodel.IncidentSearchVM.IncidentType = incidentSearchViewModel.IncidentType;
            allincidentobservationlistmodel.IncidentSearchVM.IncidentStatus = incidentSearchViewModel.IncidentStatus;
            allincidentobservationlistmodel.IncidentSearchVM.ActionerID = incidentSearchViewModel.ActionerID;
            allincidentobservationlistmodel.IncidentSearchVM.DeptManger = incidentSearchViewModel.DeptManger;

            ViewBag.fromdate = incidentSearchViewModel.IncidentFromDate;
            ViewBag.Todate = incidentSearchViewModel.IncidentToDate;
            ViewBag.PlantID = incidentSearchViewModel.IncidentPlant;
            ViewBag.IncidentType = incidentSearchViewModel.IncidentType;
            ViewBag.IncidentStatus1 = incidentSearchViewModel.IncidentStatus;
            ViewBag.Actioner = incidentSearchViewModel.ActionerID;
            ViewBag.DeptID = incidentSearchViewModel.DeptManger;
            allincidentobservationlistmodel.DeptManagerList = DeptManag;
            return PartialView(allincidentobservationlistmodel);

        }
        public ActionResult ExportAllObservation(string currentFromDate, string currentEndDate, int ActionerID, int DeptID, int IncidentPlantID, int incidentstatus, int incidenttype)
        {


            using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "ExportAllObservation";
                objCom.CommandType = CommandType.StoredProcedure;
                objCom.Parameters.AddWithValue("@FromDate", currentFromDate == null ? string.Empty : currentFromDate);
                objCom.Parameters.AddWithValue("@ToDate", currentEndDate == null ? string.Empty : currentEndDate);

                objCom.Parameters.AddWithValue("@IncidentStatus", incidentstatus);
                objCom.Parameters.AddWithValue("@IncidentType", incidenttype);
                objCom.Parameters.AddWithValue("@IncidentPlant", IncidentPlantID);
                objCom.Parameters.AddWithValue("@ActionBy", ActionerID);
                objCom.Parameters.AddWithValue("@DeptID", DeptID);
                objCon.Open();
                objCom.Connection = objCon;
                SqlDataAdapter Adapter = new SqlDataAdapter();
                Adapter.SelectCommand = objCom;
                DataSet dataSet = new DataSet();
                Adapter.Fill(dataSet);
                objCon.Close();
                var wb = new XLWorkbook(Server.MapPath("~/Template/TrackActionIncident.xlsx"));
                var worksheet = wb.Worksheet("Incident recommendations list");
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
                Response.AddHeader("content-disposition", "attachment;filename=IncidentTrackAction.xlsx");
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
        public ActionResult DeleteWhyTreeImage(int incidentID)
        {

            IncidentBLL.DeleteWhyTreeImage(incidentID);
            string strMessage = "Successfully Deleted";
            return Json(new { strMessage });
        }


        public ActionResult IncidentPdf(int id)
        {
            try
            {

                using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[IncidentPDF]";
                    objCom.Parameters.AddWithValue("@InciID", id);
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
                        "filename=Incident.pdf");
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);


                    iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 20f, 20f, 30f, 30f);

                    PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                    pdfDoc.Open();


                    if ((dataSet.Tables[0].Rows[0][1].ToString() != "Closed") && (dataSet.Tables[0].Rows[0][1].ToString() != "Approved"))
                    {
                        string imagePath = Server.MapPath("~/Images/watermark.png");
                        iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imagePath);

                        image.ScalePercent(200f);
                        image.RotationDegrees = 45f;
                        image.SetAbsolutePosition(0f, 200f);

                        pdfDoc.Add(image);
                    }

                    PdfPTable TitleTable = new PdfPTable(3);
                    TitleTable.LockedWidth = true;
                    TitleTable.SetWidths(new float[] { 8f, 13f, 8f });
                    //TitleTable.SpacingBefore = 10f;
                    //TitleTable.SpacingAfter = 1f;
                    TitleTable.TotalWidth = 555f;

                    PdfPCell Wpcell = new PdfPCell();
                    string imageURL = Server.MapPath("~/Images/SASALogo.png");
                    iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(imageURL);
                    gif.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
                    gif.SpacingBefore = 10f;

                    gif.ScaleAbsolute(150f, 45f);
                    Wpcell = new PdfPCell(gif);

                    TitleTable.AddCell(Wpcell);


                    var phrase = new Phrase("\n Incident Investigation Report (IIR)".PadRight(1000), FontFactory.GetFont("Times New Roman", 15, iTextSharp.text.Font.BOLD));
                    phrase.Add(new Chunk("\n\n\n", FontFactory.GetFont("Times New Roman", 13, iTextSharp.text.Font.BOLD)));
                    Wpcell.HorizontalAlignment = Element.ALIGN_CENTER;
                    TitleTable.AddCell(PhraseCell(phrase, PdfPCell.ALIGN_CENTER));


                    Wpcell = new PdfPCell(new Phrase("\n Incident ID:".PadRight(5) + dataSet.Tables[0].Rows[0][29].ToString(), FontFactory.GetFont("Times New Roman", 14, iTextSharp.text.Font.BOLD)));
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

                    PdfPTable Details = new PdfPTable(4);
                    Details.TotalWidth = 555f;
                    Details.LockedWidth = true;
                    Details.SetWidths(new float[] { 2f, 14f, 7f, 7f });
                    Details.AddCell(PhraseCell(new Phrase("1", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Details.AddCell(PhraseCell(new Phrase("Preliminary Incident Information ", FontFactory.GetFont("Times New Roman", 11, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Details.AddCell(PhraseCell(new Phrase("Status ", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    Details.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][1].ToString(), FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    pdfDoc.Add(Details);

                    PdfPTable tablesender = new PdfPTable(5);
                    tablesender.TotalWidth = 555f;
                    tablesender.LockedWidth = true;
                    tablesender.SetWidths(new float[] { 2f, 6f, 8f, 7f, 7f });


                    tablesender.AddCell(PhraseCell(new Phrase("1.1", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("Plant / Area", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][2].ToString(), FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("Incident Classification", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    tablesender.AddCell(PhraseCell(new Phrase("" + dataSet.Tables[0].Rows[0][3].ToString(), FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(tablesender);


                    PdfPTable Incident = new PdfPTable(5);
                    Incident.TotalWidth = 555f;
                    Incident.LockedWidth = true;
                    Incident.SetWidths(new float[] { 2f, 6f, 8f, 7f, 7f });
                    var font8 = FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD);
                    var font9 = FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL);
                    var font7 = FontFactory.GetFont("Times New Roman", 11, iTextSharp.text.Font.BOLD);
                    var font6 = FontFactory.GetFont("Times New Roman", 8, iTextSharp.text.Font.NORMAL);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("1.2", font8)));
                    Wpcell.SetLeading(3.0f, 1.0f);
                    Incident.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Incident Description", font8)));
                    Wpcell.SetLeading(3.0f, 1.0f);
                    Incident.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[0].Rows[0][4].ToString(), font9)));
                    Wpcell.SetLeading(3.0f, 1.0f);

                    Wpcell.Colspan = 3;
                    Incident.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1.3", font8)));
                    Wpcell.SetLeading(3.0f, 1.0f);
                    Incident.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Preliminary Incident Detail", font8)));
                    Wpcell.SetLeading(3.0f, 1.0f);
                    Incident.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[0].Rows[0][5].ToString(), font9)));
                    Wpcell.SetLeading(3.0f, 1.0f);
                    Wpcell.Colspan = 3;
                    Incident.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1.4", font8)));
                    Incident.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Incident Time", font8)));
                    Incident.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[0].Rows[0][6].ToString(), font9)));
                    Incident.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Created Time", font8)));
                    Incident.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[0].Rows[0][7].ToString(), font9)));
                    Incident.AddCell(Wpcell);


                    Wpcell = new PdfPCell(new Phrase(new Chunk("1.5", font8)));
                    Incident.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Reported by", font8)));
                    Incident.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[0].Rows[0][8].ToString(), font9)));
                    Incident.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("FIR Created by", font8)));
                    Incident.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[0].Rows[0][9].ToString(), font9)));
                    Incident.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1.6", font8)));
                    Wpcell.SetLeading(3.0f, 1.0f);
                    Incident.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Is anyone injured? If Yes, give details", font8)));
                    Wpcell.SetLeading(3.0f, 1.0f);
                    Incident.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[0].Rows[0][10].ToString(), font9)));
                    Wpcell.SetLeading(3.0f, 1.0f);
                    Incident.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[0].Rows[0][11].ToString(), font9)));
                    Wpcell.SetLeading(3.0f, 1.0f);
                    Wpcell.Colspan = 2;
                    Incident.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1.7", font8)));
                    Wpcell.SetLeading(3.0f, 1.0f);
                    Incident.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Is loss of Material? If Yes, give details", font8)));
                    Wpcell.SetLeading(3.0f, 1.0f);
                    Incident.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[0].Rows[0][12].ToString(), font9)));
                    Wpcell.SetLeading(3.0f, 1.0f);
                    Incident.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[0].Rows[0][13].ToString(), font9)));
                    Wpcell.SetLeading(3.0f, 1.0f);
                    Wpcell.Colspan = 2;
                    Incident.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1.8", font8)));
                    Wpcell.SetLeading(3.0f, 1.0f);
                    Incident.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Is damage to equipment? If Yes, give details", font8)));
                    Wpcell.SetLeading(3.0f, 1.0f);
                    Incident.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[0].Rows[0][14].ToString(), font9)));
                    Wpcell.SetLeading(3.0f, 1.0f);
                    Incident.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[0].Rows[0][15].ToString(), font9)));
                    Wpcell.SetLeading(3.0f, 1.0f);
                    Wpcell.Colspan = 2;
                    Incident.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1.9", font8)));
                    Wpcell.SetLeading(3.0f, 1.0f);
                    Incident.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Persons Available during incident (Write name of shift Incharge, Plant Operator, Contractors, others)", font8)));
                    Wpcell.SetLeading(3.0f, 1.0f);
                    Incident.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[0].Rows[0][16].ToString(), font9)));
                    Wpcell.SetLeading(3.0f, 1.0f);
                    Wpcell.Colspan = 3;
                    Incident.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1.10", font8)));
                    Wpcell.SetLeading(3.0f, 1.0f);
                    Incident.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Immediate Actions Taken: (List in logical order, pertinent facts uncovered in the preliminary investigation.)", font8)));
                    Wpcell.SetLeading(3.0f, 1.0f);
                    Incident.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[0].Rows[0][17].ToString(), font9)));
                    Wpcell.SetLeading(3.0f, 1.0f);
                    Wpcell.Colspan = 3;
                    Incident.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1.11", font8)));
                    Wpcell.SetLeading(3.0f, 1.0f);
                    Incident.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Probable Causes(for the incident from preliminary information)", font8)));
                    Wpcell.SetLeading(3.0f, 1.0f);
                    Incident.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[0].Rows[0][18].ToString(), font9)));
                    Wpcell.SetLeading(3.0f, 1.0f);
                    Wpcell.Colspan = 3;
                    Incident.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("1.12", font8)));
                    Incident.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Attachments", font8)));
                    Incident.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Refer annexure, if available", font9)));
                    Incident.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[0].Rows[0][19].ToString(), font9)));
                    Wpcell.Colspan = 2;
                    Incident.AddCell(Wpcell);
                    pdfDoc.Add(Incident);
                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                    PdfPTable Category = new PdfPTable(6);
                    Category.TotalWidth = 555f;
                    Category.LockedWidth = true;
                    Category.SetWidths(new float[] { 2f, 6.2f, 4f, 5f, 7f, 7f });


                    Wpcell = new PdfPCell(new Phrase(new Chunk("2", font8)));
                    Category.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Incident Categorization", font7)));
                    Wpcell.Colspan = 5;
                    Category.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2.1", font8)));
                    Wpcell.SetLeading(3.0f, 1.0f);
                    Category.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Is Investigation Required", font8)));
                    Wpcell.SetLeading(3.0f, 1.0f);
                    Category.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[0].Rows[0][24].ToString(), font9)));
                    Wpcell.SetLeading(3.0f, 1.0f);
                    Wpcell.Colspan = 4;
                    Category.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("2.2", font8)));
                    Category.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Incident Category", font8)));
                    Category.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[0].Rows[0][20].ToString(), font9)));
                    Wpcell.Colspan = 2;
                    Category.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Injury Category ", font8)));
                    Category.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk((dataSet.Tables[1].Rows[0][4].ToString() != "") ? dataSet.Tables[1].Rows[0][4].ToString() : "", font9)));
                    Category.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("2.3", font8)));
                    Wpcell.SetLeading(3.0f, 1.0f);
                    Category.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Methodology (see uploaded images for detail)", font8)));
                    Wpcell.SetLeading(3.0f, 1.0f);
                    Category.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Five Why Analysis?", font8)));
                    Wpcell.SetLeading(3.0f, 1.0f);
                    Category.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[0].Rows[0][22].ToString(), font9)));
                    Wpcell.SetLeading(3.0f, 1.0f);
                    Category.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Why Tree Analysis", font8)));
                    Wpcell.SetLeading(3.0f, 1.0f);
                    Category.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[0].Rows[0][23].ToString(), font9)));
                    Wpcell.SetLeading(3.0f, 1.0f);
                    Category.AddCell(Wpcell);


                    pdfDoc.Add(Category);
                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));


                    PdfPTable Investigation = new PdfPTable(4);
                    Investigation.TotalWidth = 555f;
                    Investigation.LockedWidth = true;
                    Investigation.SetWidths(new float[] { 2f, 10f, 11f, 7f });


                    Wpcell = new PdfPCell(new Phrase(new Chunk("3", font8)));
                    Investigation.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Investigation Team", font7)));
                    Wpcell.Colspan = 3;
                    Investigation.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    Investigation.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Role", font8)));
                    Investigation.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name", font8)));
                    Investigation.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Department", font8)));
                    Investigation.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk("3.1 ", font8)));
                    Investigation.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Incident Owner", font8)));
                    Investigation.AddCell(Wpcell);
                    if (dataSet.Tables[3].Rows.Count > 0)
                    {
                        Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[3].Rows[0][0].ToString(), font9)));
                        Investigation.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[3].Rows[0][1].ToString(), font9)));
                        Investigation.AddCell(Wpcell);
                    }
                    else
                    {
                        Wpcell = new PdfPCell(new Phrase(new Chunk(("Data Not Available"), font9)));
                        Wpcell.Colspan = 2;
                        Investigation.AddCell(Wpcell);
                    }

                    Wpcell = new PdfPCell(new Phrase(new Chunk("3.2 ", font8)));
                    Investigation.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Team Lead ", font8)));
                    Investigation.AddCell(Wpcell);
                    if (dataSet.Tables[4].Rows.Count > 0)
                    {
                        Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[4].Rows[0][0].ToString(), font9)));
                        Investigation.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[4].Rows[0][1].ToString(), font9)));
                        Investigation.AddCell(Wpcell);
                    }
                    else
                    {
                        Wpcell = new PdfPCell(new Phrase(new Chunk(("Data Not Available"), font9)));
                        Wpcell.Colspan = 2;
                        Investigation.AddCell(Wpcell);
                    }

                    Wpcell = new PdfPCell(new Phrase(new Chunk("3.3 ", font8)));
                    Investigation.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Incident Investigation Facilitator", font8)));
                    Investigation.AddCell(Wpcell);
                    if (dataSet.Tables[5].Rows.Count > 0)
                    {
                        Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[5].Rows[0][0].ToString(), font9)));
                        Investigation.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[5].Rows[0][1].ToString(), font9)));
                        Investigation.AddCell(Wpcell);
                    }
                    else
                    {

                        Wpcell = new PdfPCell(new Phrase(new Chunk(("Data Not Available"), font9)));
                        Wpcell.Colspan = 2;
                        Investigation.AddCell(Wpcell);
                    }

                    Wpcell = new PdfPCell(new Phrase(new Chunk("3.4 ", font8)));
                    Investigation.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Team Members", font8)));
                    Investigation.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Name", font8)));
                    Investigation.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Department", font8)));
                    Investigation.AddCell(Wpcell);
                    if (dataSet.Tables[6].Rows.Count > 0)
                    {
                        for (int rows = 0; rows < dataSet.Tables[6].Rows.Count; rows++)
                        {
                            for (int column = 0; column < dataSet.Tables[6].Columns.Count; column++)
                            {
                                Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[6].Rows[rows][column].ToString(), font9)));
                                Investigation.AddCell(Wpcell);
                            }
                        }
                    }

                    else
                    {
                        for (int column = 0; column < dataSet.Tables[6].Columns.Count; column++)
                        {
                            Wpcell = new PdfPCell(new Phrase(new Chunk("Data Not Available", font9)));
                            Wpcell.Colspan = 2;
                            Investigation.AddCell(Wpcell);
                        }
                    }

                    pdfDoc.Add(Investigation); // add pdf table to the document

                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                    PdfPTable summary = new PdfPTable(2);
                    summary.TotalWidth = 555f;
                    summary.LockedWidth = true;

                    //summary.SpacingBefore = 10f;
                    //summary.SpacingAfter = 1f;
                    summary.SetWidths(new float[] { 2f, 28f });
                    Wpcell = new PdfPCell(new Phrase(new Chunk("4", font8)));
                    summary.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Summary of Incident Details (with date and time)", font7)));
                    summary.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    summary.AddCell(Wpcell);

                    Wpcell = new PdfPCell(new Phrase(new Chunk((dataSet.Tables[7].Rows[0][0].ToString() != "") ? dataSet.Tables[7].Rows[0][0].ToString() : "Data Not Available", font9)));
                    //Wpcell.SetLeading(3.0f, 1.0f);

                    //UI changes based to reflect in pdf like bold, italic, underline

                    string htmlText = dataSet.Tables[7].Rows[0][0].ToString();
                    int len = htmlText.Length;
                    using (StringReader sr = new StringReader(htmlText))
                    {
                        List<IElement> elements = iTextSharp.text.html.simpleparser.HTMLWorker.ParseToList(sr, null);
                        foreach (IElement e in elements)
                        {
                            //Add those elements to the paragraph
                            Wpcell.AddElement(e);
                        }
                    }
                    summary.SplitLate = false;  //help to show continue page

                    summary.AddCell(Wpcell);

                    pdfDoc.Add(summary); // add pdf table to the document

                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                    PdfPTable whyform = new PdfPTable(3);
                    whyform.TotalWidth = 555f;
                    whyform.LockedWidth = true;
                    whyform.KeepTogether = true;
                    whyform.SetWidths(new float[] { 2f, 10f, 18f });
                    Wpcell = new PdfPCell(new Phrase(new Chunk("5", font8)));

                    whyform.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Five Why Ananlysis ", font8)));

                    Wpcell.Colspan = 2;
                    whyform.AddCell(Wpcell);

                    if (dataSet.Tables[8].Rows.Count > 0)
                    {
                        Wpcell = new PdfPCell(new Phrase(new Chunk("1", font8)));
                        Wpcell.SetLeading(3.0f, 1.0f);
                        whyform.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("Why the incident happen?", font8)));
                        Wpcell.SetLeading(3.0f, 1.0f);
                        whyform.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk((dataSet.Tables[8].Rows[0][1].ToString() != "") ? dataSet.Tables[8].Rows[0][1].ToString() : "", font9)));
                        Wpcell.SetLeading(3.0f, 1.0f);
                        whyform.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("2", font8)));
                        Wpcell.SetLeading(3.0f, 1.0f);
                        whyform.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("Why did 1 happen?", font8)));
                        Wpcell.SetLeading(3.0f, 1.0f);
                        whyform.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk((dataSet.Tables[8].Rows[0][2].ToString() != "") ? dataSet.Tables[8].Rows[0][2].ToString() : "", font9)));
                        Wpcell.SetLeading(3.0f, 1.0f);
                        whyform.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("3", font8)));
                        Wpcell.SetLeading(3.0f, 1.0f);
                        whyform.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("Why did 2 happen?", font8)));
                        Wpcell.SetLeading(3.0f, 1.0f);
                        whyform.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk((dataSet.Tables[8].Rows[0][3].ToString() != "") ? dataSet.Tables[8].Rows[0][3].ToString() : "", font9)));
                        Wpcell.SetLeading(3.0f, 1.0f);
                        whyform.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("4", font8)));
                        Wpcell.SetLeading(3.0f, 1.0f);
                        whyform.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("Why did 3 happen?", font8)));
                        Wpcell.SetLeading(3.0f, 1.0f);
                        whyform.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk((dataSet.Tables[8].Rows[0][4].ToString() != "") ? dataSet.Tables[8].Rows[0][4].ToString() : "", font9)));
                        Wpcell.SetLeading(3.0f, 1.0f);
                        whyform.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("5", font8)));
                        Wpcell.SetLeading(3.0f, 1.0f);
                        whyform.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("Why did 4 happen?", font8)));
                        Wpcell.SetLeading(3.0f, 1.0f);
                        whyform.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk((dataSet.Tables[8].Rows[0][5].ToString() != "") ? dataSet.Tables[8].Rows[0][5].ToString() : "", font9)));
                        Wpcell.SetLeading(3.0f, 1.0f);
                        whyform.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("6", font8)));
                        Wpcell.SetLeading(3.0f, 1.0f);
                        whyform.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("Why did 5 happen?", font8)));
                        Wpcell.SetLeading(3.0f, 1.0f);
                        whyform.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk((dataSet.Tables[8].Rows[0][6].ToString() != "") ? dataSet.Tables[8].Rows[0][6].ToString() : "", font9)));
                        Wpcell.SetLeading(3.0f, 1.0f);
                        whyform.AddCell(Wpcell);
                    }
                    else
                    {
                        Wpcell = new PdfPCell(new Phrase(new Chunk((""), font9)));
                        whyform.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk(("Data Not Applicable"), font9)));
                        Wpcell.Rowspan = 6;
                        Wpcell.Colspan = 2;
                        whyform.AddCell(Wpcell);
                    }
                    pdfDoc.Add(whyform);
                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));



                    PdfPTable protective = new PdfPTable(3);
                    protective.TotalWidth = 555f;
                    protective.LockedWidth = true;

                    protective.SetWidths(new float[] { 2f, 15f, 13f });
                    Wpcell = new PdfPCell(new Phrase(new Chunk("6", font8)));
                    protective.AddCell(Wpcell);
                    protective.KeepTogether = true;
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Protective Systems", font7)));
                    Wpcell.Colspan = 2;
                    protective.AddCell(Wpcell);

                    if (dataSet.Tables[10].Rows.Count > 0)
                    {
                        Wpcell = new PdfPCell(new Phrase(new Chunk("6.1", font8)));
                        Wpcell.SetLeading(3.0f, 1.0f);
                        protective.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("What exists and work?", font8)));
                        Wpcell.SetLeading(3.0f, 1.0f);
                        protective.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk((dataSet.Tables[10].Rows[0][0].ToString() != "") ? dataSet.Tables[10].Rows[0][0].ToString() : "Data Not Available", font9)));
                        Wpcell.SetLeading(3.0f, 1.0f);
                        protective.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("6.2", font8)));
                        Wpcell.SetLeading(3.0f, 1.0f);
                        protective.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("What exists and did not work ?", font8)));
                        Wpcell.SetLeading(3.0f, 1.0f);
                        protective.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk((dataSet.Tables[10].Rows[0][1].ToString() != "") ? dataSet.Tables[10].Rows[0][1].ToString() : "Data Not Available", font9)));
                        Wpcell.SetLeading(3.0f, 1.0f);
                        protective.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("6.3", font8)));
                        Wpcell.SetLeading(3.0f, 1.0f);
                        protective.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("What would have helped?", font8)));
                        Wpcell.SetLeading(3.0f, 1.0f);
                        protective.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk((dataSet.Tables[10].Rows[0][2].ToString() != "") ? dataSet.Tables[10].Rows[0][2].ToString() : "Data Not Available", font9)));
                        Wpcell.SetLeading(3.0f, 1.0f);
                        protective.AddCell(Wpcell);

                    }
                    else
                    {
                        Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                        protective.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("Data Not Applicable", font9)));
                        Wpcell.Colspan = 2;
                        protective.AddCell(Wpcell);
                    }
                    pdfDoc.Add(protective);
                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                    PdfPTable summaryfind = new PdfPTable(2);
                    summaryfind.TotalWidth = 555f;
                    summaryfind.KeepTogether = true;
                    summaryfind.LockedWidth = true;
                    summaryfind.SetWidths(new float[] { 2f, 28f });

                    Wpcell = new PdfPCell(new Phrase(new Chunk("7", font8)));
                    summaryfind.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Summary of Incident Investigation findings", font7)));

                    summaryfind.AddCell(Wpcell);

                    if (dataSet.Tables[10].Rows.Count > 0)
                    {
                        Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                        summaryfind.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk((dataSet.Tables[10].Rows[0][3].ToString() != "") ? dataSet.Tables[10].Rows[0][3].ToString() : "Data Not Available", font9)));
                        Wpcell.SetLeading(3.0f, 1.0f);
                        summaryfind.AddCell(Wpcell);

                    }
                    else
                    {
                        Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                        summaryfind.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("Data Not Applicable", font9)));
                        summaryfind.AddCell(Wpcell);

                    }
                    pdfDoc.Add(summaryfind);

                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                    PdfPTable rootcause = new PdfPTable(2);
                    rootcause.TotalWidth = 555f;
                    rootcause.LockedWidth = true;
                    rootcause.KeepTogether = true;
                    rootcause.SetWidths(new float[] { 2f, 28f });
                    Wpcell = new PdfPCell(new Phrase(new Chunk("8.1", font8)));
                    rootcause.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Root Cause Analysis", font8)));
                    rootcause.AddCell(Wpcell);
                    rootcause.SpacingAfter = 0;
                    pdfDoc.Add(rootcause);

                    PdfPTable rootcause1 = new PdfPTable(3);
                    rootcause1.TotalWidth = 555f;
                    rootcause1.LockedWidth = true;

                    rootcause1.SetWidths(new float[] { 2f, 14f, 14f });
                    Wpcell = new PdfPCell(new Phrase(new Chunk("ID", font8)));
                    rootcause1.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Root Cause Categories", font8)));
                    rootcause1.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Sub Categories", font8)));
                    rootcause1.AddCell(Wpcell);


                    if (dataSet.Tables[13].Rows.Count > 0)
                    {
                        for (int rows = 0; rows < dataSet.Tables[13].Rows.Count; rows++)
                        {
                            for (int column = 0; column < dataSet.Tables[13].Columns.Count; column++)
                            {
                                Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[13].Rows[rows][column].ToString(), font9)));
                                Wpcell.SetLeading(3.0f, 1.0f);
                                rootcause1.AddCell(Wpcell);
                            }
                        }
                    }

                    else
                    {
                        for (int column = 0; column < dataSet.Tables[13].Columns.Count; column++)
                        {
                            Wpcell = new PdfPCell(new Phrase(new Chunk("Data Not Available", font9)));

                            rootcause1.AddCell(Wpcell);
                        }
                    }

                    rootcause1.SpacingBefore = 0;
                    pdfDoc.Add(rootcause1);

                    PdfPTable rootcause2 = new PdfPTable(3);
                    rootcause2.TotalWidth = 555f;
                    rootcause2.LockedWidth = true;
                    rootcause2.KeepTogether = true;
                    rootcause2.SetWidths(new float[] { 2f, 14f, 14f });
                    Wpcell = new PdfPCell(new Phrase(new Chunk("8.2", font8)));
                    rootcause2.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Reason for selecting the Rootcause", font8)));
                    rootcause2.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk((dataSet.Tables[0].Rows[0][28].ToString() != "") ? dataSet.Tables[0].Rows[0][28].ToString() : "Data Not Available", font9)));
                    Wpcell.SetLeading(3.0f, 1.0f);
                    rootcause2.AddCell(Wpcell);
                    pdfDoc.Add(rootcause2);

                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));


                    //pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));
                    PdfPTable lessons = new PdfPTable(2);
                    lessons.TotalWidth = 555f;
                    lessons.LockedWidth = true;
                    lessons.KeepTogether = true;
                    lessons.SetWidths(new float[] { 2f, 28f });

                    Wpcell = new PdfPCell(new Phrase(new Chunk("9", font8)));
                    lessons.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Lessons Learnt", font7)));

                    lessons.AddCell(Wpcell);

                    if (dataSet.Tables[10].Rows.Count > 0)
                    {
                        Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                        lessons.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk((dataSet.Tables[10].Rows[0][4].ToString() != "") ? dataSet.Tables[10].Rows[0][4].ToString() : "Data Not Available", font9)));
                        Wpcell.SetLeading(3.0f, 1.0f);
                        lessons.AddCell(Wpcell);

                    }
                    else
                    {
                        Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                        lessons.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("Data Not Applicable", font9)));
                        lessons.AddCell(Wpcell);

                    }
                    pdfDoc.Add(lessons);

                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));


                    PdfPTable Supimg = new PdfPTable(2);
                    Supimg.TotalWidth = 555f;
                    Supimg.LockedWidth = true;
                    Supimg.KeepTogether = true;
                    Supimg.SetWidths(new float[] { 2f, 28f });
                    Wpcell = new PdfPCell(new Phrase(new Chunk("10.1", font8)));
                    Supimg.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Supporting Documents/pictures", font8)));
                    Supimg.AddCell(Wpcell);
                    //    Supimg.SpacingAfter = 0;



                    if (dataSet.Tables[12].Rows.Count > 0)
                    {
                        for (int rows = 0; rows < dataSet.Tables[12].Rows.Count; rows++)
                        {

                            string imagePath = Server.MapPath("~/UploadImages/") + Path.GetFileName(dataSet.Tables[12].Rows[rows][2].ToString());

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
                            Supimg.AddCell(Wpcell);

                            pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                            Wpcell = new PdfPCell(new Phrase(new Chunk("Picture " + dataSet.Tables[12].Rows[rows][0].ToString() + " : " + dataSet.Tables[12].Rows[rows][1].ToString(), font9)));
                            Wpcell.Colspan = 2;
                            Supimg.AddCell(Wpcell);
                        }
                    }
                    else
                    {


                        Wpcell = new PdfPCell(new Phrase(new Chunk("Data Not Available", font9)));
                        Wpcell.Colspan = 2;
                        Supimg.AddCell(Wpcell);

                    }
                    //   Supimg.SpacingBefore = 0;
                    Wpcell.Colspan = 2;
                    pdfDoc.Add(Supimg);



                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                    PdfPTable whytree = new PdfPTable(2);
                    if (dataSet.Tables[9].Rows.Count > 0)
                    {
                        whytree.TotalWidth = 750f;
                    }
                    else
                    {
                        whytree.TotalWidth = 555f;
                    }
                    whytree.LockedWidth = true;
                    whytree.KeepTogether = true;
                    whytree.SetWidths(new float[] { 2f, 28f });
                    Wpcell = new PdfPCell(new Phrase(new Chunk("10.2", font8)));
                    whytree.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Why Tree Analysis. Refer Attachment", font8)));
                    whytree.AddCell(Wpcell);


                    if (dataSet.Tables[9].Rows.Count > 0)
                    {
                        for (int rows = 0; rows < dataSet.Tables[9].Rows.Count; rows++)
                        {

                            string imagePath1 = Server.MapPath("~/InvestigationAttachments/") + Path.GetFileName(dataSet.Tables[9].Rows[rows][0].ToString());

                            iTextSharp.text.Image image1 = iTextSharp.text.Image.GetInstance(imagePath1);
                            //float fixedHeight = 500f; // set the desired height in points
                            //float aspectRatio = image1.Width / image1.Height;
                            //float fixedWidth = fixedHeight * aspectRatio;
                            //image1.ScaleToFit(fixedWidth, fixedHeight);
                            image1.ScaleToFit(730f, 730f);
                            //image1.ScaleToFitHeight = true;
                            image1.SpacingBefore = 10f;
                            image1.SpacingAfter = 10f;
                            Wpcell = new PdfPCell(image1);
                            Wpcell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                            Wpcell.Colspan = 2;
                            whytree.AddCell(Wpcell);
                        }
                    }
                    else
                    {
                        Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                        whytree.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("Data Not Applicable", font9)));
                        whytree.AddCell(Wpcell);

                    }
                    if (dataSet.Tables[9].Rows.Count > 0)
                    {
                        pdfDoc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());

                        pdfDoc.NewPage();
                    }
                    pdfDoc.Add(whytree);

                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                    PdfPTable fishbone = new PdfPTable(2);
                    fishbone.TotalWidth = 750f;
                    fishbone.LockedWidth = true;
                    fishbone.KeepTogether = true;
                    fishbone.SetWidths(new float[] { 2f, 28f });
                    Wpcell = new PdfPCell(new Phrase(new Chunk("10.3", font8)));
                    fishbone.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("FishBone Diagram", font8)));
                    fishbone.AddCell(Wpcell);


                    if (dataSet.Tables[14].Rows.Count > 0)
                    {


                        string imagePath1 = Server.MapPath("~/FishBoneDiagram/") + Path.GetFileName(dataSet.Tables[14].Rows[0][0].ToString());

                        iTextSharp.text.Image image1 = iTextSharp.text.Image.GetInstance(imagePath1);

                        image1.ScaleToFit(730f, 730f);
                        image1.SpacingBefore = 10f;
                        image1.SpacingAfter = 50f;
                        Wpcell = new PdfPCell(image1);
                        Wpcell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                        Wpcell.Colspan = 2;
                        fishbone.AddCell(Wpcell);


                    }
                    else
                    {
                        Wpcell = new PdfPCell(new Phrase(new Chunk("", font9)));
                        fishbone.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("Data Not Applicable", font9)));
                        fishbone.AddCell(Wpcell);

                    }
                    pdfDoc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());

                    pdfDoc.NewPage();
                    pdfDoc.Add(fishbone);

                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));

                    PdfPTable recommend = new PdfPTable(10);
                    recommend.TotalWidth = 750f;

                    recommend.LockedWidth = true;
                    recommend.KeepTogether = true;

                    recommend.SetWidths(new float[] { 2f, 3f, 9f, 9f, 5f, 6f, 5f, 5f, 6f, 5f });
                    Wpcell = new PdfPCell(new Phrase(new Chunk("11", font8)));
                    recommend.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Recommendations Status ", font7)));
                    Wpcell.Colspan = 9;
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

                    if (dataSet.Tables[11].Rows.Count > 0)
                    {
                        for (int rows = 0; rows < dataSet.Tables[11].Rows.Count; rows++)
                        {
                            for (int column = 0; column < dataSet.Tables[11].Columns.Count; column++)
                            {
                                Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[11].Rows[rows][column].ToString(), font9)));
                                Wpcell.SetLeading(3.0f, 1.0f);
                                recommend.AddCell(Wpcell);
                            }
                        }
                    }
                    else
                    {
                        for (int column = 0; column < dataSet.Tables[11].Columns.Count; column++)
                        {

                            Wpcell = new PdfPCell(new Phrase(new Chunk("Data Not Available", font9)));
                            recommend.AddCell(Wpcell);
                        }
                    }

                    pdfDoc.Add(recommend);

                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));
                    PdfPTable approve = new PdfPTable(5);
                    approve.TotalWidth = 750f;
                    approve.KeepTogether = true;
                    approve.LockedWidth = true;
                    approve.SetWidths(new float[] { 2f, 6f, 22f, 6f, 22f });

                    Wpcell = new PdfPCell(new Phrase(new Chunk("12", font8)));
                    approve.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Approver/Closed Details", font7)));
                    Wpcell.Colspan = 4;
                    approve.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("12.1", font8)));
                    approve.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Approver comments", font8)));
                    approve.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk((dataSet.Tables[0].Rows[0][25].ToString() != "") ? dataSet.Tables[0].Rows[0][25].ToString() : "", font9)));
                    Wpcell.SetLeading(3.0f, 1.0f);
                    approve.AddCell(Wpcell);
                    approve.AddCell(PhraseCell(new Phrase("Closure Comments", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    approve.AddCell(PhraseCell(new Phrase(("" + dataSet.Tables[1].Rows[0][0].ToString() != "") ? dataSet.Tables[1].Rows[0][0].ToString() : "Data Not Available", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));



                    Wpcell = new PdfPCell(new Phrase(new Chunk("12.2", font8)));
                    approve.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Approved by", font8)));
                    approve.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk((dataSet.Tables[0].Rows[0][26].ToString() != "") ? dataSet.Tables[0].Rows[0][26].ToString() : "", font9)));
                    approve.AddCell(Wpcell);
                    approve.AddCell(PhraseCell(new Phrase("Closed By", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    approve.AddCell(PhraseCell(new Phrase(("" + dataSet.Tables[1].Rows[0][1].ToString() != "") ? dataSet.Tables[1].Rows[0][1].ToString() : "Data Not Available", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    Wpcell = new PdfPCell(new Phrase(new Chunk("12.3", font8)));
                    approve.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk("Approved Date", font8)));
                    approve.AddCell(Wpcell);
                    Wpcell = new PdfPCell(new Phrase(new Chunk((dataSet.Tables[0].Rows[0][27].ToString() != "") ? dataSet.Tables[0].Rows[0][27].ToString() : "", font9)));
                    approve.AddCell(Wpcell);
                    approve.AddCell(PhraseCell(new Phrase("Closed Date", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.BOLD)), PdfPCell.ALIGN_LEFT));
                    approve.AddCell(PhraseCell(new Phrase(("" + dataSet.Tables[1].Rows[0][2].ToString() != "") ? dataSet.Tables[1].Rows[0][2].ToString() : "Data Not Available", FontFactory.GetFont("Times New Roman", 10, iTextSharp.text.Font.NORMAL)), PdfPCell.ALIGN_LEFT));

                    pdfDoc.Add(approve);

                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));
                    //PdfPTable DecisionTree = new PdfPTable(3);
                    //DecisionTree.TotalWidth = 750f;
                    //DecisionTree.KeepTogether = true;
                    //DecisionTree.LockedWidth = true;
                    //DecisionTree.SetWidths(new float[] { 2f, 20f, 15f});
                    //Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    //DecisionTree.AddCell(Wpcell);
                    //Wpcell = new PdfPCell(new Phrase(new Chunk("Annexure 1: Incident Category – Decision Tree", font8)));
                    //Wpcell.Colspan = 2;
                    //DecisionTree.AddCell(Wpcell);
                    //Wpcell = new PdfPCell(new Phrase(new Chunk("S.No", font8)));
                    //DecisionTree.AddCell(Wpcell);
                    //Wpcell = new PdfPCell(new Phrase(new Chunk("Description", font8)));
                    //DecisionTree.AddCell(Wpcell);
                    //Wpcell = new PdfPCell(new Phrase(new Chunk("Remarks", font8)));
                    //DecisionTree.AddCell(Wpcell);
                    
                    //var currectval = 0;
                    //var preval = 0;
                    //if (dataSet.Tables[15].Rows.Count > 0)
                    //{

                    //    for (int rows = 0; rows < dataSet.Tables[15].Rows.Count; rows++)
                    //    {
                    //        int DecisionTypeID = int.Parse(dataSet.Tables[15].Rows[rows][0].ToString());

                    //        currectval = DecisionTypeID;
                    //        if (currectval != preval)
                    //        {
                    //            Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[15].Rows[rows][0].ToString(), font8)));
                    //            DecisionTree.AddCell(Wpcell);
                    //            Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[15].Rows[rows][1].ToString(), font8)));
                    //            DecisionTree.AddCell(Wpcell);
                    //            if (currectval == 1)
                    //            {
                    //                Wpcell = new PdfPCell(new Phrase(new Chunk("Total:\t \t" + dataSet.Tables[18].Rows[0][0].ToString()+"\t \t Result: \t \t" + dataSet.Tables[18].Rows[0][1].ToString(), font8)));
                    //               DecisionTree.AddCell(Wpcell);
                                   
                    //            }
                    //            else if (currectval == 2)
                    //            {
                    //                Wpcell = new PdfPCell(new Phrase(new Chunk("Total:\t\t" + dataSet.Tables[18].Rows[0][2].ToString()+"\t \t Result: \t \t" + dataSet.Tables[18].Rows[0][3].ToString(), font8)));
                    //                DecisionTree.AddCell(Wpcell);
                    //            }
                    //            else if (currectval == 3)
                    //            {
                    //                Wpcell = new PdfPCell(new Phrase(new Chunk("Total:\t \t" + dataSet.Tables[18].Rows[0][4].ToString()+"\t \t Result:\t \t" + dataSet.Tables[18].Rows[0][5].ToString(), font8)));
                    //                DecisionTree.AddCell(Wpcell);
                    //            }
                    //            else if (currectval == 4)
                    //            {
                    //                Wpcell = new PdfPCell(new Phrase(new Chunk("Total:\t \t" + dataSet.Tables[18].Rows[0][6].ToString()+"\t \t Result:\t \t" + dataSet.Tables[18].Rows[0][7].ToString(), font8)));
                    //               DecisionTree.AddCell(Wpcell);
                    //            }
                                   
                            
                    //            preval = currectval;
                    //        }
                    //        var questionid = dataSet.Tables[15].Rows[rows][2].ToString();

                    //        if (questionid == "0")
                    //        {

                    //            Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                    //            DecisionTree.AddCell(Wpcell);
                    //            Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[15].Rows[rows][4].ToString(), font8)));
                    //            Wpcell.Colspan = 2;
                    //            DecisionTree.AddCell(Wpcell);

                    //        }
                    //        for (int column = 3; column < dataSet.Tables[15].Columns.Count; column++)
                    //        {
                              
                    //            if (questionid != "0")
                    //            {
                                    
                                
                    //                Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[15].Rows[rows][column].ToString(), font9)));
                    //                Wpcell.SetLeading(3.0f, 1.0f);
                    //                DecisionTree.AddCell(Wpcell);
                    //            }
                             
                    //        }
                           
                           

                    //    }

                    //}
                    //else
                    //{

                           // Wpcell = new PdfPCell(new Phrase(new Chunk("Data Not Available", font9)));
                           // Wpcell.Colspan = 3;
                           //DecisionTree.AddCell(Wpcell);
                        

                    //}
                    //pdfDoc.Add(DecisionTree);
                    pdfDoc.Add(new iTextSharp.text.Paragraph("".PadRight(100)));
                    if (dataSet.Tables[16].Rows.Count > 0 || dataSet.Tables[17].Rows.Count > 0)
                    {
                        PdfPTable api754 = new PdfPTable(3);
                        api754.TotalWidth = 750f;
                        api754.KeepTogether = true;
                        api754.LockedWidth = true;
                        api754.SetWidths(new float[] { 2f, 20f, 15f});
                        Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                        api754.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("Annexure 1: Incident Category – Decision Tree", font8)));
                        Wpcell.Colspan = 4;
                        api754.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("S.No", font8)));
                        api754.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("Description", font8)));
                        api754.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("User Value", font8)));
                        api754.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("1", font8)));
                        api754.AddCell(Wpcell);
                        Wpcell = new PdfPCell(new Phrase(new Chunk("API 754", font8)));
                        api754.AddCell(Wpcell);
                        if (dataSet.Tables[16].Rows.Count > 0)
                        {
                            
                            Wpcell = new PdfPCell(new Phrase(new Chunk("Result:\t \t" + dataSet.Tables[0].Rows[0][20].ToString(), font8)));
                            Wpcell.Colspan = 3;
                            api754.AddCell(Wpcell);

                            for (int rows = 0; rows < dataSet.Tables[16].Rows.Count; rows++)
                            {
                                for (int column = 0; column < dataSet.Tables[16].Columns.Count; column++)
                                {
                                    Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[16].Rows[rows][column].ToString(), font9)));
                                    Wpcell.SetLeading(3.0f, 1.0f);
                                    api754.AddCell(Wpcell);
                                }
                            }
                        }
                        else
                        {

                                Wpcell = new PdfPCell(new Phrase(new Chunk("Data Not Available", font9)));
                                Wpcell.Colspan = 2;
                                api754.AddCell(Wpcell);
                            

                        }
                        //Wpcell = new PdfPCell(new Phrase(new Chunk("6", font8)));
                        //api754.AddCell(Wpcell);
                        //Wpcell = new PdfPCell(new Phrase(new Chunk("Chemical QTY", font8)));
                        //api754.AddCell(Wpcell);
                        //if (dataSet.Tables[18].Rows.Count > 0)
                        //{
                            
                        //    Wpcell = new PdfPCell(new Phrase(new Chunk("Result:\t \t" + dataSet.Tables[18].Rows[0][10].ToString(), font8)));
                        //    Wpcell.Colspan = 3;
                        //    api754.AddCell(Wpcell);
                        //    Wpcell = new PdfPCell(new Phrase(new Chunk("", font8)));
                        //    api754.AddCell(Wpcell);
                        //    Wpcell = new PdfPCell(new Phrase(new Chunk("Type of Release", font8)));
                        //    api754.AddCell(Wpcell);
                        //    Wpcell = new PdfPCell(new Phrase(new Chunk("" + dataSet.Tables[18].Rows[0][9].ToString(), font9)));
                        //    Wpcell.Colspan = 3;
                        //    api754.AddCell(Wpcell);

                        //    for (int rows = 0; rows < dataSet.Tables[17].Rows.Count; rows++)
                        //    {
                        //        for (int column = 0; column < dataSet.Tables[17].Columns.Count; column++)
                        //        {
                        //            Wpcell = new PdfPCell(new Phrase(new Chunk(dataSet.Tables[17].Rows[rows][column].ToString(), font9)));
                        //            Wpcell.SetLeading(3.0f, 1.0f);
                        //            api754.AddCell(Wpcell);
                        //        }
                        //    }
                        //}
                        //else
                        //{
                           

                        //        Wpcell = new PdfPCell(new Phrase(new Chunk("Data Not Available", font9)));
                        //        Wpcell.Colspan = 2;
                        //        api754.AddCell(Wpcell);
                            

                        //}
                        pdfDoc.Add(api754);
                    }

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
