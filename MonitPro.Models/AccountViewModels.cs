using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;


namespace MonitPro.Models
{

     

    public class AddressBook
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }



    public class LoginViewModel
    {
     
        [DisplayName("User Name")]
        [Required(ErrorMessage = "Required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Required")]
        public string Password { get; set; }
        
        public string SessionActiveID { get; set; }
        public string OutIpAddress { get; set; }

        public string INIPAddress { get; set; }
        public List<IPADDressList> INIPList { get; set; }
    }
    public class IPADDressList
    {
        public int IPID { get; set; }
        public string INIPADdress { get; set; }
    }
    public class Designation
    {
        public int DesigID { get; set; }
        public string DesigName { get; set; }
    }
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class Role
    {
        public long RoleID { get; set; }
        public string RoleName { get; set; }
        public bool IsRole { get; set; }
    }
    public class Department
    {
        public long DeptID { get; set; }
        public string DeptName { get; set; }
    }

    public class UserEntityModel:BaseEntity
    {

        public int ID { get; set; }

        [DisplayName("User Name")]
        [Required(ErrorMessage = "*")]
        [StringLength(10, ErrorMessage = "User Name must not exceed 25 characters")]
        public string UserName { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }      
        [Required(ErrorMessage = "*")]
        public string Password { get; set; }
        [Required]
        public string Designation { get; set; }
        [Required]
        public string IsActive { get; set; }

        public dynamic dynamicObject { get; set; }
        public string IsRestrict { get; set; }
        public int Departmentid { get; set; }
    }
}
