using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data;
using MonitPro.Models;
using MonitPro.Common.Library;
using System.Web.Mvc;
using System.Xml.Serialization;
using System.IO;
namespace MonitPro.DAL
{

    public class AdminDA
    {

        #region InsertUserMaster

        public int InsertUserMaster(UserRegister userRegister)
        {
            int affectedRecordCount = 0;

            string rolevalue = string.Empty;
            List<Rolexml> rolexmls = new List<Rolexml>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "UserMasterInsert";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@FirstName", userRegister.FirstName);
                    objCom.Parameters.AddWithValue("@LastName", userRegister.LastName);
                    objCom.Parameters.AddWithValue("@EmailAddress", userRegister.EmailAddress);
                    objCom.Parameters.AddWithValue("@UserName", userRegister.UserName);
                    objCom.Parameters.AddWithValue("@Password", userRegister.Password);

                    if (!string.IsNullOrEmpty(userRegister.MobileNumber))
                        objCom.Parameters.AddWithValue("@MobileNumber", userRegister.MobileNumber);
                    else
                        objCom.Parameters.AddWithValue("@MobileNumber", DBNull.Value);

                    objCom.Parameters.AddWithValue("@Designation", userRegister.DesigID);
                    objCom.Parameters.AddWithValue("@Department", userRegister.DepartID);
                    objCom.Parameters.AddWithValue("@CreatedBy", userRegister.UserID);
                    objCom.Parameters.AddWithValue("@EmployeeID", userRegister.EmployeeID);
                    objCom.Parameters.AddWithValue("@IsRestrictedAccess", userRegister.RestrictAccess == true ? "Y" : "N");
                    SqlParameter outPutParameter = new SqlParameter();
                    outPutParameter.ParameterName = "@NewUserID";
                    outPutParameter.SqlDbType = System.Data.SqlDbType.Int;
                    outPutParameter.Direction = System.Data.ParameterDirection.Output;
                    objCom.Parameters.Add(outPutParameter);
                    objCom.Connection = objCon;
                    objCon.Open();
                    affectedRecordCount = objCom.ExecuteNonQuery();
                    int CurrentUser = int.Parse(outPutParameter.Value.ToString());
                    objCom.Parameters.Clear();
                    foreach (var item in userRegister.RoleList)
                    {
                        if (item.IsRole == true)
                        {
                            var xml = new Rolexml
                            {
                                BulkRoleID = item.RoleID

                            };
                            rolexmls.Add(xml);
                        }
                    }
                    XmlSerializer xmlSerializer = new XmlSerializer(rolexmls.GetType());

                    using (StringWriter textWriter = new StringWriter())
                    {
                        xmlSerializer.Serialize(textWriter, rolexmls);
                        rolevalue = textWriter.ToString();
                    }

                    objCom.CommandText = "BulkRoleInsert";
                    objCom.Parameters.AddWithValue("@UserID", CurrentUser);
                    objCom.Parameters.AddWithValue("@RoleID", rolevalue);
                    affectedRecordCount = objCom.ExecuteNonQuery();
                    objCon.Close();
                }
            }
            catch (Exception objException)
            {
                LogManager.Instance.Error(objException);
                throw new Exception(objException.Message);
            }

            return affectedRecordCount;
        }

        #endregion
        //#region AssignKPI

        //public AssignKPI AssignKPI(int userID)
        //{
        //    AssignKPI assignKPI = new AssignKPI();
        //    try
        //    {
        //        using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
        //        {
        //            SqlCommand objCom = new SqlCommand();
        //            objCom.CommandText = "AssignKPI";
        //            objCom.CommandType = CommandType.StoredProcedure;
        //            objCom.Parameters.AddWithValue("@UserID", userID);
        //            objCom.Connection = objCon;

        //            DataSet dsResult = new DataSet();
        //            SqlDataAdapter objAdapter = new SqlDataAdapter();
        //            objAdapter.SelectCommand = objCom;
        //            objAdapter.Fill(dsResult);


        //            assignKPI.Users = new List<User>();
        //            foreach (DataRow item in dsResult.Tables[0].Rows)
        //            {

        //                assignKPI.Users.Add(
        //                    new User
        //                    {
        //                        UserID = int.Parse(item["UserID"].ToString()),
        //                        FullName = item["UserFullName"].ToString()
        //                    }
        //                    );
        //            }

        //            assignKPI.Equipments = new List<Equipment>();
        //            foreach (DataRow item in dsResult.Tables[1].Rows)
        //            {

        //                assignKPI.Equipments.Add(
        //                    new Equipment
        //                    {
        //                        EquipmentID = int.Parse(item["PlanID"].ToString()),
        //                        EquipmentName = item["KPIName"].ToString(),
        //                        Assigned = int.Parse(item["Assigned"].ToString()) > 0 ? true : false,
        //                    }
        //                    );
        //            }
        //        }

        //    }
        //    catch (Exception objException)
        //    {
        //        LogManager.Instance.Error(objException);
        //        throw new Exception(objException.Message);
        //    }

        //    return assignKPI;
        //}
        //#endregion


        #region AssignEquipment

        public AssignEquipments AssignEquipment(int userID)
        {
            AssignEquipments assignEqipments = new AssignEquipments();
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "AssignEquipments";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@UserID", userID);
                    objCom.Connection = objCon;

                    DataSet dsResult = new DataSet();
                    SqlDataAdapter objAdapter = new SqlDataAdapter();
                    objAdapter.SelectCommand = objCom;
                    objAdapter.Fill(dsResult);

                    assignEqipments.Users = new List<User>();
                    foreach (DataRow item in dsResult.Tables[0].Rows)
                    {

                        assignEqipments.Users.Add(
                            new User
                            {
                                UserID = int.Parse(item["UserID"].ToString()),
                                FullName = item["UserFullName"].ToString()
                            }
                            );
                    }

                    assignEqipments.Equipments = new List<Equipment>();
                    foreach (DataRow item in dsResult.Tables[1].Rows)
                    {

                        assignEqipments.Equipments.Add(
                            new Equipment
                            {
                                EquipmentID = int.Parse(item["EquipmentID"].ToString()),
                                EquipmentName = item["EquipmentName"].ToString(),
                                Assigned = int.Parse(item["Assigned"].ToString()) > 0 ? true : false,
                            }
                            );
                    }
                }

            }
            catch (Exception objException)
            {
                LogManager.Instance.Error(objException);
                throw new Exception(objException.Message);
            }

            return assignEqipments;
        }
        #endregion
        public List<UserProfile> GetUserName(Profile user)
        {
            List<UserProfile> up = new List<UserProfile>();
            try
            {

                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[SearchUserName]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@UserName", user.Search == null ? string.Empty : user.Search);
                    objCom.Parameters.AddWithValue("@Dept", user.Department);
                    objCom.Parameters.AddWithValue("@Desig", user.Designation);
                    objCom.Parameters.AddWithValue("@Role", user.Role);
                    objCom.Connection = objCon;
                    objCon.Open();
                    SqlDataReader objReader = objCom.ExecuteReader();


                    while (objReader.Read())
                    {
                        up.Add(
                            new UserProfile
                            {
                                UserID = int.Parse(objReader["UserID"].ToString()),
                                FirstName = objReader["FirstName"].ToString(),
                                LastName = objReader["LastName"].ToString(),
                                EmailAddress = objReader["EmailAddress"].ToString(),
                                MobileNo = objReader["MobileNo"].ToString(),
                                UserName = objReader["UserName"].ToString(),
                                Designation = objReader["Designation"].ToString(),
                                IsActiveSelect = objReader["IsActive"].ToString(),
                                UserImage = objReader["UserImage"].ToString(),
                                DisplayUserName = objReader["DisplayUserName"].ToString(),
                                RoleName = objReader["RoleName"].ToString(),
                                EmployeeID = objReader["EmployeeID"].ToString(),
                                DeptName = objReader["Department"].ToString(),
                                IsInvestigateSelect = objReader["Investigate"].ToString(),
                                IsRestrictedSelect = objReader["IsResctrictedAccess"].ToString(),

                            });
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return up;
        }

        #region SelectUserProfile
        public List<UserProfile> SelectUserProfile()
        {
            List<UserProfile> listUserProfile = null;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[UserMasterSelect]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCon.Open();
                    objCom.Connection = objCon;
                    SqlDataReader objReader = objCom.ExecuteReader();
                    listUserProfile = new List<UserProfile>();
                    while (objReader.Read())
                    {
                        UserProfile Profile = new UserProfile();

                        Profile.UserID = int.Parse(objReader["UserID"].ToString());
                        Profile.FirstName = objReader["FirstName"].ToString();
                        Profile.LastName = objReader["LastName"].ToString();
                        Profile.EmailAddress = objReader["EmailAddress"].ToString();
                        Profile.MobileNo = objReader["MobileNo"].ToString();
                        Profile.UserName = objReader["UserName"].ToString();
                        Profile.Designation = objReader["Designation"].ToString();
                        Profile.IsActiveSelect = objReader["IsActive"].ToString();
                        Profile.UserImage = objReader["UserImage"].ToString();
                        Profile.DisplayUserName = objReader["DisplayUserName"].ToString();
                        Profile.RoleName = objReader["RoleName"].ToString();
                        Profile.EmployeeID = objReader["EmployeeID"].ToString();
                        Profile.DeptName = objReader["Department"].ToString();
                        Profile.IsInvestigateSelect = objReader["Investigate"].ToString();
                        if (objReader["DepartmentID"] != DBNull.Value)
                        {
                            Profile.DepartID = int.Parse(objReader["DepartmentID"].ToString());
                        }
                        Profile.IsRestrictedSelect = objReader["IsResctrictedAccess"].ToString();

                        listUserProfile.Add(Profile);
                    }
                }
                LogManager.Instance.Info("MonitPro.DAL.AccountDA.SelectUserProfile Method - End");
            }
            catch (Exception objException)
            {
                LogManager.Instance.Error(objException);
                throw new Exception(objException.Message);
            }
            return listUserProfile;
        }
        #endregion

        #region GetUserProfile
        public UserProfile GetUserProfile(int UserID)
        {
            UserProfile userProfile = null;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[UserMasterGet]";
                    objCom.Parameters.AddWithValue("@UserID", UserID);
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCon.Open();
                    objCom.Connection = objCon;
                    SqlDataReader objReader = objCom.ExecuteReader();
                    userProfile = new UserProfile();
                    if (objReader.Read())
                    {
                        userProfile.UserID = int.Parse(objReader["UserID"].ToString());
                        userProfile.FirstName = objReader["FirstName"].ToString();
                        userProfile.LastName = objReader["LastName"].ToString();
                        userProfile.EmailAddress = objReader["EmailAddress"].ToString();
                        userProfile.UserName = objReader["UserName"].ToString();
                        userProfile.Password = objReader["Password"].ToString();
                        userProfile.MobileNo = objReader["MobileNo"].ToString();
                        if (objReader["Designation"] != DBNull.Value)
                        {
                            userProfile.DesigID = int.Parse(objReader["Designation"].ToString());
                        }
                        if (objReader["DepartmentID"] != DBNull.Value)
                        {
                            userProfile.DepartID = int.Parse(objReader["DepartmentID"].ToString());
                        }
                        userProfile.UserImage = objReader["UserImage"].ToString();
                        userProfile.EmployeeID = objReader["EmployeeID"].ToString();
                        userProfile.IsActive = objReader["IsActive"].ToString() == "Y" ? true : false;
                        userProfile.IsInvestigate = objReader["InvestigationFacilitator"].ToString() == "Y" ? true : false;
                        userProfile.IsrestrictAccess = objReader["IsResctrictedAccess"].ToString() == "Y" ? true : false;
                    }

                    objReader.NextResult();

                    List<Role> roleList = new List<Role>();
                    while (objReader.Read())
                    {

                        roleList.Add(new Role
                        {
                            RoleID = long.Parse(objReader["RoleID"].ToString()),
                            RoleName = objReader["RoleName"].ToString(),
                            IsRole = int.Parse(objReader["ChkBox"].ToString()) > 0 ? true : false,
                        });

                    }
                    userProfile.RoleList = roleList;
                    objReader.NextResult();

                    List<Department> departList = new List<Department>();
                    while (objReader.Read())
                    {

                        departList.Add(new Department
                        {
                            DeptID = long.Parse(objReader["DeptID"].ToString()),
                            DeptName = objReader["DeptName"].ToString()
                        });

                    }
                    userProfile.DepartmentList = departList;
                }
            }
            catch (Exception objException)
            {
                LogManager.Instance.Error(objException);
                throw new Exception(objException.Message);
            }
            return userProfile;
        }
        #endregion
        #region MeasureDataUsersInsert
        public int MeasureDataUsersInsert(string equipmentList)
        {
            int affectedRecordCount = 0;

            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "MeasureDataUsersInsert";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@EquipmentList", equipmentList);
                    objCom.Connection = objCon;
                    objCon.Open();
                    affectedRecordCount = objCom.ExecuteNonQuery();
                    objCon.Close();
                }
            }
            catch (Exception objException)
            {
                LogManager.Instance.Error(objException);
                throw new Exception(objException.Message);
            }

            return affectedRecordCount;
        }

        #endregion

        #region KPIInsert
        public int KPIInsert(string equipmentList)
        {
            int affectedRecordCount = 0;

            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "KPIInsert";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@EquipmentList", equipmentList);
                    objCom.Connection = objCon;
                    objCon.Open();
                    affectedRecordCount = objCom.ExecuteNonQuery();
                    objCon.Close();
                }
            }
            catch (Exception objException)
            {
                LogManager.Instance.Error(objException);
                throw new Exception(objException.Message);
            }

            return affectedRecordCount;
        }

        #endregion

        #region EditPlan

        #region GetEquipments
        public List<EquipmentEntity> GetEquipments()
        {
            List<EquipmentEntity> equipmentList = null;

            try
            {
                equipmentList = new List<EquipmentEntity>();

                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "EquipmentSelect";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCon.Open();
                    objCom.Connection = objCon;
                    SqlDataReader objReader = objCom.ExecuteReader();

                    while (objReader.Read())
                    {
                        var equipmentEntity = new EquipmentEntity();
                        equipmentEntity.EquipmentID = int.Parse(objReader["EquipmentID"].ToString());
                        equipmentEntity.EquipmentName = objReader["EquipmentName"].ToString();
                        equipmentList.Add(equipmentEntity);
                    }
                    objCon.Close();
                }
            }
            catch (Exception objException)
            {
                LogManager.Instance.Error(objException);
                throw new Exception(objException.Message);
            }
            return equipmentList;
        }
        #endregion

        #region GetRoleList
        public List<Role> GetRoleList()
        {
            List<Role> roleList = null;
            try
            {
                roleList = new List<Role>();

                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "RoleMasterSelect";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCon.Open();
                    objCom.Connection = objCon;
                    SqlDataReader objReader = objCom.ExecuteReader();

                    while (objReader.Read())
                    {
                        var role = new Role();
                        role.RoleID = int.Parse(objReader["RoleID"].ToString());
                        role.RoleName = objReader["RoleName"].ToString();
                        roleList.Add(role);
                    }
                    objCon.Close();
                }


            }
            catch (Exception objException)
            {
                LogManager.Instance.Error(objException);
                throw new Exception(objException.Message);
            }
            return roleList;

        }
        #endregion

        #region GetDepartmentList
        public List<Department> GetDepartmentList()
        {
            List<Department> departList = null;
            try
            {
                departList = new List<Department>();

                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "DepartmentSelect";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCon.Open();
                    objCom.Connection = objCon;
                    SqlDataReader objReader = objCom.ExecuteReader();

                    while (objReader.Read())
                    {
                        var dept = new Department();
                        dept.DeptID = int.Parse(objReader["DeptID"].ToString());
                        dept.DeptName = objReader["DeptName"].ToString();
                        departList.Add(dept);
                    }
                    objCon.Close();
                }


            }
            catch (Exception objException)
            {
                LogManager.Instance.Error(objException);
                throw new Exception(objException.Message);
            }
            return departList;

        }
        #endregion

        #region Get DesignationList
        public List<Designation> GetDesignationList()
        {
            List<Designation> DesigList = new List<Designation>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "DesignationDD";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCon.Open();
                    objCom.Connection = objCon;
                    SqlDataReader objReader = objCom.ExecuteReader();

                    while (objReader.Read())
                    {
                        Designation des = new Designation();
                        des.DesigID = int.Parse(objReader["DesID"].ToString());
                        des.DesigName = objReader["DesignationName"].ToString();
                        DesigList.Add(des);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);
            }
            return DesigList;
        }
        #endregion
        #region CheckTagID
        public int CheckTagID(string id)
        {
            int result;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "CheckTagID";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("tagid", id);

                    objCon.Open();
                    objCom.Connection = objCon;
                    result = Convert.ToInt32(objCom.ExecuteScalar());
                    objCon.Close();

                }
            }
            catch (Exception objException)
            {
                LogManager.Instance.Error(objException);
                throw new Exception(objException.Message);
            }
            return result;
        }
        #endregion

        #region getAllParameter
        public List<MeasureData> getAllParameter(int id)
        {
            List<MeasureData> parameterList = null;
            try
            {
                parameterList = new List<MeasureData>();
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "EditPlanSelect";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("equipmentid", id);

                    objCon.Open();
                    objCom.Connection = objCon;

                    SqlDataReader objReader = objCom.ExecuteReader();

                    List<Factory> factoryList = new List<Factory>();

                    while (objReader.Read())
                    {
                        Factory factory = new Factory();
                        factory.FactoryID = objReader["FrequencyID"].ToString();
                        factory.FactoryName = objReader["Frequency"].ToString();

                        factoryList.Add(factory);
                    }

                    var frequency = factoryList
                       .Select(x =>
                               new SelectListItem
                               {
                                   Value = x.FactoryID,
                                   Text = x.FactoryName
                               });


                    objReader.NextResult();

                    var userList = new List<User>();
                    while (objReader.Read())
                    {
                        var user = new User();
                        user.UserID = int.Parse(objReader["UserID"].ToString());
                        user.FullName = objReader["UserFullName"].ToString();
                        userList.Add(user);
                    }

                    objReader.NextResult();

                    while (objReader.Read())
                    {
                        MeasureData measureData = new MeasureData();
                        measureData.PlanID = objReader["PlanID"].ToString();
                        measureData.TemplateID = objReader["TemplateID"].ToString();
                        measureData.Parameter = objReader["ParameterName"].ToString();
                        measureData.ParameterNo = int.Parse(objReader["ParameterNo"].ToString());
                        measureData.UOM = objReader["UnitOfMeasure"].ToString();
                        measureData.LTV = objReader["LowTargetValue"].ToString();
                        measureData.HTV = objReader["HighTargetValue"].ToString();
                        measureData.LHTPercentage = objReader["LHTargetPercentage"].ToString();
                        measureData.EquipmentID = objReader["EquipmentID"].ToString();
                        measureData.ParameterDesc = objReader["ParameterDesc"].ToString();
                        measureData.ReasonForLow = objReader["ReasonForLow"].ToString();
                        measureData.ConsequenceForLow = objReader["ConsequenceForLow"].ToString();
                        measureData.ActionForLow = objReader["ActionForLow"].ToString();
                        measureData.Notes = objReader["Notes"].ToString();
                        measureData.CreatedBy = int.Parse(objReader["CreatedBy"].ToString());
                        measureData.ReasonForHigh = objReader["ReasonForHigh"].ToString();
                        measureData.ConsequenceForHigh = objReader["ConsequenceForHigh"].ToString();
                        measureData.ActionForHigh = objReader["ActionForHigh"].ToString();
                        measureData.Priority = Convert.ToChar(objReader["Priority"]);
                        measureData.TagID = objReader["HistorianTagID"].ToString();
                        measureData.FrequencyID = objReader["FrequencyID"].ToString();
                        measureData.Responsibility = objReader["Responsibility"].ToString();
                        measureData.ReasonForMonitoring = objReader["ReasonForMonitoring"].ToString();
                        measureData.CreatedOn = objReader["CreatedOn"].ToString();
                        measureData.IsActive = objReader["IsActive"].ToString();
                        measureData.IsNumeric = objReader["IsNumeric"].ToString();

                        if (objReader["ParameterType"] != DBNull.Value)
                        {
                            measureData.ParameterType = Convert.ToChar(objReader["ParameterType"]);
                        }

                        if (objReader["Formula"] != DBNull.Value)
                        {
                            measureData.Formula = objReader["Formula"].ToString();
                        }

                        if (objReader["KPI"] != DBNull.Value)
                        {
                            measureData.IsKPI = objReader["KPI"].ToString();
                        }
                        measureData.Frequencies = frequency;
                        measureData.UserList = userList;
                        parameterList.Add(measureData);
                    }
                    objCon.Close();
                }
            }
            catch (Exception objException)
            {
                LogManager.Instance.Error(objException);
                throw new Exception(objException.Message);
            }
            return parameterList;
        }
        #endregion

        #region DeleteParam
        public int DeleteParam(int id)
        {
            int recordCount = 0;

            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "ParameterDelete";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("PlanID", id);
                    objCon.Open();
                    objCom.Connection = objCon;
                    recordCount = objCom.ExecuteNonQuery();
                    objCon.Close();
                }
            }
            catch (Exception objException)
            {
                LogManager.Instance.Error(objException);
                throw new Exception(objException.Message);
            }
            return recordCount;
        }
        #endregion

        #region EditParam
        public int EditParam(PlanData planData)
        {
            int recordCount = 0;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "PlanUpdate";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@PlanID", planData.PlanID);
                    objCom.Parameters.AddWithValue("@EquipmentID", planData.EquipmentID);
                    objCom.Parameters.AddWithValue("@ParameterName", planData.Parameter);
                    objCom.Parameters.AddWithValue("@ParameterNo", planData.ParameterNo);
                    objCom.Parameters.AddWithValue("@ParameterDesc", planData.ParameterDesc);
                    objCom.Parameters.AddWithValue("@HistorianTagID", planData.TagID);
                    if (planData.UOM == null)
                        objCom.Parameters.AddWithValue("@UnitOfMeasure", DBNull.Value);
                    else
                        objCom.Parameters.AddWithValue("@UnitOfMeasure", planData.UOM);

                    if (planData.ReasonForMonitoring == null)
                        objCom.Parameters.AddWithValue("@ReasonForMonitoring", DBNull.Value);
                    else
                        objCom.Parameters.AddWithValue("@ReasonForMonitoring", planData.ReasonForMonitoring);

                    objCom.Parameters.AddWithValue("@Priority", planData.Priority);
                    objCom.Parameters.AddWithValue("@FrequencyID", planData.FrequencyID);

                    if (planData.Notes == null)
                        objCom.Parameters.AddWithValue("@Notes", DBNull.Value);
                    else
                        objCom.Parameters.AddWithValue("@Notes", planData.Notes);

                    if (planData.LTV == null)
                        objCom.Parameters.AddWithValue("@LowTargetValue", DBNull.Value);
                    else
                        objCom.Parameters.AddWithValue("@LowTargetValue", planData.LTV);

                    if (planData.HTV == null)
                        objCom.Parameters.AddWithValue("@HighTargetValue", DBNull.Value);
                    else
                        objCom.Parameters.AddWithValue("@HighTargetValue", planData.HTV);

                    if (planData.LHTPercentage == null)
                        objCom.Parameters.AddWithValue("@LHTargetPercentage", DBNull.Value);
                    else
                        objCom.Parameters.AddWithValue("@LHTargetPercentage", planData.LHTPercentage);

                    if (planData.ReasonForLow == null)
                        objCom.Parameters.AddWithValue("@ReasonForLow", DBNull.Value);
                    else
                        objCom.Parameters.AddWithValue("@ReasonForLow", planData.ReasonForLow);

                    if (planData.ConsequenceForLow == null)
                        objCom.Parameters.AddWithValue("@ConsequenceForLow", DBNull.Value);
                    else
                        objCom.Parameters.AddWithValue("@ConsequenceForLow", planData.ConsequenceForLow);

                    if (planData.ActionForLow == null)
                        objCom.Parameters.AddWithValue("@ActionForLow", DBNull.Value);
                    else
                        objCom.Parameters.AddWithValue("@ActionForLow", planData.ActionForLow);

                    if (planData.ReasonForHigh == null)
                        objCom.Parameters.AddWithValue("@ReasonForHigh", DBNull.Value);
                    else
                        objCom.Parameters.AddWithValue("@ReasonForHigh", planData.ReasonForHigh);

                    if (planData.ConsequenceForHigh == null)
                        objCom.Parameters.AddWithValue("@ConsequenceForHigh", DBNull.Value);
                    else
                        objCom.Parameters.AddWithValue("@ConsequenceForHigh", planData.ConsequenceForHigh);

                    if (planData.ActionForHigh == null)
                        objCom.Parameters.AddWithValue("@ActionForHigh", DBNull.Value);
                    else
                        objCom.Parameters.AddWithValue("@ActionForHigh", planData.ActionForHigh);


                    objCom.Parameters.AddWithValue("@Responsibility", planData.Responsibility);
                    objCom.Parameters.AddWithValue("@IsActive", planData.IsActive);
                    objCom.Parameters.AddWithValue("@IsNumeric", planData.IsNumeric);


                    if (string.IsNullOrEmpty(planData.ParameterType))
                        objCom.Parameters.AddWithValue("@ParameterType", DBNull.Value);
                    else
                        objCom.Parameters.AddWithValue("@ParameterType", planData.ParameterType);

                    if (planData.Formula == null)
                        objCom.Parameters.AddWithValue("@Formula", DBNull.Value);
                    else
                        objCom.Parameters.AddWithValue("@Formula", planData.Formula);


                    if (string.IsNullOrEmpty(planData.IsKPI))
                    {
                        objCom.Parameters.AddWithValue("@IsKPI", DBNull.Value);
                    }
                    else
                    {
                        objCom.Parameters.AddWithValue("@IsKPI", planData.IsKPI);
                    }


                    objCom.Parameters.AddWithValue("@UpdatedBy", planData.UpdatedBy);



                    objCon.Open();
                    objCom.Connection = objCon;
                    recordCount = objCom.ExecuteNonQuery();
                    objCon.Close();
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }
            return recordCount;
        }
        #endregion

        #region AddParameter
        public int AddParameter(PlanData planData)
        {
            int recordCount = 0;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "PlanInsert";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@EquipmentID", planData.EquipmentID);
                    objCom.Parameters.AddWithValue("@ParameterName", planData.Parameter);
                    objCom.Parameters.AddWithValue("@ParameterNo", planData.ParameterNo);
                    objCom.Parameters.AddWithValue("@ParameterDesc", planData.ParameterDesc);
                    objCom.Parameters.AddWithValue("@HistorianTagID", planData.TagID);
                    if (planData.UOM == null)
                        objCom.Parameters.AddWithValue("@UnitOfMeasure", DBNull.Value);
                    else
                        objCom.Parameters.AddWithValue("@UnitOfMeasure", planData.UOM);

                    if (planData.ReasonForMonitoring == null)
                        objCom.Parameters.AddWithValue("@ReasonForMonitoring", DBNull.Value);
                    else
                        objCom.Parameters.AddWithValue("@ReasonForMonitoring", planData.ReasonForMonitoring);

                    objCom.Parameters.AddWithValue("@Priority", planData.Priority);
                    objCom.Parameters.AddWithValue("@FrequencyID", planData.FrequencyID);

                    if (planData.Notes == null)
                        objCom.Parameters.AddWithValue("@Notes", DBNull.Value);
                    else
                        objCom.Parameters.AddWithValue("@Notes", planData.Notes);

                    if (planData.LTV == null)
                        objCom.Parameters.AddWithValue("@LowTargetValue", DBNull.Value);
                    else
                        objCom.Parameters.AddWithValue("@LowTargetValue", planData.LTV);

                    if (planData.HTV == null)
                        objCom.Parameters.AddWithValue("@HighTargetValue", DBNull.Value);
                    else
                        objCom.Parameters.AddWithValue("@HighTargetValue", planData.HTV);

                    if (planData.LHTPercentage == null)
                        objCom.Parameters.AddWithValue("@LHTargetPercentage", DBNull.Value);
                    else
                        objCom.Parameters.AddWithValue("@LHTargetPercentage", planData.LHTPercentage);

                    if (planData.ReasonForLow == null)
                        objCom.Parameters.AddWithValue("@ReasonForLow", DBNull.Value);
                    else
                        objCom.Parameters.AddWithValue("@ReasonForLow", planData.ReasonForLow);

                    if (planData.ConsequenceForLow == null)
                        objCom.Parameters.AddWithValue("@ConsequenceForLow", DBNull.Value);
                    else
                        objCom.Parameters.AddWithValue("@ConsequenceForLow", planData.ConsequenceForLow);

                    if (planData.ActionForLow == null)
                        objCom.Parameters.AddWithValue("@ActionForLow", DBNull.Value);
                    else
                        objCom.Parameters.AddWithValue("@ActionForLow", planData.ActionForLow);

                    if (planData.ReasonForHigh == null)
                        objCom.Parameters.AddWithValue("@ReasonForHigh", DBNull.Value);
                    else
                        objCom.Parameters.AddWithValue("@ReasonForHigh", planData.ReasonForHigh);

                    if (planData.ConsequenceForHigh == null)
                        objCom.Parameters.AddWithValue("@ConsequenceForHigh", DBNull.Value);
                    else
                        objCom.Parameters.AddWithValue("@ConsequenceForHigh", planData.ConsequenceForHigh);

                    if (planData.ActionForHigh == null)
                        objCom.Parameters.AddWithValue("@ActionForHigh", DBNull.Value);
                    else
                        objCom.Parameters.AddWithValue("@ActionForHigh", planData.ActionForHigh);

                    objCom.Parameters.AddWithValue("@Responsibility", planData.Responsibility);
                    objCom.Parameters.AddWithValue("@IsActive", planData.IsActive);
                    objCom.Parameters.AddWithValue("@IsNumeric", planData.IsNumeric);


                    if (planData.ParameterType == null)
                        objCom.Parameters.AddWithValue("@ParameterType", DBNull.Value);
                    else
                        objCom.Parameters.AddWithValue("@ParameterType", planData.ParameterType);

                    if (planData.Formula == null)
                        objCom.Parameters.AddWithValue("@Formula", DBNull.Value);
                    else
                        objCom.Parameters.AddWithValue("@Formula", planData.Formula);

                    if (planData.IsKPI == null)
                        objCom.Parameters.AddWithValue("@KPI", DBNull.Value);
                    else
                        objCom.Parameters.AddWithValue("@KPI", planData.IsKPI);

                    objCom.Parameters.AddWithValue("@UpdatedBy", planData.UpdatedBy);

                    objCom.Parameters.AddWithValue("@LicenseCount", planData.LicenseCount);

                    objCon.Open();
                    objCom.Connection = objCon;
                    recordCount = objCom.ExecuteNonQuery();
                    objCon.Close();
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }
            return recordCount;
        }
        #endregion

        #endregion


        #region GetDivisions
        public List<Division> GetDivisions(int? factoryID)
        {
            List<Division> divisionList = new List<Division>();
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "DivisionSelect";

                    if (factoryID > 0)
                        objCom.Parameters.AddWithValue("@FactoryID", factoryID);
                    else
                        objCom.Parameters.AddWithValue("@FactoryID", DBNull.Value);

                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();

                    SqlDataReader Results = objCom.ExecuteReader();

                    while (Results.Read())
                    {
                        Division division = new Division();
                        division.DivisionID = Results[0].ToString();
                        division.DivisionName = Results[1].ToString();
                        divisionList.Add(division);
                    }
                    objCon.Close();
                    Results.Close();
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return divisionList;

        }
        #endregion

        #region GetEquipmentInfo
        public EquipmentEntity GetEquipmentInfo()
        {
            EquipmentEntity equipmentEntity = new EquipmentEntity();
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[EquipmentInfo]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;

                    DataSet dsResult = new DataSet();
                    SqlDataAdapter objAdapter = new SqlDataAdapter();
                    objAdapter.SelectCommand = objCom;
                    objAdapter.Fill(dsResult);

                    var factoryList = new List<SelectListItem>();
                    foreach (DataRow drDivision in dsResult.Tables[0].Rows)
                    {
                        SelectListItem factory = new SelectListItem();
                        factory.Value = drDivision["FactoryID"].ToString();
                        factory.Text = drDivision["FactoryName"].ToString();
                        factoryList.Add(factory);
                    }
                    equipmentEntity.FactoryList = factoryList.Where(x => x.Value == "1").ToList();

                    equipmentEntity.DivisionList = new List<SelectListItem>();


                }

            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }

            return equipmentEntity;


        }
        #endregion

        #region EquipmentType DD
        public List<EquipmentType> GetEquipmentType()
        {
            List<EquipmentType> eqtypelist = new List<EquipmentType>();
            try
            {

                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[GetEquipmentType]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Connection = objCon;
                    objCon.Open();
                    var reader = objCom.ExecuteReader();
                    while (reader.Read())
                    {
                        EquipmentType eq = new EquipmentType();
                        eq.EqTypeID = int.Parse(reader["EquipmentTypeID"].ToString());
                        eq.EqTypeName = reader["EquipmentTypeName"].ToString();
                        eqtypelist.Add(eq);
                    }
                    objCon.Close();


                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.Error(ex);
                throw new Exception(ex.Message);

            }
            return eqtypelist;
        }
        #endregion
        #region AddEquipment
        public int AddEquipment(EquipmentEntity equipmentEntity)
        {
            int affectedRecordCount = 0;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "EquipmentInsert";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@FactoryID", equipmentEntity.FactoryID);
                    objCom.Parameters.AddWithValue("@DivisionID", equipmentEntity.DivisionID);
                    objCom.Parameters.AddWithValue("@EquipmentTagID", equipmentEntity.EquipmentTagID);
                    objCom.Parameters.AddWithValue("@EquipmentName", equipmentEntity.EquipmentName);
                    objCom.Parameters.AddWithValue("@IsEquipment", equipmentEntity.IsEquipment);
                    if (!string.IsNullOrEmpty(equipmentEntity.EquipmentDescription))
                    {
                        objCom.Parameters.AddWithValue("@EquipmentDescription", equipmentEntity.EquipmentDescription);
                    }
                    else
                    {
                        objCom.Parameters.AddWithValue("@EquipmentDescription", DBNull.Value);
                    }
                    objCom.Parameters.AddWithValue("@EquipmentType", equipmentEntity.EqTypeID);
                    objCom.Parameters.AddWithValue("@CreatedBy", equipmentEntity.UserID);
                    objCom.Parameters.AddWithValue("@LicenseCount", equipmentEntity.LicenseCount);

                    SqlParameter outPutParameter = new SqlParameter();
                    outPutParameter.ParameterName = "@EquipmentID";
                    outPutParameter.SqlDbType = System.Data.SqlDbType.Int;
                    outPutParameter.Direction = System.Data.ParameterDirection.Output;
                    objCom.Parameters.Add(outPutParameter);

                    objCom.Connection = objCon;
                    objCon.Open();
                    affectedRecordCount = objCom.ExecuteNonQuery();
                    equipmentEntity.EquipmentID = int.Parse(outPutParameter.Value.ToString());
                    objCon.Close();
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }
            return affectedRecordCount;
        }
        #endregion

        #region InsertWorkPlan
        public int InsertWorkPlan(WorkPlannerViewModel workPlanner)
        {
            int affectedRecordCount = 0;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "WorkPlanInsert";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@PlanDataID", workPlanner.PlanDataID);
                    objCom.Parameters.AddWithValue("@PlanID", workPlanner.PlanID);

                    if (workPlanner.Status == 'R' || workPlanner.Status == 'G' || workPlanner.Status == 'Y')
                    {
                        objCom.Parameters.AddWithValue("@Status", workPlanner.Status);
                    }
                    else
                    {
                        objCom.Parameters.AddWithValue("@Status", DBNull.Value);
                    }

                    if (!string.IsNullOrEmpty(workPlanner.ActionNotes))
                    {
                        objCom.Parameters.AddWithValue("@ActionNotes", workPlanner.ActionNotes);
                    }
                    else
                    {
                        objCom.Parameters.AddWithValue("@ActionNotes", DBNull.Value);
                    }


                    if (workPlanner.AssignTo > 0)
                    {
                        objCom.Parameters.AddWithValue("@AssignedTo", workPlanner.AssignTo);
                    }
                    else
                    {
                        objCom.Parameters.AddWithValue("@AssignedTo", DBNull.Value);
                    }

                    if (!string.IsNullOrEmpty(workPlanner.AnalysisDocument))
                    {
                        objCom.Parameters.AddWithValue("@AnalysisDocument", workPlanner.AnalysisDocument);
                    }
                    else
                    {
                        objCom.Parameters.AddWithValue("@AnalysisDocument", DBNull.Value);
                    }

                    objCom.Parameters.AddWithValue("@CompletionStatus", workPlanner.CompletionStatus);

                    objCom.Parameters.AddWithValue("@ActionTakenBy", workPlanner.UserID);
                    objCom.Connection = objCon;

                    objCon.Open();
                    affectedRecordCount = objCom.ExecuteNonQuery();
                    objCon.Close();
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }


            return affectedRecordCount;
        }
        #endregion
        #region EditEquipment
        public List<EquipmentEntity> SelectEquipmentList()
        {
            List<EquipmentEntity> equipmentList = null;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[EquipmentSelectAll]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCon.Open();
                    objCom.Connection = objCon;
                    SqlDataReader objReader = objCom.ExecuteReader();
                    equipmentList = new List<EquipmentEntity>();

                    while (objReader.Read())
                        equipmentList.Add(new EquipmentEntity
                        {
                            EquipmentID = int.Parse(objReader["EquipmentID"].ToString()),
                            FactoryName = objReader["FactoryName"].ToString(),
                            DivisionName = objReader["DivisionName"].ToString(),
                            EquipmentTagID = objReader["EquipmentTagID"].ToString(),
                            EquipmentName = objReader["EquipmentName"].ToString(),
                            EquipmentDescription = objReader["EquipmentDescription"].ToString(),
                            IsEquipmentSelect = objReader["IsEquipment"].ToString(),
                            IsActiveSelect = objReader["IsActive"].ToString(),
                            EqTypeName = objReader["EquipmentTypeName"].ToString()


                        });
                }
            }
            catch (Exception objException)
            {
                LogManager.Instance.Error(objException);
                throw objException;
            }
            return equipmentList;
        }
        public List<EquipmentEntity> SearchEquipmentlist(SearchEquipmentlist searchEquipment)
        {
            List<EquipmentEntity> equipmentList = null;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[SearchEquipmentList]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCon.Open();
                    objCom.Connection = objCon;
                    objCom.Parameters.AddWithValue("@Division", searchEquipment.DivisionID);
                    objCom.Parameters.AddWithValue("@TypeOfEquipment", searchEquipment.EquipID);
                    SqlDataReader objReader = objCom.ExecuteReader();
                    equipmentList = new List<EquipmentEntity>();

                    while (objReader.Read())
                        equipmentList.Add(new EquipmentEntity
                        {
                            EquipmentID = int.Parse(objReader["EquipmentID"].ToString()),
                            FactoryName = objReader["FactoryName"].ToString(),
                            DivisionName = objReader["DivisionName"].ToString(),
                            EquipmentTagID = objReader["EquipmentTagID"].ToString(),
                            EquipmentName = objReader["EquipmentName"].ToString(),
                            EquipmentDescription = objReader["EquipmentDescription"].ToString(),
                            IsEquipmentSelect = objReader["IsEquipment"].ToString(),
                            IsActiveSelect = objReader["IsActive"].ToString(),
                            EqTypeName = objReader["EquipmentTypeName"].ToString()

                        });
                }
            }
            catch (Exception objException)
            {
                LogManager.Instance.Error(objException);
                throw objException;
            }
            return equipmentList;
        }

        public List<EquipmentEntity> EquipmentDD()
        {
            List<EquipmentEntity> equipmentList = null;

            try
            {
                equipmentList = new List<EquipmentEntity>();

                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[EquipmentDD]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCon.Open();
                    objCom.Connection = objCon;
                    SqlDataReader objReader = objCom.ExecuteReader();

                    while (objReader.Read())
                    {
                        var equipmentEntity = new EquipmentEntity();
                        equipmentEntity.EquipmentID = int.Parse(objReader["EquipmentID"].ToString());
                        equipmentEntity.EquipmentName = objReader["EquipmentName"].ToString();
                        equipmentList.Add(equipmentEntity);
                    }
                    objCon.Close();
                }
            }
            catch (Exception objException)
            {
                LogManager.Instance.Error(objException);
                throw new Exception(objException.Message);
            }
            return equipmentList;
        }
        public List<EquipmentEntity> DivisionDD()
        {
            List<EquipmentEntity> equipmentList = null;

            try
            {
                equipmentList = new List<EquipmentEntity>();

                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[DivisionDD]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCon.Open();
                    objCom.Connection = objCon;
                    SqlDataReader objReader = objCom.ExecuteReader();

                    while (objReader.Read())
                    {
                        var equipmentEntity = new EquipmentEntity();
                        equipmentEntity.DivisionID = int.Parse(objReader["DivisionID"].ToString());
                        equipmentEntity.DivisionName = objReader["DivisionName"].ToString();
                        equipmentList.Add(equipmentEntity);
                    }
                    objCon.Close();
                }
            }
            catch (Exception objException)
            {
                LogManager.Instance.Error(objException);
                throw new Exception(objException.Message);
            }
            return equipmentList;
        }
        public EquipmentEntity GetEquipment(int id)
        {
            LogManager.Instance.Info("MonitPro.DAL.AccountDA.ValidateUser Method - Start");
            EquipmentEntity Equipments = new EquipmentEntity();
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[EquipmentGet]";
                    objCom.Parameters.AddWithValue("@EquipmentID", id);
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCon.Open();
                    objCom.Connection = objCon;
                    SqlDataReader objReader = objCom.ExecuteReader();
                    if (objReader.Read())
                    {
                        Equipments.EquipmentID = int.Parse(objReader["EquipmentID"].ToString());
                        Equipments.FactoryID = int.Parse(objReader["FactoryID"].ToString());
                        Equipments.DivisionID = int.Parse(objReader["DivisionID"].ToString());
                        Equipments.EquipmentTagID = objReader["EquipmentTagID"].ToString();
                        Equipments.EquipmentName = objReader["EquipmentName"].ToString();
                        Equipments.EquipmentDescription = objReader["EquipmentDescription"].ToString();
                        Equipments.IsEquipment = int.Parse(objReader["IsEquipment"].ToString()) > 0 ? true : false;
                        Equipments.IsActive = int.Parse(objReader["IsActive"].ToString()) > 0 ? true : false;
                        if (objReader["EquipmentType"] != DBNull.Value)
                        {
                            Equipments.EqTypeID = int.Parse(objReader["EquipmentType"].ToString());
                        }

                    }

                    objReader.NextResult();
                    var divisionList = new List<SelectListItem>();
                    while (objReader.Read())
                    {
                        SelectListItem division = new SelectListItem();
                        division.Value = objReader["DivisionID"].ToString();
                        division.Text = objReader["DivisionName"].ToString();
                        divisionList.Add(division);
                    }
                    objReader.NextResult();
                    var factoryList = new List<SelectListItem>();
                    while (objReader.Read())
                    {
                        SelectListItem factory = new SelectListItem();
                        factory.Value = objReader["FactoryID"].ToString();
                        factory.Text = objReader["FactoryName"].ToString();
                        factoryList.Add(factory);
                    }

                    Equipments.FactoryList = factoryList;
                    Equipments.DivisionList = divisionList;

                }
                LogManager.Instance.Info("MonitPro.DAL.AccountDA.ValidateUser Method - End");
            }
            catch (Exception objException)
            {
                LogManager.Instance.Error(objException);
                throw objException;
            }
            return Equipments;
        }


        public int UpdateEquipment(EquipmentEntity equipmentEntity)
        {
            LogManager.Instance.Info("MonitPro.DAL.AccountDA.UpdateUserProfile Method - Start");
            int recordAffected = 0;
            try
            {

                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[EquipmentUpdate]";
                    objCom.Parameters.AddWithValue("@EquipmentID", equipmentEntity.EquipmentID);
                    objCom.Parameters.AddWithValue("@FactoryID", equipmentEntity.FactoryID);
                    objCom.Parameters.AddWithValue("@DivisionID", equipmentEntity.DivisionID);
                    objCom.Parameters.AddWithValue("@EquipmentTagID", equipmentEntity.EquipmentTagID);
                    objCom.Parameters.AddWithValue("@EquipmentName", equipmentEntity.EquipmentName);
                    if (!string.IsNullOrEmpty(equipmentEntity.EquipmentDescription))
                    {
                        objCom.Parameters.AddWithValue("@EquipmentDescription", equipmentEntity.EquipmentDescription);
                    }
                    else
                    {
                        objCom.Parameters.AddWithValue("@EquipmentDescription", DBNull.Value);
                    }
                    objCom.Parameters.AddWithValue("@EquipmentType", equipmentEntity.EqTypeID);
                    objCom.Parameters.AddWithValue("@IsEquipment", equipmentEntity.IsEquipment);
                    objCom.Parameters.AddWithValue("@IsActive", equipmentEntity.IsActive);
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCon.Open();
                    objCom.Connection = objCon;

                    recordAffected = objCom.ExecuteNonQuery();


                }
                LogManager.Instance.Info("MonitPro.DAL.AccountDA.UpdateUserProfile Method - End");
            }
            catch (Exception objException)
            {
                LogManager.Instance.Error(objException);
                throw objException;
            }

            return recordAffected;

        }
        #endregion


        #region UpdateApprovalList
        public int UpdateApproved(ApprovalViewModel approveViewModel)
        {
            LogManager.Instance.Info("MonitPro.DAL.AccountDA.UpdateUserProfile Method - Start");
            int affectedRecordCount = 0;
            try

            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {

                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[ApprovalListUpdate]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@ID", approveViewModel.ID);
                    objCom.Parameters.AddWithValue("@ApprovedOrRejectedBy", approveViewModel.ApprovedOrRejectedBy);
                    objCom.Parameters.AddWithValue("@ApprovedOrRejectedOn", approveViewModel.ApprovedOrRejectedOn);
                    objCom.Parameters.AddWithValue("@Status", approveViewModel.Status);
                    objCon.Open();
                    objCom.Connection = objCon;
                    affectedRecordCount = objCom.ExecuteNonQuery();

                }
                LogManager.Instance.Info("MonitPro.DAL.AccountDA.UpdateUserProfile Method - End");
            }
            catch (Exception exception)
            {
                LogManager.Instance.Error(exception);
                throw new Exception(exception.Message);
            }


            return affectedRecordCount;
        }
        #endregion
    }
}
