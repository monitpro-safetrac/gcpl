using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MonitPro.Models.CAPA
{
    public class CAPAObservation
    {
        public string CAPANumber { get; set; }

        public int ObservationID { get; set; }

        public int CAPAID { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }   

        public string Recommendation { get; set; }

        public string ActionTaken { get; set; }

        public string Comments { get; set; }

        public int StatusID { get; set; }   

        public int CurrentUser { get; set; }

        public string TargetDate { get; set; }

        public string ExtendedTargetDate { get; set; } = null;


        public int CompletedBy { get; set; }

        public string CompletedUser { get; set; }

        public string CompletedDate { get; set; }

        public List<UserProfile> ActionList { get; set; }

        public int UserID { get; set; }

        public string PlantName { get; set; }

        public string CAPASourceName { get; set; }

        public int CategoryID { get; set; }

        public string Remarks { get; set; }

        public int PriorityID { get; set; }

        public int CreatedBy { get; set; }

        public int CAPStatus { get; set; }
        public int DeptManager { get; set; }

       public int OBPlantID { get; set; }

       public string cpobservationattachement { get; set; }
       public HttpPostedFileBase CAPAObAttachment { get; set; }

    }
}
