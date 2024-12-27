using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitPro.Models.PSSR
{
    public class PSSRRequestTargetDate
    {
        public int RecomID { get; set; }
        public string RevisedRemarks { get; set; }
        public string RevisedTargetDate { get; set; }
        public int Identity { get; set; }
    }
    public class TargetDateApprove
    {
        public int RecomID { get; set; }
        public string RequestRemarks { get; set; }
        public string RequestTargetDate { get; set;}
        public string TargetDate { get; set; }
        public int Identity { get; set; }
        public string ExsistingTargetDate { get; set; }

    }
}
