using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitPro.Models.PSSR
{
    public class Employee
    {
       public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Designation { get; set; }
    }
    public class Department
    {
        public int DeptID { get; set; }
        public string DeptName { get; set; }
    }
}
