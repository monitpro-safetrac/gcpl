using MonitPro.Models.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitPro.Models.MOC
{
     public class MOCApproverList:MonitPro.Models.BaseEntity
    {
        public List<ApprovalList> ApprovalList = new List<ApprovalList>();
        public List<Employee> EmployeeList { get; set; }
        public int MOCID { get; set; }
        public string MOCNo { get; set; }
  
      
        [Required]
        public int Approver { get; set; }
        [Required]
        public string TargetDate { get; set; }
        public string MOCDescription { get; set; }
        public string PlantName { get; set; }
      
        public string MOCTitle { get; set; }
    }
}
