using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using MonitPro.Models.IncidentViewModels;

namespace IncidentReportSystem.Models
{
    public class ObservationStatusChart : MonitPro.Models.BaseEntity
    {
        public List<ObservationStatusCount> ActionCounts = new List<ObservationStatusCount>();
    }
}