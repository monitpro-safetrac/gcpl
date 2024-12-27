
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PagedList;

namespace MonitPro.Models.PSSR
{
    public class CheckList 
    {   public int PSSRID { get; set; }
        public int UserID { get; set; }
        public int SNO { get; set; }
        public int PCMID { get; set; }
        public string Description { get; set; }
        public int CheckListID { get; set; }
        public int Ischecked { get; set; }
        public string Remarks { get; set; }
        public string Consequences { get; set; }
        public string EditedBy { get; set; }
        public string EditedDateTime { get; set; }

    }

    public class CheckListDD
    {
        public int PCMID { get; set; }
        public string Category { get; set; }
        public bool yesNo { get; set; }
    }

    public class MainCheckListModel : BaseEntity
    {
        public int PSSRID { get; set; }
        public int PCMID { get; set; }
  
        public List<CheckList> CheckLists { get; set; }
        public List<CheckListDD> checkListDD { get; set; }
        public List<CheckList> CheckCheckLists { get; set; }
    }

}


