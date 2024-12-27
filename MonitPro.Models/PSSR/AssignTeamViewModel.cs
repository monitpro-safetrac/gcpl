using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitPro.Models.PSSR
{
   public class AssignTeamViewModel:BaseEntity
    {
        public CreatePSSRModel createModel { get; set; }
        public int PSSRID { get; set; }
        public int Coordinator { get; set; }
        public int ChairPerson { get; set; }
        public int OperationLead { get; set; }
        public int HSELead { get; set; }
        public int EnggLead { get; set; }
        public int PSSRLead { get; set; }
        public int DeptID { get; set; }
        public List<Employee> CoordinatorList { get; set; }
        public List<Employee> ChairPersonList { get; set; }
        public List<Employee> OperationHeadList { get; set; }
        public List<Employee> HSELeadList { get; set; }
        public List<Employee> ObserverList { get; set; }
        public List<Employee>  EnggLeadList { get; set; }
        public List<Employee> PSSRLeadList { get; set; }
        public List<Department> DepartmentList { get; set; }
        public string ChairPersonName { get; set; }
        public string HSELeadName { get; set; }
        public string EnggLeadName { get; set; }
        public string OperationLeadName { get; set; }
        public string PSSRLeadName { get; set; }
        public string ChairPersonComments { get; set; }
        public string HSELeadComments { get; set; }
        public string OperationHeadComments { get; set; }
        public string EnggLeadComments { get; set; }
        public string PSSRLeadComments { get; set; }
    }
}
