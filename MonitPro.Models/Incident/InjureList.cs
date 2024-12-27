using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MonitPro.Models.Incident
{
    public class InjureList
    {
        public int SNo { get; set; }
        public int InjuryPeopleID { get; set; }

        public string Name { get; set; }

        public string Age { get; set; }

        public int IncidentID { get; set; }
        public string CompanyName { get; set; }
        public int GenderID { get; set; }
        public string FullName { get; set; }
        public string GenderName { get; set; }
        public string ContractorEmp { get; set; }
        public string FirstAid { get; set; }
        public string Hospitalized { get; set; }
    }
}