using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MonitPro.Models.MOC
{
    public class MOCa
    {

        public string MOCTeamEmailCheck { get; set; }
        public string MOCAdvisorEmail { get; set; }
        public string CreatedByEmail { get; set; }
        public string FunMgrEmail { get; set; }
        public int MOCID { get; set; }
        public string MOCTypeName { get; set; }
        public string MOCClassName { get; set; }
        public string MOCCategoryName { get; set; }
        public string MOCNumber { get; set; }
        public int ? PlantID { get; set; }
        public int ? MOCClassificationID { get; set; }
        public int ? MOCCategoryID { get; set; }
        public int ? MOCTypeID { get; set; }
        public int? MOCPriorityID { get; set; }
        public int MOCFunCMgrID { get; set; }
        public int DRCost { get; set; }
       public string MOCStatusIdentify { get; set; }
        public string MOCTitle { get; set; }
        public string MOCRequiredOrNot { get; set; }
        public string MOCRequiredDetails { get; set; }
        public string MOCDescription { get; set; }
        public string MOCAdvisorName { get; set; }
        public string Process { get; set; }
       
        public string Civil { get; set; }
    
        public string Mechanical { get; set; }
   
        public string Electrical { get; set; }
   
        public string Instrument { get; set; }
    
        public string Others { get; set; }
        public string FunMgrComment { get; set; }

        public int DeptManagerID { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }
        public string FileName { get; set; }
        public int MOCStatusID { get; set; }
        public string PlantName { get; set; }
        public int ApproverUserID { get; set; }
        public int ApproverStageID { get; set; }
        public string CloseComments { get; set; }
        public List<MOCCategory> mocCategory { get; set; }
        public int MOCPriority { get; set; }
        public string LinkDate { get; set; }
        public string UploadDate { get; set; }
        public string FactoryManagerComments { get; set; }
        public string FunMgrName { get; set; }
        public string MOCStatusInList { get; set; }
        public bool Emergency { get; set; }
        public string EffectiveDate { get; set; }
        public string ExpiryDate { get; set; }
        public int AssetID { get; set; }
         public bool CrossBussinessIdea { get; set; }
        public bool VerifyRiskAssessment { get; set; }
        public int ConditionforMOCApprove { get; set; }
        public int PSSRSignOFFDecision { get; set; }
        public List<MOCReasonForChange> GetMocReasonForChange { get; set; }
        public string Consequences { get; set; }
        public string Likelihood { get; set; }
        public string RARating { get; set; }
    }
}
