using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitPro.Models.Incident
{
    public class IncidentEmail
    {
        public string AssignDate { get; set; }
        public string IncidentOwner { get; set; }
        public string Teamlead { get; set; }
        public string TeamMembers { get; set; }
        public string Invesfacilitator { get; set; }
        public string IncidentDescription { get; set; }
        public string InvestigatorEmail { get; set; }

        public string ObserverEmail { get; set; }
        public string OwnerEmail { get; set; }
        public string ActionerEmail { get; set; }
        public string CreatorEmail { get; set; }
        public string TeamListEmail { get; set; }
        public string SubmitForApprovalDate { get; set; }
    }
}
