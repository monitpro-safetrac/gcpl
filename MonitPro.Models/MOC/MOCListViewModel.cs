
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitPro.Models.MOC
{
   public  class MOCListViewModel: MonitPro.Models.BaseEntity
    {
        public MOCSearchViewModel MOCSearchVM= new MOCSearchViewModel();

        public List<MOCViewModel> MOCList = new List<MOCViewModel>();
        public List<ObservationViewModelMOC> MOCObserList { get; set; }
        public int CurrentUser { get; set; }
      
        public List<MOCStatus> MocstatusList { get; set; }

        public List<MOCClassification> MOCClass { get; set; }

        public List<MOCType> MocType { get; set; }

        public List<MOCCategory> MocCategory { get; set; }

        public List<UserProfile> approver { get; set; }

        public List<UserProfile> Coordinator { get; set; }

        public List <ApproverModel>ApproverModel { get; set; }
        
    }
}
