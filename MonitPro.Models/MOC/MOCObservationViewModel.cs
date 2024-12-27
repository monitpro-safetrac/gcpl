using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonitPro.Models.Account;

namespace MonitPro.Models.MOC
{
     public class MOCObservationViewModel: MonitPro.Models.BaseEntity
      {
        
            public List<ObservationViewModelMOC> ObservationViewModelListMOC1 { get; set; }

            public MOCObservation MOCObservation { get; set; }

            public int CurrentUser { get; set; }
        }
    }

