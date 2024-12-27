using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;
namespace MonitPro.Models
{

    public class BaseEntity
    {
        public int UserID { get; set; }
        public string UserFullName { get; set; }
        public List<Role> Roles { get; set; }
        public string ProfileImage { get; set; }
        public string CurrentSessionID { get; set; }
        public string PrevoiusSessionID { get; set; }
        public int DesigID { get; set; }
        public string IsRestrict { get; set; }
    }
    public class SessionDetails: BaseEntity
    {
        public string SessionActive { get; set; }
    }
   
    #region EquipmentAssignment

    public class AssignEquipments : BaseEntity
    {
        public List<User> Users { get; set; }
        public List<Equipment> Equipments { get; set; }
        public int SelectedUserID { get; set; }
        public int GetEquipmentList { get; set; }
        public int SaveUserEquipments { get; set; }
    }

    public class MeasureDataUsersEntity
    {
        public int AssignTo { get; set; }
        public List<int> EquipmentList { get; set; }
        public int AssignedBy { get; set; }
    }
     
    #endregion

    #region CreateUser

    public class UserRegister :BaseEntity
    {
           
        [Required(ErrorMessage = "First Name is Required", AllowEmptyStrings = false)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is Required", AllowEmptyStrings = false)]
        public string LastName { get; set; }

        [RegularExpression(@"^([0-9a-zA-Z]([\+\-_\.][0-9a-zA-Z]+)*)+@(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]*\.)+[a-zA-Z0-9]{2,3})$",ErrorMessage = "Invalid e-mail address")]
     
        public string EmailAddress { get; set; }
 
        [Required(ErrorMessage = "Username is Required", AllowEmptyStrings = false)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is Required", AllowEmptyStrings = false)]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "Password must be minimum 8 char length")]
        public string Password { get; set; }

        public string MobileNumber { get; set; }

        [Required(ErrorMessage = "Role is Required", AllowEmptyStrings = false)]
        public int SelectedRole { get; set; }

        public List<Role> RoleList { get; set; }
        [Required(ErrorMessage = "Department is Required", AllowEmptyStrings = false)]
        public int DepartID { get; set; }
        [Required(ErrorMessage = "Employee ID is Required", AllowEmptyStrings = false)]
        public string EmployeeID{ get; set;}
        public  bool RestrictAccess { get; set; }
        public List<Department> DepartmentList { get; set; }
        public List<Designation> DesignationList { get; set; }

        public string Designation { get; set; }
    }
 
    #endregion
    public class EquipmentList : BaseEntity
    {

        public List<EquipmentEntity> EquipmentEntity { get; set; }
        public List<Division> DivisionList = new List<Division>();

        public SearchEquipmentlist searchEquipment = new SearchEquipmentlist();
    }
    public class SearchEquipmentlist : BaseEntity
    {

        public int DivisionID { get; set; }
        public int EquipID { get; set; }

    }
    #region AddEquipment

    public class EquipmentEntity:BaseEntity
    {
        
        public int EquipmentID { get; set; }
        public int EqTypeID { get; set; }
        public string EqTypeName { get; set; }
        
        [Required(ErrorMessage="Factory is Required")]
        public int FactoryID { get; set; }

        [Required(ErrorMessage = "Division is Required")]
        public int DivisionID { get; set; }

        [Required(ErrorMessage = "Equipment Name is Required")]
        public string EquipmentName { get; set; }

         [Required(ErrorMessage = "TagID is Required")]
        public string EquipmentTagID { get; set; }

         public string EquipmentDescription { get; set; }

         [Required(ErrorMessage = "Template is Required")]
         public int TemplateID { get; set; }

         public IEnumerable<SelectListItem> FactoryList { get; set; }
         public IEnumerable<SelectListItem> DivisionList { get; set; }
         public IEnumerable<SelectListItem> TemplateList { get; set; }
        public List<EquipmentType> EquipemntTypeList { get; set; }
         public int LicenseCount { get; set; }
         public bool IsActive { get; set; }
         public string IsActiveSelect { get; set; }
         public string DivisionName { get; set; }
         public string FactoryName { get; set; }
         public bool IsEquipment { get; set; }
         public string IsEquipmentSelect { get; set; }
         public string FormatNumber { get; set;}
    }
    public class EquipmentType
    {
        public int EqTypeID { get; set; }
        public string EqTypeName { get; set; }
    }
    #endregion

    public class Profile : BaseEntity
    {
        public List<UserProfile> UserProfile { get; set; }
        public string Search { get; set; }
        public int Designation { get; set; }
        public int Department { get; set; }
        public int Role { get; set; }

        public List<Role> RoleList { get; set; }

        public List<Designation> DesignationList { get; set; }
        public List<Department> DepartmentList { get; set; }
    }

    public class ValidateFileAttribute : RequiredAttribute
    {
        public override bool IsValid(object value)
        {
            var file = value as HttpPostedFileBase;
            if (file == null)
            {
                return false;
            }

            if (file.ContentLength > 1)
            {
                return false;
            }

            try
            {
                using (var img = Image.FromStream(file.InputStream))
                {
                    return img.RawFormat.Equals(ImageFormat.Png);
                }
            }
            catch { }
            return false;
        }
    }


    public class UserProfile : BaseEntity
    {
        //public int UserID { get; set; }
        [Required(ErrorMessage = "First Name is Required", AllowEmptyStrings = false)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is Required", AllowEmptyStrings = false)]
        public string LastName { get; set; }

        [RegularExpression(@"^([0-9a-zA-Z]([\+\-_\.][0-9a-zA-Z]+)*)+@(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]*\.)+[a-zA-Z0-9]{2,3})$",ErrorMessage = "Invalid e-mail address")]

        public string EmailAddress { get; set; }
 
        [Required(ErrorMessage = "Username is Required", AllowEmptyStrings = false)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is Required", AllowEmptyStrings = false)]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "Password must be minimum 8 char length")]
        public string Password { get; set; }

        public string MobileNo { get; set; }

        [Required(ErrorMessage = "Role is Required", AllowEmptyStrings = false)]
        public int Role { get; set; }

        public int DepartID { get; set; }
        public string IsInvestigateSelect { get; set; }
        public string EmployeeID { get; set; }
        public string DeptName { get; set; }
        public bool IsActive { get; set; }
        public bool IsInvestigate { get; set; }

        public bool IsrestrictAccess { get; set; }
        public string IsRestrictedSelect { get; set; }
        public string IsActiveSelect { get; set; }
        
        public string UserImage { get; set; }

        public string Designation { get; set; }

        public List<Role> RoleList { get; set; }

        public List<Department> DepartmentList { get; set; }
        public List<Designation> DesignationList { get; set; }

        [ValidateFile(ErrorMessage = "Please select a PNG image smaller than 1MB")]
        public HttpPostedFileBase UserProfileImage { get; set; }

        public string RoleName { get; set; }

        public string DisplayUserName { get; set; }
       
    }
    public class Rolexml
    {
        public long BulkRoleID { get; set; }
    }

    public class LoginHistory : BaseEntity
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public List<LoginTimeList> LoginList { get; set; }

    }

    public class LoginTimeList
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string LoginTime { get; set; }
        public string LogOutTime { get; set; }
        public string EmployeeID { get; set; }
    }
}
