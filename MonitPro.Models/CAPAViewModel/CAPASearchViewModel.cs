using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitPro.Models.CAPAViewModel
{
    public class CAPASearchViewModel
    {
        
        public int CAPAPlant { get; set; }
        public int CAPAStatus { get; set; }
        public int CAPASource { get; set; }
        public int PriorityID { get; set; }
        public int CategoryID { get; set; }
        public string CAPAFromDate { get; set; }

        public string CAPAToDate { get; set; }

        public int DeptManager { get; set; }
        public int AuditType { get; set; }

        public int AuditPlant { get; set; }

        public int ActionerID { get; set; }

    }
}
