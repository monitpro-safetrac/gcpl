using System;
using System.ComponentModel.DataAnnotations;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MonitPro.Models.CAPA
{
    public class CreateCAPA
    {
        public string CAPANumber { get; set; }
        public int CAPAID { get; set; }

        public int IncidentID { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string AuditTime { get; set; }

        [Required]
        public int AuditTypeID { get; set; }

        public int StatusID { get; set; }
        public string Comments { get; set; }

        [Required]
        public int PlantID { get; set; }
        [Required]
        public int CAPASourceID { get; set; }     

        public int CurrentUserID { get; set; }

        public string ImageName { get; set; }
        [Required]
        public int ContractorEmpID { get; set; }
    
        public string ReportedDetail { get; set; }

        public string FileName { get; set; }
        public int CreatedBy { get; set; }

        public HttpPostedFileBase ImageFile { get; set; }

        public int CAPAPlantID { get; set; }
    }
}
