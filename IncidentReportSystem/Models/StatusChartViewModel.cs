using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using MonitPro.Models.IncidentViewModels;

namespace IncidentReportSystem.Models
{
    public class StatusChartViewModel : MonitPro.Models.BaseEntity
    {
        public List<StatusCount> statusCounts = new List<StatusCount>();
    }
}