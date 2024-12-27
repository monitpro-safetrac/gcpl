using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MonitPro.Models.IncidentViewModels;
namespace IncidentReportSystem.Models
{
    public class IncidentClassificationCount : MonitPro.Models.BaseEntity
    {
        public List<ClassificationCount> classificationCounts = new List<ClassificationCount>();
    }
}