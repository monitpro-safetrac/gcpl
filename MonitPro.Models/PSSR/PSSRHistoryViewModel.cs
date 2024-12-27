using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitPro.Models.PSSR
{
    public class PSSRHistoryViewModel
    {
        public int PSSRID { get; set; }
        public string SystemDesc { get; set; }
        public string PSSRType { get; set; }
        public string Plant { get; set; }
        public string PSSRLead {get;set;}
        public string Attachment { get; set; }
        public string ClosedDate { get; set; }
        public int SNO { get; set; }
        public string ScheduledDate { get; set; }
    
    }
    public class PSSRSearchHistory
    {
        public int PlantID { get; set; }
        public int PSSRType { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
    public class PSSRHistoryMainModel : BaseEntity
    {
      public  List<PSSRHistoryViewModel> HistoryList { get; set; }
      public PSSRSearchHistory searchHistory = new PSSRSearchHistory();
      public List<Plants> PlantList { get; set; }
        public List<PSSRType> TypeList { get; set; }

    }
}
