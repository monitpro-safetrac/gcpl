using System;
using MonitPro.Models.Incident;

namespace MonitPro.Models.IncidentViewModels
{
    public class IncObservationViewModel
    {
        public int IncidentID { get; set; }

        public string IncidentTitle { get; set; }

        public string PlantName { get; set; }

        public int CompletedBy { get; set; }

        public string CompletedUser { get; set; }

        public IncidentObservation incidentObservation { get; set; }
    }
}
