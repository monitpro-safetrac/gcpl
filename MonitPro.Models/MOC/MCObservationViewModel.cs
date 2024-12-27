using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitPro.Models.MOC
{
     public class MCObservationViewModel
    {
        public int MOCID { get; set; }

        public int CompletedBy { get; set; }

        public string CompletedUser { get; set; }

        public MOCObservation mocobservation { get; set; }
    }
}
