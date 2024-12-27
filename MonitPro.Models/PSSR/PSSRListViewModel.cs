using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitPro.Models.PSSR
{
    public class PSSRListViewModel: BaseEntity
    {
        public int SNO { get; set; }
        public int PSSRID { get; set; }
        public string PlantName { get; set; }
        public string SystemDesc { get; set; }
        public string Category { get; set; }
        public string PSSRType { get; set; }
        public string PSSRLead { get; set; }
        public string PSSRApprover { get; set; }
        public string PSSRStatus { get; set; }
        public string ScheduledDatetime { get; set; }
        public string Attachment { get; set; }
        public int chairper { get; set; }
        public int EnggLead { get; set; }
        public int HSELead { get; set; }
        public int OPLead { get; set; }
    }
    public class SearchList
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int PlantID { get; set; }
        public int Status { get; set; }
        public int Category { get; set; }
        public int Type { get; set; }
    }

}
