using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonitPro.Models;
namespace MonitPro.Models.CAPAViewModel
{
    public class ActionsCount : BaseEntity
    {
        public int SNo { get; set; }
        public string StatusName { get; set; }
        public int IIR { get; set;}
        public int CAPA { get; set; }
        public string Name { get; set; }
        public int PSSRID { get; set; }
        public int MOCID { get; set; }
        public int TotalCount { get; set; }
    }

    public class CapaSourceCounts
    {
        public string SourceName { get; set; }

        public int TotalCount { get; set; }

    }

    public class PriorityCount
    {
        public string PriorityName { get; set; }

        public int TotalCount { get; set; }

        public int Overdue { get; set; }

        public string Name { get; set; }

        public int Closed { get; set; }

        public int Opened { get; set; }

        public int ReOpen { get; set; }

        public int New { get; set; }
    }


    public class ObservationCount
    {
        public string MonthName { get; set; }

        public int MonthlyCount { get; set; }

    }


    public class CategoryCount
    {
        public string SourceName { get; set; }

        public int TotalCount { get; set; }


    }


}
