using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using MonitPro.Models.Incident;
using MonitPro.Models.CAPA;
using MonitPro.Models.CAPAViewModel;
using MonitPro.Models.IncidentViewModels;
using MonitPro.Models.Account;

namespace IncidentReportSystem.Models
{
    public class CAPAObservationViewModel: MonitPro.Models.BaseEntity
    {
        public List<ObservationViewModelCapa> ObservationViewModelList1 { get; set; }

        public CAPAObservation CAPAObservation { get; set; }

        public int CAPAID { get; set; }
        
        public string PlantName { get; set; }

        public string CAPASourceName { get; set; }
        public CreateCAPA createCapa { get; set; }
        public int CurrentUser { get; set; }
        public List<CAPAObservationStatus> observationstatuslist { get; set; }
        public List<Employee> DeptManagerList { get; set; }

        public List<CAPACategory> capacategory { get; set; }

        public List<CAPAPriority> capapriority { get; set; }
        public long RoleId { get; set; }
        public int CompletedBy { get; set; }

        public int DeptID { get; set; }

        public List<Plants>plantlist { get; set; }

    }
}