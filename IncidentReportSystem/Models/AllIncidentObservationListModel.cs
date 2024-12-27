using System.Collections.Generic;
using System.Linq;
using System.Web;
using MonitPro.Models.Incident;
using MonitPro.Models.CAPA;
using MonitPro.Models.CAPAViewModel;
using MonitPro.Models.IncidentViewModels;
using MonitPro.Models;
using MonitPro.Models.Account;

namespace IncidentReportSystem.Models
{
    public class AllIncidentObservationListModel : MonitPro.Models.BaseEntity
    {
        public int CurrentUser { get; set; }

        public IncidentObserverViewModel observers { get; set; }

        public List<IncidentViewModel> IncidentView = new List<IncidentViewModel>();
        public List<ObservationViewModel> ObservationViewModelList1 { get; set; }

        public IncidentSearchViewModel IncidentSearchVM = new IncidentSearchViewModel();

        public List<Employee> DeptManagerList { get; set; }
    }
}