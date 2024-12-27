using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitPro.Models.MOC
{
   public class TemporaryMOCList
    {
        public int MOCID { get; set; }
        public int UserID { get; set; }
        public string plant { get; set; }
        public string InitiationDate { get; set; }
        public string FirstTargetDate { get; set; }
        public string RevisedTargetDate { get; set; }
        public string ReasonExtension { get; set; }
        public int SNo { get; set; }
        public string TempStatus { get; set; }
        public string ApproverComments { get; set; }
        public int FactoryManagerID { get; set; }
        public string ApproverName { get; set; }
        public string MOCTitle { get; set; }
        public string CloseComments { get; set; }
        public int StatusID { get; set; }
        public string MOCCoordinate { get; set; }
        public string MOCNumber { get; set; }
    }
}
