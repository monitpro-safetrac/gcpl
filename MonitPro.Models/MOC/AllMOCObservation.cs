using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonitPro.Models.CAPA;


namespace MonitPro.Models.MOC
{
     public class AllMOCObservation: MonitPro.Models.BaseEntity
    {
        public int CurrentUser { get; set; }

        public List<UserProfile> ActionList { get; set; }
        public List<ObservationViewModelMOC> ObservationViewModelList1 { get; set; }
        public List<MOCRecomCategory> Recomcategory { get; set; }

        public List<MOCRecomPriority> Recompriority { get; set; }
        public MOCSearchViewModel MOCSearchVM = new MOCSearchViewModel();
        public List<CAPAObservationStatus> MOCTrackActionStatus { get; set; }
    }
}
