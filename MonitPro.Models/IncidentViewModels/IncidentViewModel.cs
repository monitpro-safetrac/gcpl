using System;

namespace MonitPro.Models.IncidentViewModels
{
    public class IncidentViewModel
    {
        public int SNo { get; set; }

        public string IncidentNO { get; set; }
        public int IncidentID { get; set; }

        public string ECNumber { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime IncidentTime { get; set; }

        public string IncidentType { get; set; }

        public string PlantArea { get; set; }

        public string CurrentStatus { get; set; }

        public DateTime IncidentCloseTime { get; set; }

        public string CreatedBy { get; set; }

        public string Comments { get; set; }

        public string ObTitle { get; set; }

        public string FileName { get; set; }
        public string ActionTaken { get; set; }
        public int Incicreator { get; set; }
        public int ObCount { get; set; }
        public string Inciclassification { get; set; }
    }
}
