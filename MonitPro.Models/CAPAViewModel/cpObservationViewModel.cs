using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonitPro.Models.CAPA;
using MonitPro.Models;

namespace MonitPro.Models.CAPAViewModel
{
     public class cpObservationViewModel
    {
        public int CAPAID { get; set; }

        public string CAPASourceName { get; set; }

        public string PlantName { get; set; }

        public int CompletedBy { get; set; }

        public string CompletedUser { get; set; }

        public CAPAObservation capaobs { get; set; }


    }
}
