using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitPro.Models.MOC
{
   public class MOCDashboard : BaseEntity
    {
        public List<MOCClassCount> ClassCount { get; set; }
        public List<MOCPlantCount> PlantCount { get; set; }
        public List<MOCCategoryCount> CategoryCount { get; set; }
        public List<MOCStatusCount> StatusCount { get; set; }
        public List<MOCPriorityCount> PriorityCount { get; set; }
        public List<MOCRecomStatusCount> RecomStatusCount { get; set; }
    }
    public class MOCClassCount
    {
        public string MOCMonth { get; set; }
        public int Temp { get; set; }
        public int Permant { get; set; }
    }
    public class MOCPlantCount
    {
        public string PlantName { get; set; }
        public int TotalCount { get; set; }
    }
    public class MOCCategoryCount
    {
        public string CategoryName { get; set; }
        public int TotalCount { get; set; }
    }
    public class MOCStatusCount
    {
        public string StatusName { get; set; }
        public int TotalCount { get; set; }
    }
    public class MOCPriorityCount
    {
        public string PriorityName { get; set; }
        public int TotalCount { get; set; }
        public int Open { get; set; }
        public int Closed { get; set; }
    }
    public class MOCRecomStatusCount
    {
        public string RecomStatusName { get; set; }
        public int TotalCount { get; set; }
        public int Overdue { get; set; }
        public int Pending { get; set; }
        public int Completed { get; set; }
    }
}
