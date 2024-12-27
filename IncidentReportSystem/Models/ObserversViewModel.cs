using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MonitPro.Models;
using MonitPro.Models.Account;
using MonitPro.Models.IncidentViewModels;

namespace IncidentReportSystem.Models
{
    public class ObserversViewModel: MonitPro.Models.BaseEntity
    {
        public IncidentObserverViewModel Obsevers { get; set; }
        public List<Employee> LeadList { get; set; }
        public int DepartID { get; set; }
        public List<Department> DepartmentList { get; set; }
        public List<Employee> InvestigatorList { get; set; }
        public List<Employee> GeneralManagerList { get; set; }

        public int IncidentID { get; set; }
        public string IncidentNO { get; set; }
        public long roleid { get; set; }
        public string IncidentTitle { get; set; }
        public string IncidentType { get; set; }
        public string PotentialLevel { get; set; }
        public string ObserverList { get; set; }

        //public int UserID { get; set; }
    }
}
