using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitPro.Models.IncidentViewModels
{
    public class IncidentObserverViewModel
    {
        public int ID { get; set; }
        public int IncidentID { get; set; }
        public int ObserverD { get; set; }
        public string EmployeeName { get; set; }
        public string IncidentTitle { get; set; }
        public int AssignedLead { get; set; }
        public int Investigator { get; set; }
        public int Manager { get; set; }
    }
}
