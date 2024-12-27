using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitPro.Models.Incident
{
        public class RootCauseSubsection
        {
            public int SubsectionID { get; set; }
            public int RootCauseID { get; set; }
            public string RootCauseName { get; set; }
            public string Name { get; set; }        
            public bool subcheck { get; set; }
    }
    
}
