using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitPro.Models.MOC
{
   public class EmailModel
    {
      
    }
    public class MOCAdvisor
    {
        public string MOCAdvisorEmailAddress { get; set; }
        public string MOCAdvisorName { get; set; }
    }
    public class MOCApproverEmail
    {
        public int MOCID { get; set; }
        public string MOCCoordinator { get; set; }
        public string NextApproverEmail { get; set; }
        public string ApproverEmail { get; set; }
        public string PSSRSignOffEmail { get; set; }
        public string MoccoordinateEmail { get; set; }
        public string FunMgrEmail { get; set; }
        public string PriorityName { get; set; }
        public string TargetDate { get; set; }
        public string ApproverName { get; set; }
       public string FacMgrEmail { get; set; }
        public string OPHTE { get; set; }
        public string ExeMgrEmail { get; set; }
        public string DREmail { get; set; }
        public string DRRemarks { get; set; }
        public string DRARemarks { get; set; }
        public string RiskRemarks { get; set; }
        public string TechRemarks { get; set; }
        public string CEMEmail { get; set; }
        public List<OPHTEEmailList> oPHTEEmailLists { get; set; }
        public List<CivilElecMechEmail> civilElecMechEmailLists { get; set; }
    }
    public class MOCObservationEmail
    {
        public string ActionByEmail { get; set; }
        public string DREmail { get; set; }
        public string RAEmail { get; set; }
        public string ExecMgr { get; set; }
        public string PSSREmail { get; set; }
        public List<ObservationViewModelMOC> obserlist { get; set; }
        public List<ActionBYEmailList> actionBYEmailLists { get; set; }
    }
    public class OPHTEEmailList
    {
        public string OPHTEEmail { get; set; }
    }
    public class CivilElecMechEmail
    {
        public string CEMEmail { get; set; }
    }
}
