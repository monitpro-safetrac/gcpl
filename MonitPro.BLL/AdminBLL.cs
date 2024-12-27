using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonitPro.DAL;
using MonitPro.Models;
using System.Xml.Serialization;
using System.IO;

namespace MonitPro.BLL
{
    public class AdminBLL
    {

        #region InsertUserMaster

        public int InsertUserMaster(UserRegister userRegister)
        {
            //Encrypt Password
            for (int i = 0; i < 3; i++)
            {
                userRegister.Password = MonitPro.Security.Security.Instance.Encrypt(userRegister.Password);
            }

            AdminDA adminDA = new AdminDA();
            return adminDA.InsertUserMaster(userRegister);
        }

        #endregion


        #region AssignEquipments
        public AssignEquipments AssignEquipment(int userID)
        {
            AdminDA adminDA = new AdminDA();
            return adminDA.AssignEquipment(userID);
        }
        #endregion

        AdminDA adminDA = new AdminDA();


        public List<Role> GetRollList()
        {

            return adminDA.GetRoleList();

        }
        public List<Department> GetDepartmentList()
        {

            return adminDA.GetDepartmentList();

        }
        public List<Designation> GetDesignationList()
        {

            return adminDA.GetDesignationList();
        }

        public int UpdateUserProfile(UserProfile userProfile)
        {
            AccountDA accountDA = new AccountDA();
            for (int i = 0; i < 3; i++)
            {
                userProfile.Password = MonitPro.Security.Security.Instance.Encrypt(userProfile.Password);
            }
            int affectedRecordCount = accountDA.UpdateUserProfile(userProfile);

            for (int i = 0; i < 3; i++)
            {
                userProfile.Password = MonitPro.Security.Security.Instance.Decrypt(userProfile.Password);
            }
            return affectedRecordCount;
        }



        #region EditPlan

        public List<EquipmentEntity> GetEquipments()
        {
            AdminDA adminDA = new AdminDA();
            return adminDA.GetEquipments();
        }

        public int CheckTagID(string id)
        {
            AdminDA adminDA = new AdminDA();
            return adminDA.CheckTagID(id);
        }

        public List<MeasureData> getAllParameter(int id)
        {
            AdminDA adminDA = new AdminDA();
            return adminDA.getAllParameter(id);
        }

        public int DeleteParam(int id)
        {
            AdminDA adminDA = new AdminDA();
            return adminDA.DeleteParam(id);
        }

        public int EditParam(PlanData planData)
        {
            AdminDA adminDA = new AdminDA();
            return adminDA.EditParam(planData);
        }

        public int AddParameter(PlanData planData)
        {
            planData.LicenseCount = Security.Security.Instance.LicensedParameterCount();
            AdminDA adminDA = new AdminDA();
            return adminDA.AddParameter(planData);
        }

        #endregion


        public int MeasureDataUsersInsert(MeasureDataUsersEntity measureDataUsersEntity)
        {

            string equipmentList = string.Empty;
            XmlSerializer xmlSerializer = new XmlSerializer(measureDataUsersEntity.GetType());
            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, measureDataUsersEntity);
                equipmentList = textWriter.ToString();
            }

            return adminDA.MeasureDataUsersInsert(equipmentList);

        }



        public EquipmentEntity GetEquipmentInfo()
        {
            return adminDA.GetEquipmentInfo();
        }
        public List<EquipmentType> GetEquipmentType()
        {
            return adminDA.GetEquipmentType();
        }

        public int AddEquipment(EquipmentEntity equipementEntity)
        {

            return adminDA.AddEquipment(equipementEntity);
        }

        public List<Division> GetDivisions(int? factoryID)
        {

            return adminDA.GetDivisions(factoryID);
        }

        public int InsertWorkPlan(WorkPlannerViewModel workPlanner)
        {

            return adminDA.InsertWorkPlan(workPlanner);
        }

        public int UpdateApproved(ApprovalViewModel approveViewModel)
        {

            return adminDA.UpdateApproved(approveViewModel);
        }
        public List<UserProfile> SelectUserProfile()
        {
            return adminDA.SelectUserProfile();
        }
        public SessionDetails GetSession(int userid)
        {
            IncidentReportDAL IncidentDAL = new IncidentReportDAL();
            return IncidentDAL.GetSession(userid);
        }
        public List<UserProfile> GetUserName(Profile user)
        {

            return adminDA.GetUserName(user);
        }
        public UserProfile GetUserProfile(int UserID)
        {
            UserProfile userProfile = adminDA.GetUserProfile(UserID);
            if (userProfile != null)
            {
                for (int i = 0; i < 3; i++)
                    userProfile.Password = Security.Security.Instance.Decrypt(userProfile.Password);
            }

            return userProfile;
        }
        public List<EquipmentEntity> SelectEquipmentList()
        {
            return adminDA.SelectEquipmentList();
        }
        public List<EquipmentEntity> SearchEquipmentlist(SearchEquipmentlist searchEquipment)
        {
            return adminDA.SearchEquipmentlist(searchEquipment);
        }
        public EquipmentEntity GetEquipment(int id)
        {
            EquipmentEntity equipmentEntity = adminDA.GetEquipment(id);

            return equipmentEntity;
        }
        public List<EquipmentEntity> EquipmentDD()
        {
            return adminDA.EquipmentDD();
        }
        public List<EquipmentEntity> DivisionDD()
        {
            return adminDA.DivisionDD();
        }
        public int UpdateEquipment(EquipmentEntity Equipments)
        {

            int affectedRecordCount = adminDA.UpdateEquipment(Equipments);


            return affectedRecordCount;
        }
    }
}
