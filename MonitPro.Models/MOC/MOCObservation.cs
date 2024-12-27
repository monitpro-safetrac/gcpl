using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitPro.Models.MOC
{
    public class MOCObservation
    {
   
        public int ObservationID { get; set; }
        public string MOCNo { get; set; }
        public int MOCID { get; set; }

        public string Description { get; set; }

        public string Recommendation { get; set; }

        public string ActionTaken { get; set; }

        public string Comments { get; set; }

        public int CurrentUser { get; set; }

        public string TargetDate { get; set; }

        public int CompletedBy { get; set; }

        public string CompletedUser { get; set; }

        public string CompletedDate { get; set; }

        public List<UserProfile> ActionList { get; set; }

        public int UserID { get; set; }

        public string PlantName { get; set; }
      
        public string MOCDescription { get; set; }
        public string MOCPlant { get; set; }


        public int CategoryID { get; set; }

        public string Remarks { get; set; }

        public int RecomPriorityID { get; set; }

        public int CreatedBy { get; set; }

        public List<MOCRecomCategory> Recomcategory { get; set; }

        public List<MOCRecomPriority> Recompriority { get; set; }

    }
}
