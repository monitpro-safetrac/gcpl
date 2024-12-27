using System.Collections.Generic;
using MonitPro.Models.Incident;

namespace MonitPro.Models.IncidentViewModels
{
    public class PdfViewModel
    {
        public IncidentViewModel Incident { get; set; }

        public List<IncidentObserver> IncidentObservers { get; set; }

        public List<IncidentObservation> IncidentObservations { get; set; }

        public List<IncidentImage> IncidentImages { get; set; }
    }
}
