using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitPro.Models.MOC
{
    public class GetMOCClosureList :BaseEntity
    {
        public int MOCClosureId { get; set; }
        public string Name { get; set; }
        public int Status { get; set; }
        public string Remarks { get; set; }
        public int MOCID { get; set; }
    }
}