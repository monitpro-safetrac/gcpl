using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using MonitPro.Models;
using MonitPro.Common.Library;
using System.Xml.Serialization;
using System.IO;
namespace MonitPro.DAL
{
    public class AccountDA
    {

        #region ValidateUser
            public UserEntityModel ValidateUser(LoginViewModel loginModel)
            {
                LogManager.Instance.Info("MonitPro.DAL.AccountDA.ValidateUser Method - Start");
                UserEntityModel userEntity = new UserEntityModel();
            int affectedRecordCount = 0;
                try
                {
                    string ConnectionString = AppConfig.ConnectionString;
                using (SqlConnection objCon = new SqlConnection(ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "LoginTimeSave";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@UserName", loginModel.UserName);
                    objCom.Connection = objCon;
                    objCon.Open();
                    affectedRecordCount = objCom.ExecuteNonQuery();
                    objCon.Close();
                }
                using (SqlConnection objCon = new SqlConnection(ConnectionString))
                    {
                        SqlCommand objCom = new SqlCommand();
                        objCom.CommandText = "ValidateUser";
                        objCom.CommandType = CommandType.StoredProcedure;
                        objCom.Parameters.AddWithValue("@UserName", loginModel.UserName);
                        objCom.Parameters.AddWithValue("@Password", loginModel.Password);
                    objCom.Parameters.AddWithValue("@SessionID", loginModel.SessionActiveID);
                    objCon.Open();
                        objCom.Connection = objCon;

                        SqlDataReader objReader = objCom.ExecuteReader();

                        List<Role> roleList=new List<Role>{};

                        //userEntity.ID = 10;
                        //userEntity.UserName = "Test";
                        //userEntity.FirstName = "Fname";
                        //userEntity.LastName = "Lname";
                        //userEntity.FullName = "Fname xyz";

                        if (objReader.Read())
                        {
                            userEntity.ID = int.Parse(objReader["UserID"].ToString());
                            userEntity.UserName = objReader["UserName"].ToString();
                            userEntity.FirstName = objReader["FirstName"].ToString();
                            userEntity.LastName = objReader["LastName"].ToString();
                            userEntity.FullName = objReader["FullName"].ToString();
                            userEntity.ProfileImage = objReader["UserImage"].ToString();
                            userEntity.IsRestrict = objReader["IsResctrictedAccess"].ToString();
                            userEntity.Departmentid = int.Parse(objReader["DepartmentID"].ToString());
                            userEntity.IsActive = objReader["IsActive"].ToString();
                    }

                        objReader.NextResult();

                        while (objReader.Read())
                        {
                            Role role=new Role();
                            role.RoleID = int.Parse(objReader["RoleID"].ToString());
                            role.RoleName = objReader["RoleName"].ToString();
                            roleList.Add(role);
                        }
                        userEntity.Roles = roleList;
                    }
                    LogManager.Instance.Info("MonitPro.DAL.AccountDA.ValidateUser Method - End");
                }catch(Exception objException)
                {
                     LogManager.Instance.Error(objException);
                     throw new Exception(objException.Message);
                }
                return userEntity;
        }
        #endregion
        #region ValidateUser
        public UserEntityModel ValidateUserLDAP(LoginViewModel loginModel)
        {
            LogManager.Instance.Info("MonitPro.DAL.AccountDA.ValidateUserLDAP Method - Start");
            UserEntityModel userEntity = new UserEntityModel();
            int affectedRecordCount = 0;
            try
            {
                string ConnectionString = AppConfig.ConnectionString;
                using (SqlConnection objCon = new SqlConnection(ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "LoginTimeSave";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@UserName", loginModel.UserName);
                    objCom.Connection = objCon;
                    objCon.Open();
                    affectedRecordCount = objCom.ExecuteNonQuery();
                    objCon.Close();
                }
                using (SqlConnection objCon = new SqlConnection(ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "ValidateUserLDAP";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCom.Parameters.AddWithValue("@UserName", loginModel.UserName);             
                    objCom.Parameters.AddWithValue("@SessionID", loginModel.SessionActiveID);
                    objCon.Open();
                    objCom.Connection = objCon;

                    SqlDataReader objReader = objCom.ExecuteReader();

                    List<Role> roleList = new List<Role> { };

                    if (objReader.Read())
                    {
                        userEntity.ID = int.Parse(objReader["UserID"].ToString());
                        userEntity.UserName = objReader["UserName"].ToString();
                        userEntity.FirstName = objReader["FirstName"].ToString();
                        userEntity.LastName = objReader["LastName"].ToString();
                        userEntity.FullName = objReader["FullName"].ToString();
                        userEntity.ProfileImage = objReader["UserImage"].ToString();

                    }

                    objReader.NextResult();

                    while (objReader.Read())
                    {
                        Role role = new Role();
                        role.RoleID = int.Parse(objReader["RoleID"].ToString());
                        role.RoleName = objReader["RoleName"].ToString();
                        roleList.Add(role);
                    }
                    userEntity.Roles = roleList;
                }
                LogManager.Instance.Info("MonitPro.DAL.AccountDA.ValidateUserLDAP Method - End");
            }
            catch (Exception objException)
            {
                LogManager.Instance.Error(objException);
                throw new Exception(objException.Message);
            }
            return userEntity;
        }
        #endregion
        public void InsertLogout(int userid)
        {
            LogManager.Instance.Info("MonitPro.DAL.AccountDA.ValidateUser Method - Start");

            int affectedRecordCount = 0;

            string ConnectionString = AppConfig.ConnectionString;

            using (SqlConnection objCon = new SqlConnection(ConnectionString))
            {
                SqlCommand objCom = new SqlCommand();
                objCom.CommandText = "LogoutTimeSave";
                objCom.CommandType = CommandType.StoredProcedure;
                objCom.Parameters.AddWithValue("@UserID", userid);

                objCom.Connection = objCon;
                objCon.Open();
                affectedRecordCount = objCom.ExecuteNonQuery();
                objCon.Close();
            }
        }


        #region UpdateUserProfile
        public int UpdateUserProfile(UserProfile userProfile)
        {
            LogManager.Instance.Info("MonitPro.DAL.AccountDA.UpdateUserProfile Method - Start");
            int recordAffected=0;

            string rolevalue = string.Empty;
            List<Rolexml> rolexmls = new List<Rolexml>();
            try
            {
                 using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[UserMasterUpdate]";
                    objCom.Parameters.AddWithValue("@UserID", userProfile.UserID);
                    objCom.Parameters.AddWithValue("@FirstName", userProfile.FirstName);
                    objCom.Parameters.AddWithValue("@LastName", userProfile.LastName);
                    objCom.Parameters.AddWithValue("@EmailAddress", userProfile.EmailAddress);

                    if (!string.IsNullOrEmpty(userProfile.MobileNo))
                    {
                        objCom.Parameters.AddWithValue("@MobileNo", userProfile.MobileNo);
                    }
                    else
                    {
                        objCom.Parameters.AddWithValue("@MobileNo", DBNull.Value);
                    }

                    objCom.Parameters.AddWithValue("@UserName", userProfile.UserName);
                    objCom.Parameters.AddWithValue("@Password", userProfile.Password);


                   
                        objCom.Parameters.AddWithValue("@Designation", userProfile.DesigID);
                    

                    objCom.Parameters.AddWithValue("@DepartmentID", userProfile.DepartID);

                    if (!string.IsNullOrEmpty(userProfile.UserImage))
                    {
                        objCom.Parameters.AddWithValue("@UserImage", userProfile.UserImage);
                    }
                    else
                    {
                        objCom.Parameters.AddWithValue("@UserImage", DBNull.Value);
                    }
                    
                    objCom.Parameters.AddWithValue("@EmployeeID", userProfile.EmployeeID);
                    objCom.Parameters.AddWithValue("@IsRestrictedAccess", userProfile.IsrestrictAccess == true ? "Y" : "N");
                    objCom.Parameters.AddWithValue("@IsActive", userProfile.IsActive==true?"Y":"N");
                    objCom.Parameters.AddWithValue("@IsInvestigate", userProfile.IsInvestigate == true ? "Y" : "N");
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCon.Open();
                    objCom.Connection = objCon;
                    recordAffected=objCom.ExecuteNonQuery();
                    objCom.Parameters.Clear();
                    foreach (var item in userProfile.RoleList)
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
                    objCom.Parameters.AddWithValue("@UserID", userProfile.UserID);
                    objCom.Parameters.AddWithValue("@RoleID", rolevalue);
                    recordAffected = objCom.ExecuteNonQuery();
                    objCon.Close();
                }
                LogManager.Instance.Info("MonitPro.DAL.AccountDA.UpdateUserProfile Method - End");
            }catch(Exception objException)
            {
                 LogManager.Instance.Error(objException);
                 throw new Exception(objException.Message);
            }

            return recordAffected;

        }
        #endregion

        public List<IPADDressList> GetIPAddressList()
        {
            List<IPADDressList> ip1 = null;
            try
            {
                using (SqlConnection objCon = new SqlConnection(AppConfig.ConnectionString))
                {
                    SqlCommand objCom = new SqlCommand();
                    objCom.CommandText = "[IPADDressSelect]";
                    objCom.CommandType = CommandType.StoredProcedure;
                    objCon.Open();
                    objCom.Connection = objCon;
                    SqlDataReader objReader = objCom.ExecuteReader();
                    ip1 = new List<IPADDressList>();
                    while (objReader.Read())
                    {
                        ip1.Add(new IPADDressList
                        {
                            IPID = int.Parse(objReader["ID"].ToString()),
                            INIPADdress = objReader["IPAddress"].ToString(),

                        });
                    }
                }
                LogManager.Instance.Info("MonitPro.DAL.AccountDA.SelectUserProfile Method - End");
            }
            catch (Exception objException)
            {
                LogManager.Instance.Error(objException);
                throw new Exception(objException.Message);
            }
            return ip1;
        }
    }
}
 

