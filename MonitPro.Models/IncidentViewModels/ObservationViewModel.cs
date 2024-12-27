using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MonitPro.Models.Incident;

namespace MonitPro.Models.IncidentViewModels
{
    public class ObservationViewModel
    {
        public int SNo { get; set; }
        public string IncidentNO { get; set; }

        public int IncidentID { get; set; }

        public int OBID { get; set; }
        public string Attachment { get; set; }
        public int ObservationID { get; set; }

        public string Observation { get; set; }

        public string Description { get; set; }

        public string Recommendation { get; set; }

        public string PriorityName { get; set; }

        public string ActionTaken { get; set; }

        public string TargetDate { get; set; }

        public string CompletedDate { get; set; }

        public string PlantName { get; set; }

        public string IncidentType { get; set; }

        public string CompletedBy { get; set; }

        public string CurrentStatus { get; set; }

        public string Comments { get; set; }

        public List<RootCause> RootCause { get; set; }

        public int RootCauseID { get; set; }

        public string CompletedUser { get; set; }
        public int ActionBy { get; set; }

        public string InciTitle { get; set; }

        public string InciDetails { get; set; }
        public int InciStatusID { get; set; }
        public string Actionstatus { get; set; }
        public int Approver { get; set; }
        public string DeptManagerName { get; set; }

    }
}
