using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MonitPro.Models.CAPAViewModel;

namespace IncidentReportSystem.Models
{
    public class ActionsStatusChartViewModel : MonitPro.Models.BaseEntity
    {
        public List<ActionsCount> ActionCounts = new List<ActionsCount>();

        public List<CapaSourceCounts> CapaSourceCount = new List<CapaSourceCounts>();

        public List<PriorityCount> prioritycount = new List<PriorityCount>();

        public List<ObservationCount> observationcount = new List<ObservationCount>();

        public List<CategoryCount> categorycount = new List<CategoryCount>();
    }

}