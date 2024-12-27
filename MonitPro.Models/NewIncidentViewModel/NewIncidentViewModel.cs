using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MonitPro.Models.Incident;
using System.ComponentModel.DataAnnotations;
using MonitPro.Models;

using MonitPro.Models.IncidentViewModels;
namespace IncidentReportSystem.Models
{
    public class NewIncidentViewModel : BaseEntity
    {
        public Incident Incident { get; set; }
        public WhyForm WhyForm { get; set; }

        public string Title { get; set; }
        public int IncidentID { get; set; }

        public List<ContractorEmp> contractorEmp { get; set; }

        public InjuredPeoples Injuredpeoples { get; set; }
      
        public List<InjureList> InjureList { get; set; }
        public List<Contractor> Contractor { get; set; }
        public List<Gender> Gender { get; set; }      
        public List<ObserverTeamModel> ObserverTeamList = new List<ObserverTeamModel>();
        public List<IncidentType> IncidentTypeList { get; set; }

        public List<IncidentClassfication> IncidentClassficationList { get; set; }

        public List<Plants> PlantList { get; set; }

        public List<Priority> prioritiesList { get; set; }

        public List<Status> statusList { get; set; }

        public List<InjuryType> InjuryTypesList { get; set; }

        public List<RootCause> rootcause { get; set; }

        public int CurrentUserID { get; set; }


        public string ImageName { get; set; }

        public string FileName { get; set; }


        public string ECNumber { get; set; }

        public string Comments { get; set; }

        [Required]
        public HttpPostedFileBase ImageFile { get; set; }      

        public List<RootCauseMaster> RootCauseMasterList { get; set; }
        public List<RootCauseXMLList> RootCauseXML { get; set; }

        public List<TenetsList> Tenets { get; set; }

        public List<Tenets4> Tenets4 { get; set; }
         public List<TenetsXML> tenetsxml { get; set; }
         public long RoleID { get; set; }
    }
}

