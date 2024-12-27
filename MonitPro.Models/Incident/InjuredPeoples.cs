using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MonitPro.Models.Incident
{
    public class InjuredPeoples
    {

        public int InjuryPeopleID { get; set; }
        [Required(ErrorMessage = "The Field is Required")]
        public string Name { get; set; }

        public List<Gender> Gender { get; set; }
        [Required(ErrorMessage = "The Field is Required")]
        public string Age { get; set; }
        public List<Contractor> Contractor { get; set; }
        public int IncidentID { get; set; }
        [Required(ErrorMessage = "The Field is Required")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "The Field is Required")]
        public int CompanyName { get; set; }
        [Required(ErrorMessage = "The Field is Required")]
        public int GenderID { get; set; }
        [Required(ErrorMessage = "The Field is Required")]
        public int ContractorEmpID { get; set; }
        [Required(ErrorMessage = "The Field is Required")]
        public string FirstAid { get; set; }
        [Required(ErrorMessage = "The Field is Required")]
        public string Hospitalized { get; set; }

    }
}