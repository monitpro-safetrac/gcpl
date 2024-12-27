using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MonitPro.Models.Incident;

namespace IncidentReportSystem.Models
{
    public class IncidentImageViewModel : MonitPro.Models.BaseEntity
    {
        public int IncidentID { get; set; }
        public string IncidentNO { get; set; }
        public string IncidentTitle { get; set; }
        public string PlantArea { get; set; }
        public string IncidentDetail { get; set; }

        public IncidentImage IncidentImage = new IncidentImage();

        public List<IncidentImage> IncidentImages = new List<IncidentImage>();
    }
}
