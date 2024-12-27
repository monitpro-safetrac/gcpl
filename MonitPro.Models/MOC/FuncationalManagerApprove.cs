using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitPro.Models.MOC
{
     public class FuncationalManagerApprove
    {
        public int MOCID { get; set; }
        public string Remarks { get; set; }
        public string TargetDate { get; set; }
        public string FunApprovalDate { get; set; }
   
        public int FuncationalManagerID { get; set; }
        public int UserID { get; set; }
        public int ApproveStatus { get; set; }
        public int ID { get; set; }
        
    }
}
