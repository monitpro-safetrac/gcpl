using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using MonitPro.DAL;
using IncidentReportSystem.Models;
using MonitPro.Models.Common;
using MonitPro.Models.Incident;
using MonitPro.Models.Account;
using MonitPro.Models.IncidentViewModels;
using MonitPro.Common.Library;
using MonitPro.Models.CAPAViewModel;
using MonitPro.Models;
using MonitPro.Models.CAPA;

namespace MonitPro.BLL
{
    public class IncidentReportBLL
    {
        IncidentReportDAL IncidentDAL = new IncidentReportDAL();
        CAPADAL CapaDAL = new CAPADAL();
        AccountDA accountDA = new AccountDA();

        public IncidentReportBLL()
        {

        }

        public List<IncidentType> GetIncidentTypes()
        {
            return IncidentDAL.GetIncidentTypes();
        }
        public CategoryCalculation GetCategoryCalculation(int IncidentID, int DecisionTypeID)
        {
            return IncidentDAL.GetCategoryCalculation(IncidentID, DecisionTypeID);
        }
            public List<IncidentCategoryDecision> GetIncidentCategoryDecisions(int DecisionType, int IncidentID)
        {
            return IncidentDAL.GetIncidentCategoryDecisions(DecisionType,IncidentID);
        }
        public List<ChemicalQTY> GetIncidentChemicalQTYDetails(int IncidentID)
        {
            return IncidentDAL.GetIncidentChemicalQTYDetails(IncidentID);
        }
        public void IncidentCategoryOverallCalculation(int IncidentID)
        {
            IncidentDAL.IncidentCategoryOverallCalculation(IncidentID);
        }
            public int IncidentCategoryInsert(IncidentMaincategoryModel mainmodel)
        {
            return IncidentDAL.IncidentCategoryInsert(mainmodel);
        }
        public int FishBoneInsert(FishBone fish)
        {
            return IncidentDAL.FishBoneInsert(fish);

        }
        public FishBone GetFishBoneDetails(int IncidentID)
        {
            return IncidentDAL.GetFishBoneDetails(IncidentID);
        }
            public List<API754Details> GetAPI754Details(int IncidentID)
        {
            return IncidentDAL.GetAPI754Details(IncidentID);
        }
            public List<Status> GetIncidentStatus()
        {
            return IncidentDAL.GetIncidentStatus();
        }

        public List<IncidentClassfication> GetIncidentClassfication()
        {
            return IncidentDAL.GetIncidentClassfication();
        }

        public List<Plants> GetPlants()
        {
            return IncidentDAL.GetPlants();
        }

        public List<Priority> GetPriorities()
        {
            return IncidentDAL.GetPriorities();
        }
        public List<CAPAPriority> GetCAPAPriorities()
        {
            return CapaDAL.GetCAPAPriority();
        }
        public List<Employee> GetDeptEmployees(int? deptID)
        {

            return IncidentDAL.GetDeptEmployees(deptID);
        }
        public List<InjuryType> GetInjuryTypes()
        {
            return IncidentDAL.GetInjuryTypes();
        }
        public List<CAPAObservationStatus> GetCAPAObservationStatus()
        {
            return CapaDAL.GetCAPAObservationStatus();
        }
        public List<ClassficationFactor> GetClassficationFactor()
        {
            return IncidentDAL.GetClassficationFactor();
        }


        public List<Contractor> GetContractor()
        {
            return IncidentDAL.GetContractor();
        }
        public List<Gender> GetGender()
        {
            return IncidentDAL.GetGender();
        }
        public List<ContractorEmp> GetContractorEmp()
        {
            return IncidentDAL.GetContractorEmp();
        }
        public void DeleteWhyTreeImage(int incidentId)
        {
            IncidentDAL.DeleteWhyTreeImage(incidentId);
        }
        public List<InjureList> GetAllInjuredPersonList(int incidentID)
        {
            return IncidentDAL.GetAllInjuredPersonList(incidentID);
        }
        public int InsertInjuryPersonDetails(NewIncidentViewModel IncidentReport)
        {
            return IncidentDAL.InsertInjuryPersonDetails(IncidentReport);
        }
        public IncidentObserverViewModel GetIncidentLead(int incidentID)
        {
            return IncidentDAL.GetIncidentLead(incidentID);
        }
        public List<Employee> GetInvestigator()
        {
            return IncidentDAL.GetInvestigator();
        }
        public List<Employee> GetIncidentObservers(int incidentID)
        {
            return IncidentDAL.GetIncidentObservers(incidentID);
        }

        public List<ObservationViewModel> GetObservations(int incidentID, int ObservationID)
        {
            return IncidentDAL.GetObservations(incidentID,ObservationID);
        }
        public CreateCAPA GetMyCAPADetails(int ObservID)
        {
            return CapaDAL.GetMyCAPADetails(ObservID);
        }
        public CreateCAPA GetMyInciDetails(int ObservID)
        {
            return CapaDAL.GetMyInciDetails(ObservID);
        }
        public Incident GetIncident(int incidentID)
        {
            return IncidentDAL.GetIncident(incidentID);
        }
        public List<DecisionTypeDD> GetDecisionTypesDD(int IncidentID)
        {
            return IncidentDAL.GetDecisionTypesDD(IncidentID);
        }

            public NewIncidentViewModel GetSelectedRootCause(int incidentID)
        {
            return IncidentDAL.GetSelectedRootCause(incidentID);
        }
        public int Savewhyform(NewIncidentViewModel IncidentReport)
        {
            return IncidentDAL.Savewhyform(IncidentReport);
        }
        public WhyForm GetWhyForm(int incidentID)
        {
            return IncidentDAL.GetWhyForm(incidentID);
        }
        public NewIncidentViewModel GetTenets(int incidentID)
        {
            return IncidentDAL.GetTenets(incidentID);
        }

        public List<TenetsList> SaveTenets(NewIncidentViewModel incidentVM, List<TenetsList> Tenets)
        {
            return IncidentDAL.SaveTenets(incidentVM, Tenets);
        }

        public List<Tenets4> Save4Tenets(NewIncidentViewModel incidentVM, List<Tenets4> Tenets4)
        {
            return IncidentDAL.Save4Tenets(incidentVM, Tenets4);
        }
        public List<UserProfile> GetActionList()
        {
            return CapaDAL.GetActionList();
        }
        public SessionDetails GetSession(int userid)
        {
            return IncidentDAL.GetSession(userid);
        }
        public List<ObservationViewModel> GetMyInciObservation(int InciObservID)
        {
            return IncidentDAL.GetMyInciObservation(InciObservID);
        }
        public IncObservationViewModel GetObservation(int ObservationID)
        {
            return IncidentDAL.GetObservation(ObservationID);
        }

        public void DeleteObservervation(int ObservationID)
        {
            IncidentDAL.DeleteObservervation(ObservationID);
        }

        public List<IncidentViewModel> GetOpenIncidents()
        {
            return IncidentDAL.GetOpenIncidents();
        }
        public List<ObserverTeamModel> GetAllObservations()
        {
            return IncidentDAL.GetAllObservations();
        }
       
        public List<Employee> GetAllGeneralManager()
        {
            return IncidentDAL.GetAllGeneralManager();
        }
        public List<Department> GetDepartmentList()
        {
            AdminDA adminDA = new AdminDA();
            return adminDA.GetDepartmentList();

        }
        public List<IncidentViewModel> SearchOpenIncidents(IncidentSearchViewModel incidentSearchViewModel)
        {
            return IncidentDAL.SearchOpenIncidents(incidentSearchViewModel);
        }
        public List<IncidentViewModel> SearchClosedIncidents(IncidentSearchViewModel incidentSearchViewModel)
        {
            return IncidentDAL.SearchClosedIncidents(incidentSearchViewModel);
        }
            public List<IncidentViewModel> GetAllClosedIncidents()
        {
            return IncidentDAL.GetAllClosedIncidents();
        }

        public List<MonthlyCount> GetMonthlyCount()
        {
            return IncidentDAL.GetMonthlyCount();
        }
        public List<StatusCount> GetStatusCount()
        {
            return IncidentDAL.GetStatusCount();
        }

        public List<ObservationStatusCount> GetRecommenStatusCount()
        {
            return IncidentDAL.GetRecommenStatusCount();
        }

        public List<TypeCount> GetIncidentCategoryCount()
        {
            return IncidentDAL.GetIncidentCategoryCount();
        }

        public List<ActionsCount> GetMyActionStatusCount(int UserID)
        {
            return IncidentDAL.GetMyActionStatusCount(UserID);
        }

        public List<ClassificationCount> GetClassificationCount()
        {
            return IncidentDAL.GetClassificationCount();
        }
        public List<RootCauseCount> GetRootCauseCount()
        {
            return IncidentDAL.GetRootCauseCount();
        }
        #region "get Last monthly count Incident Summary chart"
        public List<MonthlyCount> GetLastMonthlyCount()
        {
            return IncidentDAL.GetLastMonthlyCount();
        }
        #endregion

        #region "get Last monthly  Incident Category chart"
        public List<TypeCount> GetLastMonthIncidentCategory()
        {
            return IncidentDAL.GetLastMonthIncidentCategory();
        }
        #endregion

        #region "get Last monthly  Incident Category chart"
        public List<ClassificationCount> GetLastMonthClassificationCount()
        {
            return IncidentDAL.GetLastMonthClassificationCount();
        }
        #endregion

        #region "get Last monthly  Incident Category chart"
        public List<StatusCount> GetLastMonthStatusCount()
        {
            return IncidentDAL.GetLastMonthStatusCount();
        }
        #endregion

        #region "get Last monthly  RootCauseCount chart"
        public List<RootCauseCount> GetLastMonthRootCauseCount()
        {
            return IncidentDAL.GetLastMonthRootCauseCount();
        }
        #endregion

        #region "get Last monthly  RecommenStatusCount chart"
        public List<ObservationStatusCount> GetLastMonthRecommenStatusCount()
        {
            return IncidentDAL.GetLastMonthRecommenStatusCount();
        }
        #endregion



        public DetailedIncidentViewModel GetIncidentDetailForView(int incidentID)
        {
            return IncidentDAL.GetIncidentDetailForView(incidentID);
        }

        public int IncidentReportUpdate(NewIncidentViewModel incidentReport, int currentuser)
        {
            int NewIncidentID = 0;
            StringBuilder mailBody = new StringBuilder();
            StringBuilder mailBody1 = new StringBuilder();
            MonitProMailer monitProMailer = new MonitProMailer();
            MonitProEmail monitProEmail = new MonitProEmail();
          


            string strECNo = String.Format("EC{0}", DateTime.Now);
            incidentReport.Incident.ECNumber = incidentReport.Incident.ECNumber == null ? strECNo : incidentReport.Incident.ECNumber;
            incidentReport.Incident.Comments = incidentReport.Incident.Comments == null ? String.Empty : incidentReport.Incident.Comments;
            if ((incidentReport.Incident.ImageFile != null) && (incidentReport.Incident.InvesAttachment != null))
            {

                var fileName = Path.GetFileName(incidentReport.Incident.ImageFile.FileName);
                var fileName2 = Path.GetFileName(incidentReport.Incident.InvesAttachment.FileName);
                NewIncidentID = IncidentDAL.IncidentReportUpdate(incidentReport, fileName, fileName2, currentuser);
            }
            else if ((incidentReport.Incident.ImageFile != null) && (incidentReport.Incident.InvesAttachment == null))
            {
                var fileName = Path.GetFileName(incidentReport.Incident.ImageFile.FileName);
                var fileName2 = incidentReport.Incident.secondfile == null ? String.Empty : incidentReport.Incident.secondfile;
                NewIncidentID = IncidentDAL.IncidentReportUpdate(incidentReport, fileName, fileName2, currentuser);

            }
            else if ((incidentReport.Incident.ImageFile == null) && (incidentReport.Incident.InvesAttachment != null))
            {
                var fileName = incidentReport.Incident.FileName == null ? String.Empty : incidentReport.Incident.FileName;
                var fileName2 = Path.GetFileName(incidentReport.Incident.InvesAttachment.FileName);
                NewIncidentID = IncidentDAL.IncidentReportUpdate(incidentReport, fileName, fileName2, currentuser);

            }

            else
            {
                var fileName = incidentReport.Incident.FileName == null ? String.Empty : incidentReport.Incident.FileName;
                var fileName2 = incidentReport.Incident.secondfile == null ? String.Empty : incidentReport.Incident.secondfile;
                NewIncidentID = IncidentDAL.IncidentReportUpdate(incidentReport, fileName, fileName2, currentuser);
            }
           

            if (incidentReport.Incident.Investigation == null && incidentReport.Incident.StatusID == 1)
            {
               Incident incident = IncidentDAL.GetIncident(incidentReport.Incident.IncidentID);
                monitProEmail.ToAddress = ConfigurationManager.AppSettings["HSEManagerEmail"] + "," + ConfigurationManager.AppSettings["IALGroupEmail"];
                monitProEmail.Subject = "Incident #" + incident.IncidentNO+ " (" + incidentReport.Incident.Title + ") is submitted for review";
                mailBody.Append("Dear Sir,  <br />");
                mailBody.Append("<br /><b><u>Preliminary Incident Report (FIR)</u></b>");
                mailBody.Append("<br /><br/><table width=100% border = 1>");
                mailBody.Append(" <tr><td><b>Plant/Area  </b></td><td>" + incident.PlantName+ "</td><td><b>Incident Classification  </b></td><td>" + incident.IncidentClassName+"</td></tr>");
                mailBody.Append(" <tr><td><b>Incident Description </b></td><td colspan = 3> " + incident.Title+"</td></tr>");
                //mailBody.Append("<br/><b>Preliminary Detail:</b>" + incident.Description);
                mailBody.Append(" <tr><td><b>Incident Time </b></td><td> " + incident.IncidentTime+ "</td><td><b> Created By </b></td><td>"+ incident.CreatedByName+"</td><tr>");
                mailBody.Append("<tr><td><b>Is Anyone injured </b></td><td> " + incident.injuredOrNot + " </td><td><b>Details </b></td><td>" + incident.injuredDecription + "</td></tr>");
                mailBody.Append(" <tr><td><b>Is loss of material </b></td><td>  " + incident.LossOfMaterial + "</td><td><b>Details </b></td><td>" + incident.LossQuantity+ "</td></tr>");
                mailBody.Append(" <tr><td><b>Is there damage to equipment </b></td><td> " + incident.DamageEquipment + "</td><td><b>Details </b></td><td>" + incident.DamageDetails + "</td></tr>");
                mailBody.Append(" <tr><td><b>Persons available during incident </b></td><td colspan = 3> " + incident.PersonAvailable+ "</td></tr>");
                mailBody.Append(" <tr><td><b>Immediate actions taken </b></td><td colspan =3> " + incident.ImmediateAction+ "</td></tr>");
                mailBody.Append("<tr><td><b>Probable causes </b></td><td colspan =3 >" + incident.ProbableCauses + "</td></tr>");
                mailBody.Append(" </table>");
                mailBody.Append(" <br /><br /> Please log in to the GCPL application for further details. ");
                mailBody1.Append("<br />Regards,<br/>Safety Manager<br/>Note: This is a system generated email.<br/>");
                monitProEmail.CC = ConfigurationManager.AppSettings["IncCC"];
                monitProEmail.Body = mailBody.ToString();
                monitProEmail.Body1 = mailBody1.ToString();
                monitProMailer.SendEmail(monitProEmail);
            }

            if (incidentReport.Incident.StatusID == 4)
            {

                List<IncidentEmail> EmailList = new List<IncidentEmail>();
                EmailList = IncidentDAL.GetObservationForMailing(incidentReport.Incident.IncidentID);
                foreach (IncidentEmail iemail in EmailList)
                {
                    mailBody.Clear();
                    monitProEmail.ToAddress = iemail.OwnerEmail;
                    monitProEmail.CC = ConfigurationManager.AppSettings["HSEManagerEmail"] + iemail.ObserverEmail + iemail.TeamListEmail;
                    monitProEmail.Subject = incidentReport.Incident.IncidentID + " (" + incidentReport.Incident.Title + ") submitted for approval";
                    mailBody.Append("Dear Sir,  <br />");
                    //mailBody.Append("<br/><b>Preliminary details:&nbsp;&nbsp;</b>" + iemail.IncidentDescription);
                    mailBody.Append("<br/>Incident #" + incidentReport.Incident.IncidentID + " has been submitted for your review and approval. ");
                    mailBody.Append("<br/>Please log in to GCPL application and complete the detail. ");
                    mailBody.Append("The approval shall be completed in 7 days on (on or before <b>" + iemail.SubmitForApprovalDate + "</b>) ");
                    mailBody1.Append(" <br /> Regards,<br/>Safety Manager<br/>Note: This is a system generated email.");
                    monitProEmail.Body = mailBody.ToString();
                    monitProEmail.Body1 = mailBody1.ToString();
                    if (monitProEmail.ToAddress != "")
                    {
                        monitProMailer.SendEmail(monitProEmail);
                    }
                }
            }
            return incidentReport.Incident.IncidentID;
        }

        public void SaveObservers(int incidentID, string observerList, string lead, string deptManager, int userID, string investigator)
        {
            if (observerList.IndexOf(',') == 0)
            {
                observerList = observerList.Remove(0, 1);
            }
            IncidentDAL.SaveObservers(incidentID, observerList, lead, deptManager, userID, investigator);
            Incident incident = new Incident();
            //sending email to observers
            StringBuilder mailBody = new StringBuilder();
            StringBuilder mailBody1 = new StringBuilder();
            List<IncidentEmail> EmailList = new List<IncidentEmail>();
            incident = IncidentDAL.GetIncident(incidentID);
            EmailList = IncidentDAL.GetObservationForMailing(incidentID);
            MonitProMailer monitProMailer = new MonitProMailer();

            foreach (IncidentEmail iemail in EmailList)
            {
                mailBody.Clear();
                mailBody.Append("Dear Sir,  <br />");
                //mailBody.Append("<br/><b>Preliminary details:&nbsp;&nbsp;</b>" + iemail.IncidentDescription);
                mailBody.Append(" <br />Incident Investigation Team for the incident <b>#" + incidentID + "</b> is formed. Investigation Team leader will be responsible to investigate along with the team, prepare the report and submit for approval.");
                mailBody.Append(" <br /> Please log in to GCPL application and complete the details. ");
                mailBody.Append(" <br /> Investigation shall be completed in 30 days (on or before <b>" + iemail.AssignDate + "</b>) and submitted for approval.");
                mailBody.Append("<br /><br/><table width=100% border = 1>");
                mailBody.Append("<tr><td><b>Incident Owner </b></td><td>"+ iemail.IncidentOwner+"</td></tr>");
                mailBody.Append("<tr><td><b>Investigation Team lead</b></td><td>" + iemail.Teamlead + "</td></tr>");
                mailBody.Append("<tr><td><b>Team Members</b></td><td>" + iemail.TeamMembers + "</td></tr>");
                mailBody.Append("<tr><td><b>Investigation facilitator</b></td><td>" + iemail.Invesfacilitator + "</td></tr>");
                mailBody.Append("</table>");
                mailBody1.Append("<br /><br />Regards,<br/>Safety Manager<br/>Note: This is a system generated email.<br/>");
                MonitProEmail monitProEmail = new MonitProEmail();

                monitProEmail.ToAddress = iemail.OwnerEmail + iemail.ObserverEmail + iemail.TeamListEmail + iemail.InvestigatorEmail;
                monitProEmail.CC = ConfigurationManager.AppSettings["HSEManagerEmail"];
                monitProEmail.Subject = "Incident # " + incidentID + " (" + incident.Title + ") Investigation team assigned";
                monitProEmail.Body = mailBody.ToString();
                monitProEmail.Body1 = mailBody1.ToString();
                if (monitProEmail.ToAddress != "")
                {
                    monitProMailer.SendEmail(monitProEmail);
                }
            }

        }

        public void SaveObservation(IncidentObservation insObservation)
        {
            if (insObservation.InciObAttachment != null)
            {
                var fileName = Path.GetFileName(insObservation.InciObAttachment.FileName);
                IncidentDAL.SaveObservervation(insObservation, fileName);
            }
            else if (insObservation.InciAttachment != " ")
            {
                var fileName = insObservation.InciAttachment;
                IncidentDAL.SaveObservervation(insObservation, fileName);
            }
            else if (insObservation.InciObAttachment == null)
            {
                var fileName = string.Empty;
                IncidentDAL.SaveObservervation(insObservation, fileName);
            }
  
        }
        public int DeleteIncidentObservation(int OBID)
        {
            return IncidentDAL.DeleteIncidentObservation(OBID);
        }
        public void DeleteOBDOC(int obid)
        {
             IncidentDAL.DeleteOBDOC(obid);
        }
            public List<ObservationViewModel> GetAllIncidentObservation()
        {
            return IncidentDAL.GetAllIncidentObservation();
        }
    
        public List<ObservationViewModel> SearchOpenIncidentsForObservation(IncidentSearchViewModel incidentsearchviewmodel)

        {
            return IncidentDAL.SearchOpenIncidentsForObservation(incidentsearchviewmodel);
        }

        public void UpdateIncidentStatus(int incidentID, int StatusID, string Comments, int userID)
        {
            IncidentDAL.UpdateIncidentStatus(incidentID, StatusID, Comments, userID);
            Incident incident = new Incident();
            incident = IncidentDAL.GetIncident(incidentID);
            //sending email to observers
            StringBuilder mailBody = new StringBuilder();
            StringBuilder mailBody1 = new StringBuilder();
            incident = IncidentDAL.GetIncident(incidentID);
            List<IncidentEmail> EmailList = new List<IncidentEmail>();
            EmailList = IncidentDAL.GetObservationForMailing(incidentID);
            MonitProMailer monitProMailer = new MonitProMailer();
            MonitProEmail monitProEmail = new MonitProEmail();
            if ((StatusID == 5) || (StatusID == 2))
            {
                foreach (IncidentEmail iemail in EmailList)
                {
                    mailBody.Clear();
                    mailBody.Append("Dear Sir,  <br />");
                    if (StatusID == 5)
                    {
                        //mailBody.Append("<b>Preliminary details:&nbsp;&nbsp;</b>" + iemail.IncidentDescription);
                        mailBody.Append("<br/>Incident #" + incidentID + " has been approved and due for action.Please log in to the GCPL application and complete the detail. ");
                        monitProEmail.Subject = "Incident # " + incidentID + " (" + incident.Title + ") approved and due for action";
                        monitProEmail.ToAddress = iemail.ActionerEmail + iemail.ObserverEmail;
                        monitProEmail.CC = ConfigurationManager.AppSettings["HSEManagerEmail"]+iemail.TeamListEmail + "," + ConfigurationManager.AppSettings["IALGroupEmail"]; 
                    }
                    if (StatusID == 2)
                    {
                        //mailBody.Append("<b>Preliminary details:&nbsp;&nbsp;</b>" + iemail.IncidentDescription);
                        mailBody.Append("<br/>Incident  #" + incidentID + " is recycled. Please discuss with Incident Owner and log in to the GCPL application and resubmit for approval.");
                        monitProEmail.Subject = "Incident # " + incidentID + " (" + incident.Title + ") is Recycled";
                        monitProEmail.ToAddress = iemail.ObserverEmail + iemail.TeamListEmail;
                        monitProEmail.CC = ConfigurationManager.AppSettings["HSEManagerEmail"] + "," + ConfigurationManager.AppSettings["IncCC"];
                    }


                    mailBody.Append(" <br /> <br />  Approver Comments: " + Comments + "<br /><br />");

                    mailBody1.Append("<br />Regards,<br/>Safety Manager<br/>Note: This is a system generated email.");

                    monitProEmail.Body = mailBody.ToString();
                    monitProEmail.Body1 = mailBody1.ToString();
                    if (monitProEmail.ToAddress != "")
                    {
                        monitProMailer.SendEmail(monitProEmail);
                    }
                }
            }



        }


        public List<IncidentImage> GetIncidentImages(int incidentID)
        {
            return IncidentDAL.GetIncidentImages(incidentID);
        }

        public Attachment AddTempAttachment(string FullFileName, string folderToUpload, int sno)
        {
            Attachment attachment = new Attachment();
            string pathToUpload = ConfigurationManager.AppSettings["PathToUpload"];

            attachment.FileName = Path.GetFileName(FullFileName);
            attachment.FilePath = FullFileName;
            attachment.SNo = sno++;       
   

            return attachment;
        }

        public bool UploadImages(IncidentImage incidentImage, int currentUser)
        {
            bool isSuccess = false;
            var fileName = Path.GetFileName(incidentImage.ImageFile.FileName);

            try
            {

                IncidentDAL.SaveIncidentImages(incidentImage, fileName, currentUser);

                isSuccess = true;
            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
            }

            return isSuccess;
        }

        public void DeleteImage(int ImageId)
        {
            IncidentDAL.DeleteImage(ImageId);
        }


        #region "Employee Info - This section should go to AccountBL Class"
        public List<Employee> GetAllEmployees()
        {
            return IncidentDAL.GetAllEmployees();
        }
        #endregion
    }
}