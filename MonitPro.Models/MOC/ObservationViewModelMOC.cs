using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitPro.Models.MOC
{
    public class ObservationViewModelMOC
    {
        public string ActionByName { get; set; }
        public int SNo { get; set; }
        public string MOCNo { get; set; }
        public int MOCID { get; set; }
        public string RecomCategory { get; set; }
      public string PSSREmail { get; set; }
        public string ExecMgrEmail { get; set; }
       public string ActionByEmail { get; set; }
        public string DREmail { get; set; }
        public string RAEmail { get; set; }
        public int OBID { get; set; }

        public int ObservationID { get; set; }

        public string Observation { get; set; }

        public string PlantName { get; set; }
        public int DptID { get; set; }

        public string FunctionalMgr { get; set; }

        public string Recommendation { get; set; }

        public string ActionTaken { get; set; }

        public string TargetDate { get; set; }

        public string CompletedDate { get; set; }

        public string Comments { get; set; }

        public int CurrentUser { get; set; }

        public int CompletedBy { get; set; }

        public string CompletedUser { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string PlantArea { get; set; }

        public string AuditType { get; set; }

        public string CreatedBy { get; set; }
        public string FileName { get; set; }
        public string ActionStatus { get; set; }
        public string CategoryName { get; set; }
        public string PriorityName { get; set; }
        public int ActionBy { get; set; }
        public int PriorityID { get; set; }

       
    }
    public class ActionBYEmailList
    {
        public string Actionby { get; set; }
        public int CompletedBYID { get; set; }
    }

}

