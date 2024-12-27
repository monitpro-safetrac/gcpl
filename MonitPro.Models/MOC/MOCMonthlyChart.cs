using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitPro.Models.MOC
{
    public class MOCMonthlyChart
    {
        public string MonthName { get; set; }
        public int TotalCount { get; set; }
        public int Permanent { get; set; }
        public int Temporary { get; set; }
    }
    public class PlantWise
    {
        public string PlantName { get; set; }
        public int TotalCount { get; set; }
    }

    public class MocCategoryCount
    {
        public string CategoryName { get; set; }
        public int TotalCount { get; set; }
    }

    public class MOCpriorityCount
    {
        public string Priority { get; set; }
        public int TotalCount { get; set; }
        public int StatusOpen { get; set; }
        public int StatusClose { get; set; }
    }

    public class MOCOverallStatus
    {
        public string StatusName { get; set; }
        public int totalCount { get; set; }
    }

    public class MOCRecommandStatus
    {
        public string StatusName { get; set; }
        public int TotalCount { get; set; }
        public int StatusOpen { get; set; }
        public int StatusClose { get; set; }
        public int Overdue { get; set; }
    }

}
