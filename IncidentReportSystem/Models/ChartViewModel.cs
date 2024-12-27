using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using MonitPro.Models.IncidentViewModels;

namespace IncidentReportSystem.Models
{
    public class ChartViewModel : MonitPro.Models.BaseEntity
    {
        public List<MonthlyCount> MonthlyCounts = new List<MonthlyCount>();
    }
}