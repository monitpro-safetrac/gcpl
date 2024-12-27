using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MonitPro.Models.Incident;
using MonitPro.Models.CAPA;
using System.ComponentModel.DataAnnotations;


namespace IncidentReportSystem.Models
{
    public class NewCAPAModel : MonitPro.Models.BaseEntity
    {
        public CreateCAPA CreateCAPA { get; set; }
     
        public List<IncidentType> IncidentTypeList { get; set; }

        public List<AuditType> AuditType { get; set; }

        public List<CAPASource> CAPASource { get; set; }      

        public List<Plants> PlantList { get; set; }

        public List<CAPAPlants> capaplants { get; set; }

        public List<Status> statusList { get; set; }
        public List<ContractorEmp> contractorEmp { get; set; }
        public int CurrentUserID { get; set; }   

        public string FileName { get; set; }

        public string AuditTime { get; set; }

        [Required]
        public HttpPostedFileBase ImageFile { get; set; }

        public string Comments { get; set; }

    }
}

