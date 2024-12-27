using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitPro.Models.PSSR
{
    public class CheckListSaveXML
    {
        public int PCMID { get; set; }
        public int ChID { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }
        public string Consequences { get; set; }
        public string Remarks { get; set; }
        public string EditedBy { get; set; }
        public string EditedDatetime { get; set; }
    }
    public class ChecklistAssignXML
    {
        public int PCMID { get; set; }
        public int chID { get; set; }
    }
}


