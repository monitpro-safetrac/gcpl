using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using MonitPro.Models.Incident;
using MonitPro.Models.IncidentViewModels;

namespace IncidentReportSystem.Models
{
    public class IncidentListViewModel : MonitPro.Models.BaseEntity
    {
        public IncidentSearchViewModel IncidentSearchVM = new IncidentSearchViewModel();

        public List<IncidentViewModel> IncidentList = new List<IncidentViewModel>();
        
        public List<ObserverTeamModel> ObserverTeamList = new List<ObserverTeamModel>();

        public List<Status> statusList { get; set; }
        public List<StatusCount> statusCounts = new List<StatusCount>();
        public List<ObservationViewModel> ObservationView { get; set; }
        public List<IncidentClassfication> IncidentClassficationList { get; set; }
        public string ObTitle { get; set; }

        public string ActionTaken { get; set; }

        public int CurrentUser { get; set; }

        public string FileName { get; set; }

    }
}