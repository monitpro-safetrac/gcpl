using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitPro.Models.CAPAViewModel
{
    public class ObservationViewModelCapa
    {
        public string CAPANumber { get; set; }
        public int SNo { get; set; }

        public int CAPAID { get; set; }

        public int InciID { get; set; }

        public int OBID { get; set; }
        public int PSSRID { get; set; }
        public int MOCID { get; set; }

        public int ObservationID { get; set; }

        public string Observation { get; set; }

        public string GetStatus { get; set; } 

        public string PlantName{ get; set; }
        public int DptID { get; set; }
        
        public string FunctionalMgr { get; set; }

        public string CAPASourceName { get; set; }

        public string Recommendation { get; set; }

        public string ActionTaken { get; set; }

        public string TargetDate { get; set; }

        public string CompletedDate { get; set; }

        public string CurrentStatus { get; set; }

        public string Comments { get; set; }

        public int CurrentUser { get; set; }

        public int CompletedBy { get; set; }

        public string CompletedUser { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime AuditTime { get; set; }

        public string PlantArea { get; set; }

        public string CAPASource { get; set; }

        public string AuditType { get; set; }

        public string CreatedBy { get; set; }
        public string FileName { get; set; }

        public string CategoryName { get; set; }
        public string PriorityName { get; set; }
        public string CPStatus { get; set; }
        public int InciStatusID { get; set; }
        public string AtachmentName { get; set; }
        public string ObPlantName { get; set; }

    }
}
