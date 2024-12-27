using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitPro.Models.PSSR
{
   public class PSSRDashboard:BaseEntity
    {
       public List<ActionByRecomStatus> actionbystatusCountList = new List<ActionByRecomStatus>();
        public List<CAPAPlantwiseCount> CAPACountList = new List<CAPAPlantwiseCount>();
        public List<MonthwiseStatus> MonthStatusCountList = new List<MonthwiseStatus>();
    }
    public class ActionByRecomStatus
    {
        public string ActionBy { get; set; }
        public int Pending { get; set; }
        public int Overdue { get; set; }
        public int Completed { get; set; }
    }
    public class CAPAPlantwiseCount
    {
        public string Plantname { get; set; }
        public int TotalCount { get; set; }
    }
    public class MonthwiseStatus
    {
        public string MonthName { get; set; }
        public int Draft { get; set; }
        public int Schedule { get; set; }
        public int Submittedforapproval { get; set; }
        public int Approved { get; set; }
        public int Closed { get; set; }
    }
}
