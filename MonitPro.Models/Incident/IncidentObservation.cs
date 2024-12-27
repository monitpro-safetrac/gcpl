using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MonitPro.Models.CAPA;
namespace MonitPro.Models.Incident
{
    public class IncidentObservation
    {
        public int ObservationID { get; set; }

        public int IncidentID { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string FailureFactor { get; set; }
        public HttpPostedFileBase InciObAttachment { get; set; }
        public List<UserProfile> ActionList { get; set; }
        public string  InciAttachment { get; set; }
        public string Recommendation { get; set; }

        public string ActionTaken { get; set; }

        public string Comments { get; set; }

        public int StatusID { get; set; }

        public int ResponsibleUser { get; set; }

        public int CurrentUser { get; set; }

        public string TargetDate { get; set; }

        public int CompletedBy { get; set; }

        public int UserID { get; set; }

        public string CompletedUser { get; set; }

        public string CompletedDate { get; set; }

        public int ClassficationFactorID { get; set; }

        public List<CAPAPriority> PriorityList { get; set; }

        public string RootCause { get; set; }

        public int PriorityID { get; set; }

        public int ObserverManager { get; set; }
        public List<CAPAObservationStatus> observationstatuslist { get; set; }
        public int DeptManager { get; set; }
        public int CpStatusID { get; set; }

    }
}
