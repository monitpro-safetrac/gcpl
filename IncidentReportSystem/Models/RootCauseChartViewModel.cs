using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MonitPro.Models.IncidentViewModels;
namespace IncidentReportSystem.Models
{
    public class RootCauseChartViewModel : MonitPro.Models.BaseEntity
    {
        public List<RootCauseCount> rootCauseCounts = new List<RootCauseCount>();
    }
}