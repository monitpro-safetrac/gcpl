using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitPro.Models.MOC
{
    public class MOCClosureList : BaseEntity
    {
     
        public List<GetMOCClosureList> GetMOCClosureList { get; set; }
        public int  MOCID { get; set; }
        public int moci { get; set; }
           
            
    }
}