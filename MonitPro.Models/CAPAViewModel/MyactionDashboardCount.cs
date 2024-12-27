using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitPro.Models.CAPAViewModel
{
    public class MyactionDashboardCount
    {

        public int IncidentCount { get; set; }
        public string InciPlant { get; set; }
        public string InciClassfication { get; set; }
        public int CapaOVerdueCount { get; set; }
        public int PSSRCategoryACount { get; set; }
        public int PSSRScheduledCount { get; set; }
        public int HotworkCount { get; set; }
        public int ConfinedSpaceCount { get; set; }
        public int ExcavationCount { get; set; }
        public int TotalPermitCount { get; set; }
        public int MOCTotalCount { get; set; }
        public int PermanentCount { get; set; }
        public int TemporaryCount { get; set; }
        public int T1 { get; set; }
        public int T2 { get; set; }
        public int T3 { get; set; }
        public int T4 { get; set; }
        public int TotalIncident { get; set; }
        public int Near_Miss { get; set; }

    }
    public class MyApprovalCount
    {
        public int IncidentApprovalCount { get; set; }
        public int PSSRApprovalCount { get; set; }
        public int MOCApprovalCount { get; set; }
        public int PermitApprovalCount { get; set; }
        public int CMSApprovalCount { get; set; }
    }
}
