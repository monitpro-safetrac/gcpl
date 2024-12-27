using MonitPro.Models.Account;
using MonitPro.Models.Incident;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitPro.Models.MOC
{
    public class NewMOCModel : BaseEntity
    {
        public MOCa moca { get; set; }
        public List<ObservationViewModelMOC> AllMOCOBserList { get; set; }
        public List<Plants> PlantList { get; set; }
        public List<MOCClassification> mocclass { get; set; }
        public MOCObservationEmail MocObserList { get; set; }
        public MOCObservationEmail MocPSSRObserList { get; set; }
        public List<MOCType> moctype { get; set; }
        public List<UserProfile> FunMgrList { get; set; }
        public List<Employee> DeptManagerList { get; set; }
        public List<MOCStatus> statuslist { get; set; }
        public List<MOCPriority> Prioritylist { get; set; }
        public int CurrentUserID { get; set; }
        public int MOCID { get; set; }
        public List<MOCCategory> MocCategory { get; set; }
        public ApprovalList DesignReview = new ApprovalList();
        public ApprovalList DesignReviewApproval = new ApprovalList();
        public ApprovalList Tech = new ApprovalList();
        public ApprovalList approvallistfactorymanager = new ApprovalList();
        public ApprovalList ExeCivil = new ApprovalList();
        public ApprovalList ExeElec = new ApprovalList();
        public ApprovalList ExeMech = new ApprovalList();
        public ApprovalList PSSR = new ApprovalList();
        public ApprovalList PSSRSign = new ApprovalList();
        public FuncationalManagerApprove  FuncationalMgrApprove {get;set;}
        public ApprovalList ApprovalStage { get; set; }
        public ApprovalList risk = new ApprovalList();
        public ApprovalList documentreview = new ApprovalList(); 
        public string DRemarks { get; set; }
        public string FRemarks { get; set; }
        public string TRemarks { get; set; }
        public string FactRemarks { get; set; }
        public string ERemarks { get; set; }
        public string DocRemarks { get; set; }
        public string RRemarks { get; set; }
        public string ApprovalDate { get; set; }
        public long RoleID { get; set; }
        public int ApproverUserID { get; set; }
         public int ApprovalStageID { get; set; }
        public int RecomID { get; set; }
        public int PSSRRecomID { get; set; }
        
    }
  
}
