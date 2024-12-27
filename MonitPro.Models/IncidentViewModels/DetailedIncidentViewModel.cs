using MonitPro.Models.Incident;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitPro.Models.IncidentViewModels
{
    public class DetailedIncidentViewModel
    {
        public IncidentViewModel incidentViewModel { get; set; }

        public List<IncidentUser> incidentUsers { get; set; }

        public List<ObservationViewModel> ObservationDetail { get; set; }

        public List<IncidentImage> incidentImages { get; set; }
    }
}
