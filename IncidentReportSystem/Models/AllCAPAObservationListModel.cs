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
    public class AllCAPAObservationListModel: MonitPro.Models.BaseEntity
    {
        public List<ObservationViewModelCapa> ObservationViewModelList1 { get; set; }

        public List<CAPAViewModel> CapaList = new List<CAPAViewModel>();
      
        public int CurrentUser { get; set; }
       public List<ActionsCount> ActionCounts { get; set; }

        public cpObservationViewModel cpObservationViewModel { get; set; }


        public List<CAPACategory> capacategory { get; set; }

        public List<CAPAPriority> capapriority { get; set; }
        public CAPASearchViewModel CAPASearch = new CAPASearchViewModel();

        public List<CAPAObservationStatus>observationstatuslist { get; set; }
        public List<Employee> DeptManagerList { get; set; }
        public MyactionDashboardCount GetDashboardOVerallStatusCount {  get; set; }
        public MyApprovalCount GetMyApprovalCount { get; set; }

    }



}