using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonitPro.DAL;
using MonitPro.Models.Account;
using MonitPro.Models.Common;
using MonitPro.Models.Incident;
using MonitPro.Models.CAPA;
using MonitPro.Models.MOC;
using MonitPro.Models;
using MonitPro.Common.Library;
using System.Configuration;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Net;

namespace MonitPro.BLL
{
    public class MOCBLL
    {
        MOCDAL MocDAL = new MOCDAL();
        CAPADAL capadal = new CAPADAL();
        IncidentReportDAL IncidentDAL = new IncidentReportDAL();
        StringBuilder mailBody = new StringBuilder();

        StringBuilder mailBody1 = new StringBuilder();
        MonitProMailer monitProMailer = new MonitProMailer();
        MonitProEmail monitProEmail = new MonitProEmail();
        public MOCBLL()
        {

        }
        public List<Plants> GetPlants()
        {
            return MocDAL.GetPlants();
        }
        public List<MOCClassification> GetMOCClassification()
        {
            return MocDAL.GetMOCClassification();
        }
        public List<MOCReasonForChange> GetMOCReasonForChanges()
        {
            return MocDAL.GetMOCReasonForChanges();
        }
            public List<MOCCategory> GetMOCCategory()
        {
            return MocDAL.GetMOCCategory();
        }
        public List<MOCStatus> GetMOCStatus()
        {
            return MocDAL.GetMOCStatus();
        }
        public List<UserProfile> GetMOCApprover()
        {
            return MocDAL.GetMOCApprover();
        }
        public List<MOCTempStatus> GetMOCTempStatus()
        {
            return MocDAL.GetMOCTempStatus();
        }
        public MOCa GetMOC(int MOCID)
        {
            return MocDAL.GetMOC(MOCID);
        }
        public List<MOCViewModel> SearchOpenMOC(MOCSearchViewModel mocsearchviewmodel)
        {
            return MocDAL.SearchOpenMOC(mocsearchviewmodel);
        }
        public List<MOCAttachment> GetMOCAttachments(int MOCID)
        {
            return MocDAL.GetMOCAttachments(MOCID);
        }

        public void DeleteAttachments(int MOCAttachmentsID)
        {
            MocDAL.DeleteAttachments(MOCAttachmentsID);
        }

        public bool UploadAttachments(MOCAttachment mocattachments, int currentUser)
        {
            bool isSuccess = false;
            var fileName = Path.GetFileName(mocattachments.ImageFile.FileName);

            try
            {

                MocDAL.SaveMocAttachments(mocattachments, fileName, currentUser);

                isSuccess = true;
            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
            }

            return isSuccess;
        }
        public ApprovalList GetApprovalStageApprovar(int MOCID)
        {
            return MocDAL.GetApprovalStageApprovar(MOCID);
        }

        public List<UserProfile> GetActionList()
        {
            return capadal.GetActionList();
        }


        public List<MOCPriority> GetMOCPriority()
        {
            return MocDAL.GetMOCPriority();
        }
        public List<MOCRecomCategory> GetMOCRecomCategory()
        {
            return MocDAL.GetMOCRecomCategory();
        }

        public List<CAPAObservationStatus> GetCAPAObservationStatus()
        {
            return capadal.GetCAPAObservationStatus();
        }
        public List<MOCRecomPriority> GetMOCRecomPriority()
        {
            return MocDAL.GetMOCRecomPriority();
        }
        public List<MOCType> GetMOCType()
        {
            return MocDAL.GetMOCType();
        }
        public List<Employee> GetMOCFunMgr(int Deptid)
        {
            return MocDAL.GetMOCFunMgr(Deptid);
        }
        public MOCObservationEmail CheckMOCPSSRObservation(int MOCID)
        {
            return MocDAL.CheckMOCPSSRObservation(MOCID);
        }
        public int MOCInsertUpdate(NewMOCModel newmoc, int currentuser)
        {
            int NewMOCID = 0;

            MOCa moca1 = new MOCa();
            string strMOCNO = String.Format("MOC{0}", newmoc.moca.MOCID);
            moca1.MOCStatusIdentify = newmoc.moca.MOCStatusIdentify;
            newmoc.moca.MOCNumber = newmoc.moca.MOCNumber == null ? string.Empty : newmoc.moca.MOCNumber;
            if (newmoc.moca.ImageFile != null)
            {

                var fileName = Path.GetFileName(newmoc.moca.ImageFile.FileName);
                NewMOCID = MocDAL.MOCInsertUpdate(newmoc, fileName, currentuser);
            }
            else
            if (newmoc.moca.FileName == null)
            {
                var fileName = string.Empty;
                NewMOCID = MocDAL.MOCInsertUpdate(newmoc, fileName, currentuser);
            }
            else
            {
                var fileName = newmoc.moca.FileName;
                NewMOCID = MocDAL.MOCInsertUpdate(newmoc, fileName, currentuser);
            }
          
            moca1 = MocDAL.GetMOC(NewMOCID);


            if (newmoc.moca.MOCStatusIdentify == "30" && newmoc.moca.MOCStatusID == 1)
            {
                monitProEmail.ToAddress = moca1.FunMgrEmail;
                monitProEmail.CC = moca1.CreatedByEmail;
                monitProEmail.Subject = " MOC submitted for approval";
                mailBody.Append("Dear Sir,");
                mailBody.Append("<br />Below MOC is created and submitted for your review and approval as per the details below:");
                mailBody.Append("<br /><table width='100%' border='1'>");
                mailBody.Append("</br><tr><td><b>Plant/Area</b></td><td>" + moca1.PlantName + "</td><td><b>Change Type</b></td><td>" + moca1.MOCClassName + "</td></tr>");
                mailBody.Append("</br><tr><td><b>Secondary Changes</b></td><td>" + moca1.MOCCategoryName + "</td><td><b>Change Category </b></td><td>" + moca1.MOCTypeName + "</td></tr>");
                mailBody.Append("</br><tr><td><b>Change Title</b></td><td colspan = 3>" + moca1.MOCTitle + "</td></tr>");

                mailBody.Append("</table>");
                mailBody.Append(" <br/><br/> Please log in to the GCPL application and do the needful.");
                mailBody.Append("<br />" + ConfigurationManager.AppSettings["Link"]);
                mailBody.Append("<br />Regards,<br />");
                mailBody.Append(moca1.CreatedBy);

                mailBody1.Append(" <br /> <br />Note: This is a system generated email.");
                monitProEmail.Body = mailBody.ToString();
                monitProEmail.Body1 = mailBody1.ToString();
                monitProMailer.SendEmail(monitProEmail);
            }
            else if (newmoc.moca.MOCStatusID == 2 && newmoc.moca.MOCStatusIdentify == "40")
            {
                monitProEmail.ToAddress = moca1.MOCAdvisorEmail;
                monitProEmail.CC = moca1.CreatedByEmail;
                monitProEmail.Subject = moca1.MOCNumber + " Approved by functional manager and submitted for level assignment";
                mailBody.Append("Dear Sir,");
                mailBody.Append("<br />" + moca1.MOCNumber + " is Approved by functional manager and submitted for level assignment");
                mailBody.Append("<br /><table width='100%' border='1'>");
                mailBody.Append("</br><tr><td><b>Plant/Area</b></td><td>" + moca1.PlantName + "</td><td><b>Change Type</b></td><td>" + moca1.MOCClassName + "</td></tr>");
                mailBody.Append("</br><tr><td><b>Secondary Changes</b></td><td>" + moca1.MOCCategoryName + "</td><td><b>Change Category </b></td><td>" + moca1.MOCTypeName + "</td></tr>");
                mailBody.Append("</br><tr><td><b>Change Title</b></td><td colspan = 3>" + moca1.MOCTitle + "</td></tr>");

                mailBody.Append("</table>");
                mailBody.Append(" <br/><br/> Please log in to the GCPL application and do the needful.");
                mailBody.Append("<br />" + ConfigurationManager.AppSettings["Link"]);
                mailBody.Append("<br />Regards,<br />");
                mailBody.Append(moca1.FunMgrName);

                mailBody1.Append(" <br /> <br />Note: This is a system generated email.");
                monitProEmail.Body = mailBody.ToString();
                monitProEmail.Body1 = mailBody1.ToString();
                monitProMailer.SendEmail(monitProEmail);
            }

            return newmoc.moca.MOCID;
        }

        public List<MOCViewModel> GetOpenMOC()
        {
            return MocDAL.GetOpenMOC();
        }
        public List<ApproverModel> GetApproverForMOCPageList()
        {
            return MocDAL.GetApproverForMOCPageList();
        }
        public List<TemporaryMOCList> GetTemporaryList()
        {
            return MocDAL.GetTemporaryList();
        }
        public void SaveMOCObservation(MOCObservation cpObservation)
        {
            MocDAL.SaveMOCObservation(cpObservation);

        }


        public MCObservationViewModel EditMOCObservation(int ObservationID)
        {
            return MocDAL.EditMOCObservation(ObservationID);
        }
        public MOCObservationEmail CheckMOCObservation(int MOCID)
        {
            return MocDAL.CheckMOCObservation(MOCID);

        }

        public List<ObservationViewModelMOC> GetObservationModel(int MOCID, int ObservID)
        {
            return MocDAL.GetObservationModel(MOCID, ObservID);
        }

        public List<ObservationViewModelMOC> GetAllMOCObservation()
        {
            return MocDAL.GetAllMOCObservation();
        }
       
        public void UpdateMOCStatus(int MOCID, int StatusID, string CloseComments, int userID)
        {
            MocDAL.UpdateMOCStatus(MOCID, StatusID, CloseComments, userID);

        }
        public List<ObservationViewModelMOC> SearchOpenMOCForObservation(MOCSearchViewModel mOCSearch)

        {
            return MocDAL.SearchOpenMOCForObservation(mOCSearch);
        }

        public List<MOCViewModel> GetAllClosedMOC()
        {
            return MocDAL.GetAllClosedMOC();

        }
        public List<Employee> GetAllEmployees()
        {
            return IncidentDAL.GetAllEmployees();
        }

        public MOCApproverList GetApprovalStages()
        {
            return MocDAL.GetApprovalStages();
        }
        public MOCApproverList GetApprovalStagesSave(int MOCID)
        {
            return MocDAL.GetApprovalStagesSave(MOCID);
        }
        public List<ApprovalList> SaveApprovals(MOCApproverList approverList, List<ApprovalList> ApprovalList)
        {

            List<ApprovalList> approveList = new List<ApprovalList>();
            MOCApproverEmail ApproverEmail = new MOCApproverEmail();
            MOCa mocd = new MOCa();
            MocDAL.SaveApprovals(approverList, ApprovalList);
            mocd = MocDAL.GetMOC(approverList.MOCID);
            ApproverEmail = MocDAL.GetMOCApproverEmail(approverList.MOCID, approverList.UserID);
            if (mocd.MOCTeamEmailCheck == "Send Email")
            {
                //
                StringBuilder ccbuild = new StringBuilder();
                string[] ccaddress1 = { ApproverEmail.NextApproverEmail, ApproverEmail.MoccoordinateEmail};
                string[] ccaddress = ccaddress1.Distinct().ToArray();
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
                var tt = ccbuild.ToString();
                if (mocd.MOCAdvisorEmail != null && mocd.MOCAdvisorEmail != "" && tt != null && tt != "")
                {
                    monitProEmail.ToAddress = ccbuild.ToString();
                    monitProEmail.CC = mocd.MOCAdvisorEmail;
                }
                else if (tt != null && tt != "")
                {
                    monitProEmail.ToAddress = ccbuild.ToString();
                }
                else if (mocd.MOCAdvisorEmail != null && mocd.MOCAdvisorEmail != "")
                {
                    monitProEmail.ToAddress = ccbuild.ToString();
                }
                //else
                //{
                //    monitProEmail.ToAddress = ConfigurationManager.AppSettings["MOCnoMail"];
                //}
                
                //
                //monitProEmail.ToAddress = ApproverEmail.NextApproverEmail + ',' + ApproverEmail.MoccoordinateEmail;
                //monitProEmail.CC = mocd.MOCAdvisorEmail ;
                monitProEmail.Subject = mocd.MOCNumber + " is submitted for Design Review.";
                mailBody.Append("Dear Sir,");
                mailBody.Append("<br />" + mocd.MOCNumber + " is submitted for Design Review. Mr. " + ApproverEmail.MOCCoordinator + " has been assigned as MOC Coordinator and he will monitor the MOC progress and till completion");
                mailBody.Append("<br /><table width='100%' border='1'>");
                mailBody.Append("</br><tr><td><b>Plant/Area</b></td><td>" + mocd.PlantName + "</td><td><b>Change Type</b></td><td>" + mocd.MOCClassName + "</td></tr>");
                mailBody.Append("</br><tr><td><b>Secondary Changes</b></td><td>" + mocd.MOCCategoryName + "</td><td><b>Change Category </b></td><td>" + mocd.MOCTypeName + "</td></tr>");
                mailBody.Append("</br><tr><td><b>Change Title</b></td><td colspan = 3>" + mocd.MOCTitle + "</td></tr>");
                mailBody.Append("</br><tr><td><b>MOC Priority</b></td><td>" + ApproverEmail.PriorityName + "</td><td><b>Target Date</b></td><td>" + ApproverEmail.TargetDate + "</td></tr>");
                mailBody.Append("</table>");
                mailBody.Append(" <br/><br/> Please log in to the GCPL application and do the needful.");
                mailBody.Append("<br />" + ConfigurationManager.AppSettings["Link"]);
                mailBody.Append("<br />Regards,<br />");
                mailBody.Append(ApproverEmail.ApproverName);

                mailBody1.Append(" <br /> <br />Note: This is a system generated email.");
                monitProEmail.Body = mailBody.ToString();
                monitProEmail.Body1 = mailBody1.ToString();
                if(monitProEmail.ToAddress != null && monitProEmail.ToAddress != "")
                {
                    monitProMailer.SendEmail(monitProEmail);
                }
                
            }
            return approveList;

        }
        public MOCObservationEmail CheckCriticalRecomm(int MOCID)
        {
            return MocDAL.CheckCriticalRecomm(MOCID);
        }
        public MOCObservationEmail CheckPSSRCriticalRecomm(int MOCID)
        {
            return MocDAL.CheckPSSRCriticalRecomm(MOCID);
        }

        public void ApproverAdd(int MOCID, int ApproverStagesID, int EmployeeID, string TargetDate)
        {

            MocDAL.ApproverAdd(MOCID, ApproverStagesID, EmployeeID, TargetDate);

        }
        public void DR_RARecom(int MOCID, int UserID)
        {
            MOCObservationEmail obemail = new MOCObservationEmail();
            MOCApproverEmail ApproverEmail = new MOCApproverEmail();
            MOCa mocdetatils = new MOCa();
            obemail = MocDAL.CheckMOCObservation(MOCID);
            mocdetatils = MocDAL.GetMOC(MOCID);

            ApproverEmail = MocDAL.GetMOCApproverEmail(MOCID, UserID);

            mailBody.Clear();
            mailBody1.Clear();
            //
            StringBuilder ccbuild = new StringBuilder();
            string[] ccaddress = { obemail.DREmail, ApproverEmail.MoccoordinateEmail, mocdetatils.MOCAdvisorEmail , obemail.RAEmail };
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
            var tt = ccbuild.ToString();
            if (obemail.ActionByEmail != null && obemail.ActionByEmail != "" && tt != null && tt != "")
            {
                monitProEmail.ToAddress = obemail.ActionByEmail;
                monitProEmail.CC = ccbuild.ToString();
            }
            else if (obemail.ActionByEmail != null && obemail.ActionByEmail != "")
            {
                monitProEmail.ToAddress = obemail.ActionByEmail;
            }
            else if (tt != null && tt != "")
            {
                monitProEmail.ToAddress = ccbuild.ToString();
            }
            //

            //monitProEmail.ToAddress = obemail.ActionByEmail;
            //monitProEmail.CC = obemail.DREmail + ',' + ApproverEmail.MoccoordinateEmail + ',' + mocdetatils.MOCAdvisorEmail + ',' + obemail.RAEmail;
            monitProEmail.Subject = mocdetatils.MOCNumber + " Recommendations submitted for your action.";
            mailBody.Append("Dear Sir,");
            mailBody.Append("<br />The following recommendation(s) need your action.");
            mailBody.Append("<br /><table width='100%' border='1'>");
            mailBody.Append("</br><tr><th><b>MOC Number</b></th><th><b>Recom ID</b></th><th><b>Recomm Category</b></th><th><b>Priority</b></th><th><b>Actions / Deliverables</b></th><th><b>Action By</b></th><th><b>Target Date</b></th></tr>");
            foreach (var item in obemail.obserlist)
            {

                mailBody.Append("</br><tr><td>" + item.MOCNo + "</td><td>" + item.ObservationID + "</td><td>" + item.RecomCategory + "</td><td>" + item.PriorityName + "</td><td>" + item.Recommendation + "</td><td>" +item.ActionByName+"</td><td>"+ item.TargetDate+"</td></tr>");

            }
            mailBody.Append("</table>");

            mailBody.Append(" <br/><br/> Please log in to the GCPL application and do the needful.");
            mailBody.Append("<br />" + ConfigurationManager.AppSettings["Link"]);
            mailBody.Append("<br />Regards,<br />");
            mailBody.Append(mocdetatils.MOCAdvisorName);

            mailBody1.Append(" <br /> <br />Note: This is a system generated email.");
            monitProEmail.Body = mailBody.ToString();
            monitProEmail.Body1 = mailBody1.ToString();
            monitProMailer.SendEmail(monitProEmail);

        }

        public void PSSRRecom(int MOCID, int UserID)
        {
            MOCObservationEmail obemail = new MOCObservationEmail();
            MOCApproverEmail ApproverEmail = new MOCApproverEmail();
            MOCa mocdetatils = new MOCa();
            obemail = MocDAL.CheckMOCPSSRObservation(MOCID);
            mocdetatils = MocDAL.GetMOC(MOCID);

            ApproverEmail = MocDAL.GetMOCApproverEmail(MOCID, UserID);

            mailBody.Clear();
            mailBody1.Clear();
            monitProEmail.ToAddress = obemail.ActionByEmail;
            if (obemail.ExecMgr != "")
            {
                monitProEmail.CC = obemail.PSSREmail + ',' + ApproverEmail.MoccoordinateEmail + ',' + mocdetatils.MOCAdvisorEmail + ',' + obemail.ExecMgr;
            }
            else
            {
                monitProEmail.CC = obemail.PSSREmail + ',' + ApproverEmail.MoccoordinateEmail + ',' + mocdetatils.MOCAdvisorEmail;

            }
            monitProEmail.Subject = mocdetatils.MOCNumber + " Recommendations submitted for your action.";
            mailBody.Append("Dear Sir,");
            mailBody.Append("<br />The following recommendation(s) need your action.");
            mailBody.Append("<br /><table width='100%' border='1'>");
            mailBody.Append("</br><tr><th><b>MOC Number</b></th><th><b>Recom ID</b></th><th><b>Recomm Category</b></th><th><b>Priority</b></th><th><b>Actions / Deliverables</b></th><th><b>Target Date</b></th></tr>");
            foreach (var item in obemail.obserlist)
            {

                mailBody.Append("</br><tr><td>" + item.MOCNo + "</td><td>" + item.ObservationID + "</td><td>" + item.RecomCategory + "</td><td>" + item.PriorityName + "</td><td>" + item.Recommendation + "</td><td>" + item.TargetDate);

            }
            mailBody.Append("</table>");

            mailBody.Append(" <br/><br/> Please log in to the GCPL application and do the needful.");
            mailBody.Append("<br />" + ConfigurationManager.AppSettings["Link"]);
            mailBody.Append("<br />Regards,<br />");
            mailBody.Append(mocdetatils.MOCAdvisorName);

            mailBody1.Append(" <br /> <br />Note: This is a system generated email.");
            monitProEmail.Body = mailBody.ToString();
            monitProEmail.Body1 = mailBody1.ToString();
            monitProMailer.SendEmail(monitProEmail);

        }

        public int FuncationalManagerApprovers(FuncationalManagerApprove funap, int MOCID)
        {
            MOCApproverEmail ApproverEmail = new MOCApproverEmail();
            MOCa mocdetatils = new MOCa();

            MOCObservationEmail obemailcritical = new MOCObservationEmail();
            MOCObservationEmail obemail = new MOCObservationEmail();
            obemail = MocDAL.CheckMOCObservation(MOCID);
            obemailcritical = MocDAL.CheckCriticalRecomm(MOCID);
            MocDAL.FuncationalManagerApprovers(funap, MOCID);
            mocdetatils = MocDAL.GetMOC(MOCID);

            ApproverEmail = MocDAL.GetMOCApproverEmail(MOCID, funap.UserID);

            //if (funap.ApproveStatus == 3)
            //{
            //    monitProEmail.ToAddress = ApproverEmail.NextApproverEmail;
            //    monitProEmail.CC = ApproverEmail.ApproverEmail + ',' + ApproverEmail.MoccoordinateEmail;

            //    monitProEmail.Subject = mocdetatils.MOCNumber + " is submitted for Design Review Approval";
            //    mailBody.Append("Dear Sir,");
            //    mailBody.Append("<br />" + mocdetatils.MOCNumber + " is submitted for Design Review Approval");
            //}
            if (funap.ApproveStatus == 3)
            {
                StringBuilder ccbuild = new StringBuilder();
                string[] ccaddress1 = { ApproverEmail.DREmail, ApproverEmail.MoccoordinateEmail, mocdetatils.MOCAdvisorEmail };
                string[] ccaddress = ccaddress1.Distinct().ToArray();
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
                var tt= ccbuild.ToString();
                if (ApproverEmail.NextApproverEmail!=null && ApproverEmail.NextApproverEmail!="" && tt != null && tt != "")
                {
                    monitProEmail.ToAddress = ApproverEmail.NextApproverEmail;
                    monitProEmail.CC = ccbuild.ToString();
                }
                else if(ApproverEmail.NextApproverEmail != null && ApproverEmail.NextApproverEmail != "")
                {
                    monitProEmail.ToAddress = ApproverEmail.NextApproverEmail;
                }
               else if(tt!=null && tt!="")
                {
                    monitProEmail.ToAddress = ccbuild.ToString();
                }
               
                //monitProEmail.CC = ccbuild.ToString();

                monitProEmail.Subject = mocdetatils.MOCNumber + " is submitted for Risk Assessment";
                mailBody.Append("Dear Sir,");
                mailBody.Append("<br />" + mocdetatils.MOCNumber + " Design Review approved and  submitted for Risk Assessment");
            }
            else if (funap.ApproveStatus == 2)
            {
                monitProEmail.ToAddress = ApproverEmail.DREmail;
                monitProEmail.CC = ApproverEmail.MoccoordinateEmail;
                monitProEmail.Subject = mocdetatils.MOCNumber + " is Recycled for Design Review";
                mailBody.Append("Dear Sir,");
                mailBody.Append("<br />" + mocdetatils.MOCNumber + "  is Recycled for Re-Design Review.");

            }
            else if (funap.ApproveStatus == 6)
            {
                StringBuilder ccbuild = new StringBuilder();
                string[] ccaddress1 = { ApproverEmail.ApproverEmail, ApproverEmail.MoccoordinateEmail, mocdetatils.MOCAdvisorEmail };
                string[] ccaddress = ccaddress1.Distinct().ToArray();
                foreach (string cc in ccaddress)
                {
                     if(!string.IsNullOrEmpty(cc))
                    {
                        ccbuild.Append(cc);
                        ccbuild.Append(",");
                    }
                }
                if (ccbuild.Length > 0)
                    ccbuild.Length--;
                string tt = ccbuild.ToString();
                if (ApproverEmail.NextApproverEmail != null && ApproverEmail.NextApproverEmail != "" && tt != null && tt != "")
                {
                    monitProEmail.ToAddress = ApproverEmail.NextApproverEmail;
                    monitProEmail.CC = ccbuild.ToString();
                }
                else if (ApproverEmail.NextApproverEmail != null && ApproverEmail.NextApproverEmail != "")
                {
                    monitProEmail.ToAddress = ApproverEmail.NextApproverEmail;
                }
                else if (tt != null && tt != "")
                {
                    monitProEmail.ToAddress = ccbuild.ToString();
                }
                
                //monitProEmail.ToAddress = ApproverEmail.NextApproverEmail;
                //monitProEmail.CC = ApproverEmail.ApproverEmail + ',' + ApproverEmail.MoccoordinateEmail;
                monitProEmail.Subject = mocdetatils.MOCNumber + "  is submitted for Technical Approval";
                mailBody.Append("Dear Sir,");
                mailBody.Append("<br /> Design review and Risk Assessment completed for " + mocdetatils.MOCNumber + " and submitted for Technical Approval .");
                mailBody.Append("<br /><table width='100%' border='1'>");
                mailBody.Append("</br><tr><td><b>Plant/Area</b></td><td>" + mocdetatils.PlantName + "</td><td><b>Change Type</b></td><td>" + mocdetatils.MOCClassName + "</td></tr>");
                mailBody.Append("</br><tr><td><b>Secondary Changes</b></td><td>" + mocdetatils.MOCCategoryName + "</td><td><b>Change Category </b></td><td>" + mocdetatils.MOCTypeName + "</td></tr>");
                mailBody.Append("</br><tr><td><b>Change Title</b></td><td colspan = 3>" + mocdetatils.MOCTitle + "</td></tr>");
                mailBody.Append("</br><tr><td><b>MOC Priority</b></td><td>" + ApproverEmail.PriorityName + "</td><td><b>Target Date</b></td><td>" + ApproverEmail.TargetDate + "</td></tr>");
                mailBody.Append("</br><tr><td><b>Change Description</b></td><td colspan = 3>" + mocdetatils.MOCDescription + "</td></tr>");
                mailBody.Append("</table>");
                mailBody.Append(" <br/><br/> Please log in to the GCPL application and do the needful.");
                mailBody.Append("<br />" + ConfigurationManager.AppSettings["Link"]);
                mailBody.Append("<br />Regards,<br />");
                mailBody.Append(ApproverEmail.ApproverName);

                mailBody1.Append(" <br /> <br />Note: This is a system generated email.");
                monitProEmail.Body = mailBody.ToString();
                monitProEmail.Body1 = mailBody1.ToString();
                monitProMailer.SendEmail(monitProEmail);

                DR_RARecom(mocdetatils.MOCID, funap.UserID);
            }

            else if (funap.ApproveStatus == 4)
            {
                StringBuilder ccbuild = new StringBuilder();
                string[] ccaddress = { ApproverEmail.ApproverEmail, ApproverEmail.MoccoordinateEmail, mocdetatils.MOCAdvisorEmail };
                ccaddress= ccaddress.Distinct().ToArray(); 
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
                var tt = ccbuild.ToString();
                if (ApproverEmail.NextApproverEmail != null && ApproverEmail.NextApproverEmail != "" && tt != null && tt != "")
                {
                    monitProEmail.ToAddress = ApproverEmail.NextApproverEmail;
                    monitProEmail.CC = ccbuild.ToString();
                }
                else if (ApproverEmail.NextApproverEmail != null && ApproverEmail.NextApproverEmail != "")
                {
                    monitProEmail.ToAddress = ApproverEmail.NextApproverEmail;
                }
                else if (tt != null && tt != "")
                {
                    monitProEmail.ToAddress = ccbuild.ToString();
                }
                //monitProEmail.ToAddress = ApproverEmail.NextApproverEmail;
                //monitProEmail.CC = ApproverEmail.ApproverEmail + ',' + ApproverEmail.MoccoordinateEmail ;
                monitProEmail.Subject = mocdetatils.MOCNumber + " is submitted for Factory Manager Approval.";
                mailBody.Append("Dear Sir,");
                mailBody.Append("<br /> Design review, Risk Assessment and Technical Approval  completed for " + mocdetatils.MOCNumber + " and submitted for Final Approval");
                mailBody.Append("<br /><table width='100%' border='1'>");
                mailBody.Append("</br><tr><td><b>Plant/Area</b></td><td>" + mocdetatils.PlantName + "</td><td><b>Change Type</b></td><td>" + mocdetatils.MOCClassName + "</td></tr>");
                mailBody.Append("</br><tr><td><b>Secondary Changes</b></td><td>" + mocdetatils.MOCCategoryName + "</td><td><b>Change Category </b></td><td>" + mocdetatils.MOCTypeName + "</td></tr>");
                mailBody.Append("</br><tr><td><b>Change Title</b></td><td colspan = 3>" + mocdetatils.MOCTitle + "</td></tr>");
                mailBody.Append("</br><tr><td><b>MOC Priority</b></td><td>" + ApproverEmail.PriorityName + "</td><td><b>Cost - INR</b></td><td>" + mocdetatils.DRCost + "</td></tr>");
                mailBody.Append("</br><tr><td><b>Design Review Comments<b></td><td colspan = 3>" + ApproverEmail.DRRemarks + "</td></tr>");
                mailBody.Append("</br><tr><td><b>Risk Assessment Comments<b></td><td>" + ApproverEmail.RiskRemarks + "</td><td><b>Technical Approval  Comments</b></td><td>" + ApproverEmail.TechRemarks + "</td></tr>");
                mailBody.Append("</table>");

                mailBody.Append("<br /><table width='100%' border='1'>");
                mailBody.Append("</br><tr><th><b>MOC Number</b></th><th><b>Recom ID</b></th><th><b>Recomm Category</b></th><th><b>Priority</b></th><th><b>Actions / Deliverables</b></th><th><b>Target Date</b></th></tr>");
                foreach (var item in obemailcritical.obserlist)
                {
                    mailBody.Append("</br><tr><td>" + item.MOCNo + "</td><td>" + item.ObservationID + "</td><td>" + item.RecomCategory + "</td><td>" + item.PriorityName + "</td><td>" + item.Recommendation + "</td><td>" + item.TargetDate);

                }
                mailBody.Append("</table>");

                mailBody.Append(" <br/><br/> Please log in to the GCPL application and do the needful.");
                mailBody.Append("<br />" + ConfigurationManager.AppSettings["Link"]);
                mailBody.Append("<br />Regards,<br />");
                mailBody.Append(mocdetatils.MOCAdvisorName);

                mailBody1.Append(" <br /> <br />Note: This is a system generated email.");
                monitProEmail.Body = mailBody.ToString();
                monitProEmail.Body1 = mailBody1.ToString();
                monitProMailer.SendEmail(monitProEmail);
            }
            else if (funap.ID == 12)
            {

                monitProEmail.ToAddress = ApproverEmail.NextApproverEmail + ',' + ApproverEmail.MoccoordinateEmail;
                monitProEmail.CC = mocdetatils.MOCAdvisorEmail ;
                 monitProEmail.Subject = mocdetatils.MOCNumber + " is submitted for Operations.";
                mailBody.Append("Dear Sir,");
                mailBody.Append("<br />" + mocdetatils.MOCNumber + " is approved by factory Manager. Please do the needful for successful execution as per MOC.");
            
            }
            else if (funap.ID == 13)
            {

                monitProEmail.ToAddress = ApproverEmail.NextApproverEmail + ',' + ApproverEmail.MoccoordinateEmail;
                monitProEmail.CC = mocdetatils.MOCAdvisorEmail;
                monitProEmail.Subject = mocdetatils.MOCNumber + " is submitted for Maintenance Lead.";
                mailBody.Append("Dear Sir,");
                mailBody.Append("<br />" + mocdetatils.MOCNumber + " is approved by Operations. Please do the needful for successful execution as per MOC.");
               
            }
            else if (funap.ID == 14)
            {

                monitProEmail.ToAddress = ApproverEmail.NextApproverEmail + ',' + ApproverEmail.MoccoordinateEmail;
                monitProEmail.CC = mocdetatils.MOCAdvisorEmail;
                monitProEmail.Subject = mocdetatils.MOCNumber + " is submitted for HSE.";
                mailBody.Append("Dear Sir,");
                mailBody.Append("<br />" + mocdetatils.MOCNumber + " is approved by Maintenance Lead. Please do the needful for successful execution as per MOC.");
              
            }
            else if (funap.ID == 14)
            {

                PSSRRecom(mocdetatils.MOCID, funap.UserID);
            }
            //else if (funap.ApproveStatus == 10)
            //{
            //    monitProEmail.ToAddress = ApproverEmail.NextApproverEmail;
            //    monitProEmail.CC = ApproverEmail.MoccoordinateEmail + ',' + mocdetatils.MOCAdvisorEmail + ',' + ApproverEmail.ApproverEmail;
            //    monitProEmail.Subject = mocdetatils.MOCNumber + " is Submitted for As built Documentation.";

            //    mailBody.Append("Dear Sir,");
            //    mailBody.Append("<br />Please complete the As built documentation for " + mocdetatils.MOCNumber);
            //    mailBody.Append("<br /><table width='100%' border='1'>");
            //    mailBody.Append("</br><tr><td><b>Plant/Area</b></td><td>" + mocdetatils.PlantName + "</td><td><b>MOC Class</b></td><td>" + mocdetatils.MOCClassName + "</td></tr>");
            //    mailBody.Append("</br><tr><td><b>MOC Category of Change</b></td><td>" + mocdetatils.MOCCategoryName + "</td><td><b>MOC Type </b></td><td>" + mocdetatils.MOCTypeName + "</td></tr>");
            //    mailBody.Append("</br><tr><td><b>MOC Title</b></td><td colspan = 3>" + mocdetatils.MOCTitle + "</td></tr>");
            //    mailBody.Append("</br><tr><td><b>MOC Priority</b></td><td colspan = 3>" + ApproverEmail.PriorityName + "</td></tr>");
            //    mailBody.Append("</table>");
            //    mailBody.Append(" <br/><br/> Please log in to the GCPL application and do the needful.");
            //    mailBody.Append("<br />" + ConfigurationManager.AppSettings["Link"]);
            //    mailBody.Append("<br />Regards,<br />");
            //    mailBody.Append(mocdetatils.MOCAdvisorName);

            //    mailBody1.Append(" <br /> <br />Note: This is a system generated email.");
            //    monitProEmail.Body = mailBody.ToString();
            //    monitProEmail.Body1 = mailBody1.ToString();
            //    monitProMailer.SendEmail(monitProEmail);
            //}

            if (funap.ApproveStatus == 8)
            {
                //
                monitProEmail.ToAddress = mocdetatils.CreatedByEmail;
                if (mocdetatils.MOCStatusIdentify == "50")
                {
                   
                }
                else
                {
                    monitProEmail.CC = ApproverEmail.MoccoordinateEmail ;
                }
                monitProEmail.Subject = mocdetatils.MOCNumber + "  is Rejected.";
                mailBody.Append("Dear Sir,");
                mailBody.Append("<br />" + mocdetatils.MOCNumber + " is Rejected.");
                mailBody.Append("<br /><table width='100%' border='1'>");
                mailBody.Append("</br><tr><td><b>Plant/Area</b></td><td>" + mocdetatils.PlantName + "</td><td><b>MOC Class</b></td><td>" + mocdetatils.MOCClassName + "</td></tr>");
                mailBody.Append("</br><tr><td><b>MOC Category of Change</b></td><td>" + mocdetatils.MOCCategoryName + "</td><td><b>MOC Type </b></td><td>" + mocdetatils.MOCTypeName + "</td></tr>");
                mailBody.Append("</br><tr><td><b>MOC Title</b></td><td colspan = 3>" + mocdetatils.MOCTitle + "</td></tr>");
                mailBody.Append("</br><tr><td><b>MOC Priority</b></td><td>" + ApproverEmail.PriorityName + "</td><td><b>Target Date</b></td><td>" + ApproverEmail.TargetDate + "</td></tr>");
                mailBody.Append("</table>");
                mailBody.Append(" <br/><br/> Please log in to the GCPL application and do the needful.");
                mailBody.Append("<br />" + ConfigurationManager.AppSettings["Link"]);
                mailBody.Append("<br />Regards,<br />");
                mailBody.Append(ApproverEmail.ApproverName);

                mailBody1.Append(" <br /> <br />Note: This is a system generated email.");
                monitProEmail.Body = mailBody.ToString();
                monitProEmail.Body1 = mailBody1.ToString();
                monitProMailer.SendEmail(monitProEmail);

            }
            if (funap.ID != 5 && funap.ID != 6  && funap.ID !=12 && funap.ID != 15 && funap.ID != 17 && funap.ID != 18 && funap.ID != 16)
            {
                mailBody.Append("<br /><table width='100%' border='1'>");
                mailBody.Append("</br><tr><td><b>Plant/Area</b></td><td>" + mocdetatils.PlantName + "</td><td><b>Change Type</b></td><td>" + mocdetatils.MOCClassName + "</td></tr>");
                mailBody.Append("</br><tr><td><b>Secondary Changes</b></td><td>" + mocdetatils.MOCCategoryName + "</td><td><b>Change Category </b></td><td>" + mocdetatils.MOCTypeName + "</td></tr>");
                mailBody.Append("</br><tr><td><b>Change Title</b></td><td colspan = 3>" + mocdetatils.MOCTitle + "</td></tr>");
                mailBody.Append("</br><tr><td><b>MOC Priority</b></td><td>" + ApproverEmail.PriorityName + "</td><td><b>Target Date</b></td><td>" + ApproverEmail.TargetDate + "</td></tr>");
                mailBody.Append("</table>");
                mailBody.Append(" <br/><br/> Please log in to the GCPL application and do the needful.");
                mailBody.Append("<br />" + ConfigurationManager.AppSettings["Link"]);
                mailBody.Append("<br />Regards,<br />");
                mailBody.Append(ApproverEmail.ApproverName);

                mailBody1.Append(" <br /> <br />Note: This is a system generated email.");
                monitProEmail.Body = mailBody.ToString();
                monitProEmail.Body1 = mailBody1.ToString();
                monitProMailer.SendEmail(monitProEmail);

            }
            return MOCID;
        }
        public MOCApproverEmail GetMOCApproverEmail(int MOCID, int UserID)
        {
            return MocDAL.GetMOCApproverEmail(MOCID, UserID);
        }
        public List<MOCViewModel> SearchClosedMOC(MOCSearchViewModel mocsearchviewmodel)
        {
            return MocDAL.SearchClosedMOC(mocsearchviewmodel);
        }
        
        public int TemporaryMOCApprove(TemporaryMOCList temp)
        {
            MOCApproverEmail ApproverEmail = new MOCApproverEmail();
            MOCa mocdetatils = new MOCa();

            MocDAL.TemporaryMOCApprove(temp);
            mocdetatils = MocDAL.GetMOC(temp.MOCID);
            ApproverEmail = MocDAL.GetMOCApproverEmail(temp.MOCID, temp.UserID);

            monitProEmail.ToAddress = ApproverEmail.FacMgrEmail+','+ mocdetatils.MOCAdvisorEmail;
            monitProEmail.CC = ApproverEmail.MoccoordinateEmail;
            monitProEmail.Subject = mocdetatils.MOCNumber + " is Requested for Extension.";
            mailBody.Append("Dear Sir,");
            mailBody.Append("<br />The temporary" + mocdetatils.MOCNumber + " is Requested for Extension. ");

            mailBody.Append("<br /><table width='100%' border='1'>");
            mailBody.Append("</br><tr><td><b>Plant/Area</b></td><td>" + mocdetatils.PlantName + "</td><td><b>Change Type</b></td><td>" + mocdetatils.MOCClassName + "</td></tr>");
            mailBody.Append("</br><tr><td><b>Secondary Changes</b></td><td>" + mocdetatils.MOCCategoryName + "</td><td><b>Change Category </b></td><td>" + mocdetatils.MOCTypeName + "</td></tr>");
            mailBody.Append("</br><tr><td><b>Change Title</b></td><td colspan = 3>" + mocdetatils.MOCTitle + "</td></tr>");
            mailBody.Append("</table>");
            mailBody.Append("<br />Reason for Extension :" + temp.ReasonExtension);
            mailBody.Append("<br />Revised Target date     :" + temp.RevisedTargetDate);
            mailBody.Append(" <br/><br/> Please log in to the GCPL application and do the needful.");
            mailBody.Append("<br />" + ConfigurationManager.AppSettings["Link"]);
            mailBody.Append("<br />Regards,<br />");
            mailBody.Append(ApproverEmail.ApproverName);

            mailBody1.Append(" <br /> <br />Note: This is a system generated email.");
            monitProEmail.Body = mailBody.ToString();
            monitProEmail.Body1 = mailBody1.ToString();
            monitProMailer.SendEmail(monitProEmail);
            return temp.MOCID;

        }
      

            public void UpdateTemporaryMOCStatus(int MOCID, int StatusID, string CloseComments, int userID)
        {
            MOCApproverEmail ApproverEmail = new MOCApproverEmail();
            MOCa mocdetatils = new MOCa();
            MocDAL.UpdateTemporaryMOCStatus(MOCID, StatusID, CloseComments, userID);

            mocdetatils = MocDAL.GetMOC(MOCID);
            ApproverEmail = MocDAL.GetMOCApproverEmail(MOCID, userID);
            if (StatusID == 2)
            {
                monitProEmail.ToAddress = ApproverEmail.MoccoordinateEmail+',' + mocdetatils.MOCAdvisorEmail;
           
                monitProEmail.Subject = mocdetatils.MOCNumber + " is Approved for Extension.";
                mailBody.Append("Dear Sir,");
                mailBody.Append("<br />The temporary " + mocdetatils.MOCNumber + " is Approved for Extension. ");

                mailBody.Append("<br /><table width='100%' border='1'>");
                mailBody.Append("</br><tr><td><b>Plant/Area</b></td><td>" + mocdetatils.PlantName + "</td><td><b>Change Type</b></td><td>" + mocdetatils.MOCClassName + "</td></tr>");
                mailBody.Append("</br><tr><td><b>Secondary Changes</b></td><td>" + mocdetatils.MOCCategoryName + "</td><td><b>Change Category </b></td><td>" + mocdetatils.MOCTypeName + "</td></tr>");
                mailBody.Append("</br><tr><td><b>Change Title</b></td><td colspan = 3>" + mocdetatils.MOCTitle + "</td></tr>");
                mailBody.Append("</table>");

                mailBody.Append(" <br/><br/> Please log in to the GCPL application and do the needful.");
                mailBody.Append("<br />" + ConfigurationManager.AppSettings["Link"]);
                mailBody.Append("<br />Regards,<br />");
                mailBody.Append(mocdetatils.MOCAdvisorName);

                mailBody1.Append(" <br /> <br />Note: This is a system generated email.");
                monitProEmail.Body = mailBody.ToString();
                monitProEmail.Body1 = mailBody1.ToString();
                monitProMailer.SendEmail(monitProEmail);
            }
            else if (StatusID == 3)
            {

                monitProEmail.ToAddress = ApproverEmail.MoccoordinateEmail+','+ mocdetatils.MOCAdvisorEmail;
            
                monitProEmail.Subject = mocdetatils.MOCNumber + " is Rejected for Extension.";
                mailBody.Append("Dear Sir,");
                mailBody.Append("<br />The temporary " + mocdetatils.MOCNumber + " is Rejected for Extension. ");

                mailBody.Append("<br /><table width='100%' border='1'>");
                mailBody.Append("</br><tr><td><b>Plant/Area</b></td><td>" + mocdetatils.PlantName + "</td><td><b>Change Type</b></td><td>" + mocdetatils.MOCClassName + "</td></tr>");
                mailBody.Append("</br><tr><td><b>Secondary Changes</b></td><td>" + mocdetatils.MOCCategoryName + "</td><td><b>Change Category </b></td><td>" + mocdetatils.MOCTypeName + "</td></tr>");
                mailBody.Append("</br><tr><td><b>Change Title</b></td><td colspan = 3>" + mocdetatils.MOCTitle + "</td></tr>");
                mailBody.Append("</table>");

                mailBody.Append(" <br/><br/> Please log in to the GCPL application and do the needful.");
                mailBody.Append("<br />" + ConfigurationManager.AppSettings["Link"]);
                mailBody.Append("<br />Regards,<br />");
                mailBody.Append(ApproverEmail.ApproverName);
                mailBody1.Append(" <br /> <br />Note: This is a system generated email.");
                monitProEmail.Body = mailBody.ToString();
                monitProEmail.Body1 = mailBody1.ToString();
                monitProMailer.SendEmail(monitProEmail);
            }
            else if (StatusID == 4)
            {
                monitProEmail.ToAddress = mocdetatils.MOCAdvisorEmail;
                monitProEmail.CC = ApproverEmail.MoccoordinateEmail ;
                monitProEmail.Subject = mocdetatils.MOCNumber + "  is Normalized.";
                mailBody.Append("Dear Sir,");
                mailBody.Append("<br />The temporary " + mocdetatils.MOCNumber + " is normalized.");

                mailBody.Append("<br /><table width='100%' border='1'>");
                mailBody.Append("</br><tr><td><b>Plant/Area</b></td><td>" + mocdetatils.PlantName + "</td><td><b>Change Type</b></td><td>" + mocdetatils.MOCClassName + "</td></tr>");
                mailBody.Append("</br><tr><td><b>Secondary Changes</b></td><td>" + mocdetatils.MOCCategoryName + "</td><td><b>Change Category </b></td><td>" + mocdetatils.MOCTypeName + "</td></tr>");
                mailBody.Append("</br><tr><td><b>Change Title</b></td><td colspan = 3>" + mocdetatils.MOCTitle + "</td></tr>");
                mailBody.Append("</table>");

                mailBody.Append(" <br/><br/> Please log in to the GCPL application and do the needful.");
                mailBody.Append("<br />" + ConfigurationManager.AppSettings["Link"]);
                mailBody.Append("<br />Regards,<br />");
                mailBody.Append(ApproverEmail.ApproverName);
                mailBody1.Append(" <br /> <br />Note: This is a system generated email.");
                monitProEmail.Body = mailBody.ToString();
                monitProEmail.Body1 = mailBody1.ToString();
                monitProMailer.SendEmail(monitProEmail);
            }
        }

        public List<GetMOCClosureList> GetMOCClosureList(int MOCID)
        {

            return MocDAL.GetMOCClosureList(MOCID);




        }
        public List<GetMOCClosureList> SaveMOCClosureList(MOCClosureList mocclosure, List<GetMOCClosureList> GetMOCClosureList)
        {
            return MocDAL.SaveMOCClosureList(mocclosure, GetMOCClosureList);

        }
        public List<MOCClassCount> GetMOCClassCount(DateTime startDate, DateTime endDate)
        {
            return MocDAL.GetMOCClassCount(startDate, endDate);
        }
        public List<MOCPlantCount> GetMOCPlantCount(DateTime startDate, DateTime endDate)
        {
            return MocDAL.GetMOCPlantCount(startDate, endDate);
        }
        public List<MOCCategoryCount> GetMOCCategoryCount(DateTime startDate, DateTime endDate)
        {
            return MocDAL.GetMOCCategoryCount(startDate, endDate);
        }
        public List<MOCStatusCount> GetMOCStatusCount(DateTime startDate, DateTime endDate)
        {
            return MocDAL.GetMOCStatusCount(startDate, endDate);
        }
        public List<MOCPriorityCount> GetMOCPriorityCount(DateTime startDate, DateTime endDate)
        {
            return MocDAL.GetMOCPriorityCount(startDate, endDate);
        }
        public List<MOCRecomStatusCount> GetMOCRecomStatusCount(DateTime startDate, DateTime endDate)
        {
            return MocDAL.GetMOCRecomStatusCount(startDate, endDate);
        }



        public List<MOCMonthlyChart> GetMonthlyMOCStatusCount()
        {
            return MocDAL.GetMonthlyMOCStatusCount();
        }

        public List<PlantWise> GetPlantwiseCount()
        {
            return MocDAL.GetPlantwiseCount();
        }
        public List<MocCategoryCount> GetMocCategoryCount()
        {
            return MocDAL.GetMocCategoryCount();
        }

        public List<MOCpriorityCount> GetMocPriorityCount()
        {
            return MocDAL.GetMocPriorityCount();

        }

        public List<MOCOverallStatus> GetMOCOverallStatus()
        {
            return MocDAL.GetMOCOverallStatus();
        }

        public List<MOCRecommandStatus> GetMOCRecommandStatus()
        {
            return MocDAL.GetMOCRecommandStatus();
        }
        public List<TemporaryMOCList> SearchTempMOC(MOCSearchViewModel mocsearchviewmodel)
        {
            return MocDAL.SearchTempMOC(mocsearchviewmodel);
        }
    }
}
