using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitPro.Models.IncidentViewModels
{
    public class ObserverTeamModel
    {
        public int SNo { get; set; }
        public int ObserverTeamLead { get; set; }
        public int DeptManager { get; set; }
        public int ObserverD { get; set; }
        public int HSEManager { get; set; }
        public int ID { get; set; }
        public int IncidentID { get; set; }
        public int CompletedBy { get; set; }
    }
}