using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitPro.Models.CAPA
{
    public class CAPAEmail
    {

        public int ID { get; set; }
        public int CAPAID { get; set; }
        public string FunctionalMgr { get; set; }
        public string ActionerEmail { get; set; }
        public string Area { get; set; }
        public  string Category { get; set; }
        public string Observation { get; set; }
        public string Recommendation { get; set; }

        public string Priority { get; set; }
        public string TargetDate { get; set; }

        public string FirstownerEmail { get; set; }
        public string SecondownerEmail { get; set; }
        public string HSEMgrEmail { get; set; }

    }
}
