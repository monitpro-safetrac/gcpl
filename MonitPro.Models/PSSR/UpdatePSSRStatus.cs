using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitPro.Models.PSSR
{
  public  class UpdatePSSRStatus:BaseEntity
    {
        public int PSSRID { get; set; }
        public string ClosureComments { get; set; }
    }
}
