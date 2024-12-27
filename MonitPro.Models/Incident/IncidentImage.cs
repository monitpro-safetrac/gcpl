using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MonitPro.Models.Incident
{
    public class IncidentImage
    {
        public int IncidentImageId { get; set; }
        public int SNo { get; set; }
        public int IncidentId { get; set; }
        [Required(ErrorMessage = "Required")]
        public string ImageName { get; set; }

        public string FileName { get; set; }

        [Required(ErrorMessage = "Required")] 
        public HttpPostedFileBase ImageFile { get; set; }
        [Required(ErrorMessage = "Required")]
        public string ImageDescription { get; set; }
    }
}
