using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MonitPro.Models.Account;
using MonitPro.Models.Incident;
using MonitPro.Models.IncidentViewModels;

namespace IncidentReportSystem.Models
{
    public class IncidentObservationViewModel : MonitPro.Models.BaseEntity
    {        
        public List<ObservationViewModel> ObservationViewModelList { get; set; }
        public IncidentObserverViewModel observers { get; set; }
        public string IncidentNo { get; set; }

        public IncidentObservation Observation { get; set; }
        
        public List<ClassficationFactor> FactorList { get; set; }

        public List<Status> StatusList { get; set; }

        public int IncidentID { get; set; }
        
        public string IncidentTitle { get; set; }
        public string IncidentPlant { get; set; }
        public string IncidentDetail { get; set; }

        public string PlantName { get; set; }

        public List<RootCause> RootCauseList { get; set; }

        public int RootCauseID { get; set; }

        public int CurrentUser { get; set; }

        public int actionerid { get; set; }

        public List<Employee> DeptManagerList { get; set; }

        public long RoleID { get; set; }
        public int  StatusID { get; set; }

    }
}