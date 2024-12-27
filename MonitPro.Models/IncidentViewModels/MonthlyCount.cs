using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitPro.Models.IncidentViewModels
{
    public class MonthlyCount
    {
        public string MonthName { get; set; }
        public int MonthlyCounts { get; set; }
        public int TotalCount { get; set; }
        public int ReportableYes { get; set; }
        public int ReportableNo { get; set; }
    }

    public class StatusCount
    {
        public string StatusName { get; set; }

        public int TotalCount { get; set; }
    }
    public class TypeCount
    {
        public string TypeName { get; set; }

        public int TotalCount { get; set; }
    }
    public class ObservationStatusCount
    {
        public string ActionName { get; set; }

        public int TotalCount { get; set; }
    }
    public class ClassificationCount
    {
        public string ClassificationName { get; set; }

        public int TotalCount { get; set; }
    }
    public class RootCauseCount
    {
        public string RootCauseName { get; set; }

        public int TotalCount { get; set; }
    }
}
