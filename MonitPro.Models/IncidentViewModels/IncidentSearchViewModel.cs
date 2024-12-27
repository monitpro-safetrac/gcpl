using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonitPro.Models.CAPA;

namespace MonitPro.Models.IncidentViewModels
{
    public class IncidentSearchViewModel
    {
        public string IncidentTitle { get; set; }

        public int IncidentStatus { get; set; }

        public string IncidentFromDate { get; set; }

        public string IncidentToDate { get; set; }

        public int IncidentType { get; set; }
        public int InciClass { get; set; }

        public int IncidentPlant { get; set; }

        public int ActionerID { get; set; }

        public int DeptManger { get; set; }
    }
}
