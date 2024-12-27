using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitPro.Models.MOC
{
    public class MOCStatus
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

    }
    public class MOCTempStatus
        {
        public int TempID { get; set; }
        public string TempName { get; set; }
        public string TempDescription { get; set; }
        }
}
