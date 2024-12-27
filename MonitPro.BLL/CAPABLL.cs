
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
using MonitPro.Models.CAPA;
using MonitPro.Models.CAPAViewModel;
using MonitPro.Models;
using MonitPro.Models.MOC;


namespace MonitPro.BLL
{
    public class CAPABLL
    {
        CAPADAL CapaDAL = new CAPADAL();
        IncidentReportDAL IncidentDAL = new IncidentReportDAL();
        MOCDAL mOCDAL = new MOCDAL();

        public CAPABLL()
        {

        }

        public List<CAPASource> GetAuditCAPAsource(int? AuditID)
        {

            return CapaDAL.GetAuditCAPAsource(AuditID);
        }
        public List<ActionerModel> GetAllCAPAObservations()
        {
            return CapaDAL.GetAllCAPAObservations();
        }
        public void UpdateAreaOwnerBLL(string PlantID, string AreaOwnerID)
        {
             CapaDAL.UpdateAreaOwnerDAL(PlantID, AreaOwnerID);
        }




        public List<CAPAEmail> GetActionerForMailing(int id)
        {
            return CapaDAL.GetActionerForMailing(id);
        }
        public List<Plants> GetPlants()
        {
            return IncidentDAL.GetPlants();
        }
        public List<CAPAPlants> GetcapaPlants()
        {
            return CapaDAL.GetcapaPlants();
        }
        public List<ActionsCount> GetMyActionStatusCount(int UserID)
        {
            return IncidentDAL.GetMyActionStatusCount(UserID);
        }
        public List<AuditType> GetAuditType()
        {
            return CapaDAL.GetAuditType();
        }
        public MyactionDashboardCount GetDashboardOverallCount()
        {
            return CapaDAL.GetDashboardOverallCount();
        }
        public MyApprovalCount GetDashboardApprovalCount(int UserID)
        {
            return CapaDAL.GetDashboardApprovalCount(UserID);
        }
        public List<CAPASource> GetCAPASource()
        {
            return CapaDAL.GetCAPASource();
        }
        public List<CAPACategory> GetCAPACategory()
        {
            return CapaDAL.GetCAPACategory();
        }
        #region "get Last month ObservationCount chart"
        public List<ObservationCount> GetLastMonthObservationCount()
        {
            return CapaDAL.GetLastMonthObservationCount();
        }
        #endregion

        #region "get Last month CapaSourceCount chart"
        public List<CapaSourceCounts> GetLastMonthCapaSourceCount()
        {
            return CapaDAL.GetLastMonthCapaSourceCount();
        }
        #endregion

        #region "get Last month CategoryCount chart"
        public List<CategoryCount> GetLastMonthCategoryCount()
        {
            return CapaDAL.GetLastMonthCategoryCount();
        }
        #endregion

        #region "get Last month ActionStatusCount chart"
        public List<ActionsCount> GetLastMonthActionStatusCount()
        {
            return CapaDAL.GetLastMonthActionStatusCount();
        }
        #endregion

        #region "get Last month CapaPriorityCount chart"
        public List<PriorityCount> GetLastMonthCapaPriorityCount()
        {
            return CapaDAL.GetLastMonthCapaPriorityCount();
        }
        #endregion

        public List<CAPAPriority> GetCAPAPriority()
        {
            return CapaDAL.GetCAPAPriority();
        }
        public List<PriorityCount> GetLastMonthCapaFunctionalManagerCount()
        {
            return CapaDAL.GetLastMonthCapaFunctionalManagerCount();
        }


        public List<Status> GetStatus()
        {
            return IncidentDAL.GetIncidentStatus();
        }
        public List<ContractorEmp> GetContractorEmp()
        {
            return IncidentDAL.GetContractorEmp();
        }

        public List<CAPAObservationStatus> GetCAPAObservationStatus()
        {
            return CapaDAL.GetCAPAObservationStatus();
        }
        public CreateCAPA GetCAPADetails(int capaID)
        {
            return CapaDAL.GetCAPADetails(capaID);
        }

        public List<Employee> GetAllManager()
        {
            return IncidentDAL.GetAllManager();
        }
        public CreateCAPA GetMyCAPADetails(int ObservID)
        {
            return CapaDAL.GetMyCAPADetails(ObservID);
        }
        public int CAPAUpdate(NewCAPAModel newcapa, int currentuser)
        {
            int NewcapaID = 0;
            StringBuilder mailBody = new StringBuilder();

            CreateCAPA createcapa = new CreateCAPA();

            newcapa.CreateCAPA.Comments = newcapa.CreateCAPA.Comments == null ? String.Empty : newcapa.CreateCAPA.Comments;
            if (newcapa.CreateCAPA.ImageFile != null)
            {

                var fileName = Path.GetFileName(newcapa.CreateCAPA.ImageFile.FileName);
                NewcapaID = CapaDAL.CAPAUpdate(newcapa, fileName, currentuser);
            }
            else
            if (newcapa.CreateCAPA.FileName == null)
            {
                var fileName = string.Empty;
                NewcapaID = CapaDAL.CAPAUpdate(newcapa, fileName, currentuser);
            }
            else
            {
                var fileName = newcapa.CreateCAPA.FileName;
                NewcapaID = CapaDAL.CAPAUpdate(newcapa, fileName, currentuser);
            }

            return newcapa.CreateCAPA.CAPAID;
        }
        public List<CAPAViewModel> GetOpenCapa()
        {
            return CapaDAL.GetOpenCapa();
        }
        public List<CAPAViewModel> SearchOpenCapa(CAPASearchViewModel capasearchviewmodel)
        {
            return CapaDAL.SearchOpenCapa(capasearchviewmodel);
        }


        public List<CAPAViewModel> GetAllClosedCapa()
        {
            return CapaDAL.GetAllClosedCapa();
        }

        public void UpdateCAPAStatus(int capaID, int StatusID, string Comments, int userID)
        {
            CapaDAL.UpdateCAPAStatus(capaID, StatusID, Comments, userID);


        }


        public List<ObservationViewModelCapa> GetObservations(int capaID, int OBID)
        {
            return CapaDAL.GetObservations(capaID, OBID);
        }

        public List<ObservationViewModelCapa> GetMyObservations(int ObservID)
        {
            return CapaDAL.GetMyObservations(ObservID);
        }
        public CAPAObservation GetCAPAPlantObservations(int capaID)
        {
            return CapaDAL.GetCAPAPlantObservations(capaID);
        }

        public List<UserProfile> GetActionList()
        {
            return CapaDAL.GetActionList();
        }

        public SessionDetails GetSession(int userid)
        {
            return IncidentDAL.GetSession(userid);
        }

        public void DeleteOBImage(int OBID)
        {
            CapaDAL.DeleteOBImage(OBID);
        }

        public void SaveCAPAObservation(CAPAObservation cpObservation)
        {
            if (cpObservation.CAPAObAttachment != null)
            {

                var fileName = Path.GetFileName(cpObservation.CAPAObAttachment.FileName);
                CapaDAL.SaveCAPAObservation(cpObservation, fileName);
            }
            else
             if (cpObservation.cpobservationattachement != "  ")
            {
                var fileName = cpObservation.cpobservationattachement;
                CapaDAL.SaveCAPAObservation(cpObservation, fileName);
            }

            else
             if (cpObservation.CAPAObAttachment == null)
            {
                var fileName = string.Empty;
                CapaDAL.SaveCAPAObservation(cpObservation, fileName);
            }
            //sending email to CAPA Actioner
            StringBuilder mailBody = new StringBuilder();
            StringBuilder mailBody1 = new StringBuilder();
            StringBuilder ccbuild = new StringBuilder();
            StringBuilder ccfunbuild = new StringBuilder();
            List<CAPAEmail> EmailList = new List<CAPAEmail>();


            EmailList = CapaDAL.GetActionerForMailing(cpObservation.ObservationID);

            CAPAMailer Mailer = new CAPAMailer();

            foreach (CAPAEmail iemail in EmailList)
            {
                string[] ccaddress = { iemail.HSEMgrEmail, iemail.FirstownerEmail, iemail.SecondownerEmail };
                string[] ccfunaddress = { iemail.FunctionalMgr, iemail.HSEMgrEmail };
                foreach (string cc in ccaddress)
                {
                    if (!string.IsNullOrEmpty(cc))
                    {
                        ccbuild.Append(cc);
                        ccbuild.Append(",");
                    }
                }
                if (ccbuild.Length > 0)
                    ccbuild.Length--;

                foreach (string ccfun in ccfunaddress)
                {
                    if (!string.IsNullOrEmpty(ccfun))
                    {
                        ccfunbuild.Append(ccfun);
                        ccfunbuild.Append(",");
                    }
                }
                if (ccfunbuild.Length > 0)
                    ccfunbuild.Length--;
                if (iemail.ActionerEmail != "" || iemail.FunctionalMgr != null)
                {
                    MonitProEmail monitProEmail = new MonitProEmail();
                    mailBody.Clear();
                    mailBody.Append("Dear Sir, ");

                    if (iemail.FunctionalMgr != null && iemail.ActionerEmail == "")
                    {
                        mailBody.Append(" <br/> <br />CAPA Recommendation #" + iemail.ID + "is submitted for your review and action.Please log in to the GCPL application and do the needful.  ");
                        monitProEmail.ToAddress = iemail.FunctionalMgr;
                        monitProEmail.CC = ccbuild.ToString();
                        monitProEmail.Subject = "CAPA Recommendation #" + iemail.ID + ".-submitted for action";
                    }
                    if (iemail.ActionerEmail != "")
                    {
                        mailBody.Append(" <br /><br/>CAPA Recommendation #" + iemail.ID + " is submitted for your review and action.Please log in to the GCPL application and do the needful. ");
                        monitProEmail.ToAddress = iemail.ActionerEmail;
                        monitProEmail.CC = ccfunbuild.ToString();
                        monitProEmail.Subject = "CAPA Recommendation#" + iemail.ID + ".-submitted for action";
                    }
                    mailBody.Append("<br/><br/><table width=100% border=1>");
                    mailBody.Append("<tr><td><b>CAPA Recommend ID</b></td><td>" + iemail.ID + "</td><td><b>Area</b></td><td>" + iemail.Area + "</td><td><b>CAPA Category</b></td><td>" + iemail.Category + "</td></tr>");
                    mailBody.Append("<tr><td><b>CAPA Observation</b></td><td>" + iemail.Observation + "</td><td><b>CAPA Recommendation</b></td><td>" + iemail.Recommendation + "</td></tr>");
                    mailBody.Append("<tr><td><b>Priority</b></td><td>" + iemail.Priority + "</td><td><b>Target Date</b></td><td>" + iemail.TargetDate + "</td></tr>");
                    mailBody.Append("</table>");
                    mailBody1.Append(" <br />Regards,");
                    mailBody1.Append(" <br />Safety Manager");
                    mailBody1.Append(" <br /><br /> Note: This is a system generated email.");

                    monitProEmail.Body = mailBody.ToString();
                    monitProEmail.Body1 = mailBody1.ToString();
                    if (monitProEmail.ToAddress != "")
                    {
                        Mailer.SendCAPAEmail(monitProEmail);
                    }
                }

            }




        }

        public cpObservationViewModel EditCAPAObservation(int ObservationID)
        {
            return CapaDAL.EditCAPAObservation(ObservationID);
        }



        public void DeleteCAPAObservation(int ObservationID)
        {
            CapaDAL.DeleteCAPAObservation(ObservationID);
        }

        public List<ObservationViewModelCapa> GetAllCAPAObservation()
        {
            return CapaDAL.GetAllCAPAObservation();
        }
        public List<ObservationViewModelCapa> GetMyActionStatus(int UserID)
        {
            return CapaDAL.GetMyActionStatus(UserID);
        }

        public List<ActionsCount> GetActionStatusCount()
        {
            return CapaDAL.GetActionStatusCount();
        }

        public List<CapaSourceCounts> GetCapaSourceCount()
        {
            return CapaDAL.GetCapaSourceCount();
        }

        public List<CategoryCount> GetCategoryCount()
        {
            return CapaDAL.GetCategoryCount();
        }

        public List<ObservationCount> GetObservationCount()
        {
            return CapaDAL.GetObservationCount();
        }

        public List<PriorityCount> GetCapaFunctionalManagerCount()
        {
            return CapaDAL.GetCapaFunctionalManagerCount();
        }

        public List<PriorityCount> GetCapaPriorityCount()
        {
            return CapaDAL.GetCapaPriorityCount();
        }


        public List<ObservationViewModelCapa> SearchOpenCapaForObservation(CAPASearchViewModel capasearchviewmodel)
        {
            return CapaDAL.SearchOpenCapaForObservation(capasearchviewmodel);
        }

        public List<MOCRecomStatusCount> GetMOCRecomStatusCount(DateTime startDate, DateTime endDate)
        {
            return mOCDAL.GetMOCRecomStatusCount(startDate, endDate);
        }




    }
}


