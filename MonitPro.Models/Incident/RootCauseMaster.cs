using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitPro.Models.Incident
{
    public class RootCauseMaster
    {
        public int sno { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool maincheck { get; set; }
        public int RootCauseID { get; set; }
        public List<RootCauseSubsection> SubList { get; set; }


    }
}
