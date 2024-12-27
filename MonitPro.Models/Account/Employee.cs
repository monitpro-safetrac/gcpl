using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitPro.Models.Account
{
    public class Employee
    {
        public int ID { get; set; }
        public string DeptEmpID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Designation { get; set; }
        public DateTime Dob { get; set; }
        public int DepartmentID { get; set; }
        public string Email { get; set; }
        public string PhNumber { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
