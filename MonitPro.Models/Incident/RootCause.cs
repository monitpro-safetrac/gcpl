using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitPro.Models.Incident
{
    public class RootCause:BaseEntity
    {

        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<RootCauseSubsection> RootCauseList { get; set; }

        public List<RootCauseMaster> RootCauseMasterList { get; set; }

    }
}
