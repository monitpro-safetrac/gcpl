using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitPro.Models.CAPAViewModel
{
    public class ActionerModel
    {
        public int SNo { get; set; }

        public int HSEManager { get; set; }

        public int CAPAID { get; set; }
        public int CompletedBy { get; set; }
        public int DeptManager { get; set; }

    }
}