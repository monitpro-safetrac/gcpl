using MonitPro.Models.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitPro.Models.MOC
{
     public class ApprovalList
    {

        public int ID { get; set; }
        public string ApprovalName { get; set; }
        public string ApprovalTargetDate { get; set; }
        public int IsTeamApprover { get; set; }
        [Required]
        public string DRTargetDate { get; set; }
        [Required]
        public  int  UserID { get; set; }
        public string DRName { get; set; }
        public string DRRemarks { get; set; }
        public int DRUserID { get; set; }
        public string DRApprovalDate { get; set; }
   
        public string EnableRemarks { get; set; }
        public int UpdateID { get; set; }

        public string DRAName { get; set; }
        public string DRARemarks { get; set; }
        public int DRAUserID { get; set; }
        public string DRAApprovalDate { get; set; }
        public string DRATargetDate { get; set; }
        public string RiskName { get; set; }
        public string RiskRemarks { get; set; }
        public int RiskUserID { get; set; }
        public string RiskApprovalDate { get; set; }
        public string RiskTargetDate { get; set; }
        public string TechName { get; set; }
        public string TechRemarks { get; set; }
        public int TechUserID { get; set; }
        public string TechApprovalDate { get; set; }
        public string TechTargetDate { get; set; }
        public string FacMgrName { get; set; }
        public string FacMgrRemarks { get; set; }
        public int FacMgrUserID { get; set; }
        public string FacMgrApprovalDate { get; set; }
        public string FacMgrTargetDate { get; set; }

        public string ExcivilName { get; set; }
        public string ExcivilRemarks { get; set; }
        public int CivilUserID { get; set; }
        public string ExcivilApprovalDate { get; set; }
        public string ExcivilTargetDate { get; set; }
        public string ExMechName { get; set; }
        public string ExMechRemarks { get; set; }
        public int MechUserID { get; set; }
        public string ExMechApprovalDate { get; set; }
        public string ExMechTargetDate { get; set; }

        public string ExElecName { get; set; }
        public string ExElecRemarks { get; set; }
        public int ElecUserID { get; set; }
        public string ExElecApprovalDate { get; set; }
        public string ExElecTargetDate { get; set; }

        public string PSSRName { get; set; }
        public string PSSRRemarks { get; set; }
        public int PSSRUserID { get; set; }
        public string PSSRApprovalDate { get; set; }
        public string PSSRTargetDate { get; set; }
        public string PSSRSignName { get; set; }
        public string PSSRSignRemarks { get; set; }
        public int PSSRSignUserID { get; set; }
        public string PSSRSignApprovalDate { get; set; }
        public string PSSRSignTargetDate { get; set; }
        public string BDocName { get; set; }
        public string BDocRemarks { get; set; }
        public int BDocUserID { get; set; }
        public string BDocApprovalDate { get; set; }
        public string BDocTargetDate { get; set; }
    }
}
