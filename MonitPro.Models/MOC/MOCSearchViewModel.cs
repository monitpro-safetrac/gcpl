using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonitPro.Models.CAPA;

namespace MonitPro.Models.MOC
{
    public class MOCSearchViewModel
    {
        public int Plant { get; set; }

        public int MOCStatus { get; set; }
        public int MOCTempStatus { get; set; }
        public string MOCFromDate { get; set; }

        public string MOCToDate { get; set; }

        public int MOCType { get; set; }
        public int MOCcoordinator { get; set; }

        public string MOCNumber { get; set; }

        public string MOCTitle { get; set; }

        public int Approver { get; set; }

        public int MOCCategory { get; set; }

        public string Status { get; set; }

        public int RecomCategoryID { get; set; }

        public int RecomPriorityID { get; set; }

        public int RecomStatus { get; set; }

        public int ActionerID { get; set; }

        public int ClassID { get; set; }

    }
}
