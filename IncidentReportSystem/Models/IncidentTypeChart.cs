
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using MonitPro.Models.IncidentViewModels;

namespace IncidentReportSystem.Models
{
    public class IncidentTypeChart : MonitPro.Models.BaseEntity
    {
        public List<TypeCount> CategoryCounts = new List<TypeCount>();
    }
}