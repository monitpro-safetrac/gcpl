using MonitPro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MonitPro.BLL;
using MonitPro.Validations;
using System.Data.SqlClient;
using System.Data;
using System.Xml.Serialization;
using System.IO;
using MonitPro.Common.Library;
using System.Configuration;
using ClosedXML.Excel;

namespace ValsparApp.Controllers
{
    [ValidateSession]
    public class AdminController : Controller
    {

        #region CreateUser

        [HttpGet]
        [AuthorizedUsers("Administrator", "HSE Manager")]
        public ActionResult Register()
        {
            AdminBLL adminBLL = new AdminBLL();
            UserRegister userRegister = new UserRegister();
            SessionDetails sess = new SessionDetails();
            sess = adminBLL.GetSession(CurrentUser.UserID);
            userRegister.CurrentSessionID = CurrentUser.CurrentSessionID;
            userRegister.PrevoiusSessionID = sess.SessionActive;
            if (userRegister.CurrentSessionID == userRegister.PrevoiusSessionID)
            {

            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";

            }
            userRegister.RoleList = adminBLL.GetRollList();
            userRegister.DepartmentList = adminBLL.GetDepartmentList();

            userRegister.DesignationList = adminBLL.GetDesignationList();
            userRegister.UserID = CurrentUser.UserID;
            userRegister.UserFullName = CurrentUser.FullName;
            userRegister.Roles = CurrentUser.Roles;
            userRegister.ProfileImage = CurrentUser.ProfileImage;
            userRegister.IsRestrict = CurrentUser.IsRestrict;
            return View(userRegister);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(UserRegister userRegister)
        {
            AdminBLL adminBLL = null;
            try
            {
                if (ModelState.IsValid)
                {
                    adminBLL = new AdminBLL();
                    if (adminBLL.InsertUserMaster(userRegister) > 0)
                    {
                        ViewBag.IsInsertSuccessful = true;
                    }

                }
            }
            catch (Exception objException)
            {
                ViewBag.IsValidationFailed = true;
                ModelState.AddModelError("ValidationError", objException.Message);
            }

            userRegister.RoleList = adminBLL.GetRollList();
            userRegister.DepartmentList = adminBLL.GetDepartmentList();

            userRegister.DesignationList = adminBLL.GetDesignationList();
            userRegister.UserFullName = CurrentUser.FullName;
            userRegister.Roles = CurrentUser.Roles;
            userRegister.ProfileImage = CurrentUser.ProfileImage;
            userRegister.IsRestrict = CurrentUser.IsRestrict;
            return View(userRegister);
        }

        #endregion
        

        #region AssignWorkType

        [HttpGet]
        //[AuthorizedUsers("Supervisor")]
        public ActionResult AssignApprover()
        {
            AssignEquipments assignEquipments = null;
            try
            {
                AdminBLL adminBLL = new AdminBLL();
                assignEquipments = adminBLL.AssignEquipment(0);
            }
            catch (Exception objException)
            {
                ModelState.AddModelError("Error", objException.Message);
            }
            assignEquipments.UserID = CurrentUser.UserID;
            assignEquipments.UserFullName = CurrentUser.FullName;
            assignEquipments.Roles = CurrentUser.Roles;
            assignEquipments.ProfileImage = CurrentUser.ProfileImage;
            assignEquipments.IsRestrict = CurrentUser.IsRestrict;
            return View(assignEquipments);
        }

        #endregion


        #region AssignWorkType
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[AuthorizedUsers("Supervisor")]
        public ActionResult AssignApprover(AssignEquipments assignEquipments)
        {
            //    try
            //    {
            //        AdminBLL adminBLL = new AdminBLL();

            //        if (Request.Form["GetEquipmentList"] == "1")
            //        {
            //            int userID = assignEquipments.SelectedUserID;
            //            assignEquipments = adminBLL.AssignEquipment(userID);
            //            assignEquipments.SelectedUserID = userID;
            //        }

            //        if (Request.Form["SaveUserEquipments"] == "1")
            //        {
            //            List<int> equipmentList = assignEquipments.Equipments.Where(x => x.Assigned == true).Select(y => y.EquipmentID).ToList();

            //            MeasureDataUsersEntity measureDataUsersEntity = new MeasureDataUsersEntity();
            //            measureDataUsersEntity.AssignTo = assignEquipments.SelectedUserID;
            //            measureDataUsersEntity.AssignedBy = CurrentUser.UserID;
            //            measureDataUsersEntity.EquipmentList = equipmentList;

            //            //foreach (var item in assignEquipments.Equipments)
            //            //{
            //            //    if (item.Assigned ==true)
            //            //    {
            //            //        equipmentList.Add(item.EquipmentID);
            //            //        listEquipments.Add( 
            //            //            new MeasureDataUsersEntity
            //            //            {
            //            //                assi =assignEquipments.SelectedUserID,
            //            //                EquipmentID =item.EquipmentID,
            //            //                AssignedBy =CurrentUser.UserID 
            //            //             });
            //            //        //int recordCount = adminBLL.MeasureDataUsersInsert(listEquipments);
            //            //        //ViewBag.IsInsertSuccessful = true;
            //            //     }
            //            //}

            //            int recordCount = adminBLL.MeasureDataUsersInsert(measureDataUsersEntity);
            //            ViewBag.IsInsertSuccessful = true;

            //            int userID = assignEquipments.SelectedUserID;
            //            assignEquipments = adminBLL.AssignEquipment(userID);
            //            assignEquipments.SelectedUserID = userID;
            //        }
            //    }
            //    catch (Exception objException)
            //    {
            //        ModelState.AddModelError("ServerSideError", objException.Message);
            //        ViewBag.IsServerSideError = true;
            //    }

            //    ModelState.Clear();
            //    assignEquipments.UserID = CurrentUser.UserID;
            //    assignEquipments.Roles = CurrentUser.Roles;
            //    assignEquipments.UserFullName = CurrentUser.FullName;
            //    assignEquipments.ProfileImage = CurrentUser.ProfileImage;
            return View(assignEquipments);
        }





        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet ds = new DataSet();


        #endregion


        #region AssignApprover

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




        #region UserProfileUpdate

        [HttpGet]
        [AuthorizedUsers("Administrator", "HSE Manager")]
        public ActionResult UserList()
        {
            AdminBLL accountBLL = new AdminBLL();
            var userProfile = accountBLL.SelectUserProfile();
            SessionDetails sess = new SessionDetails();
            sess = accountBLL.GetSession(CurrentUser.UserID);
            Profile profile = new Profile();
            profile.CurrentSessionID = CurrentUser.CurrentSessionID;
            profile.PrevoiusSessionID = sess.SessionActive;
            if (profile.CurrentSessionID == profile.PrevoiusSessionID)
            {

            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";

            }

            profile.DesignationList = accountBLL.GetDesignationList();
            profile.DepartmentList = accountBLL.GetDepartmentList();
            profile.RoleList = accountBLL.GetRollList();
            profile.UserProfile = userProfile;
            profile.UserFullName = CurrentUser.FullName;
            profile.Roles = CurrentUser.Roles;
            profile.ProfileImage = CurrentUser.ProfileImage;
            profile.IsRestrict = CurrentUser.IsRestrict;
            return View(profile);
        }

        [HttpPost]
        public ActionResult UserList(Profile user)
        {
            AdminBLL adminBLL = new AdminBLL();
            AdminBLL accountBLL = new AdminBLL();
            var userProfile = adminBLL.GetUserName(user);
            Profile profile = new Profile();

            profile.DesignationList = accountBLL.GetDesignationList();
            profile.DepartmentList = accountBLL.GetDepartmentList();
            profile.RoleList = accountBLL.GetRollList();
            profile.UserProfile = userProfile;
            profile.UserFullName = CurrentUser.FullName;
            profile.Roles = CurrentUser.Roles;
            profile.ProfileImage = CurrentUser.ProfileImage;
            profile.IsRestrict = CurrentUser.IsRestrict;
            return View(profile);
        }

        public ActionResult ExportUserList()
        {

            using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "ExportUserList";
                objCom.CommandType = CommandType.StoredProcedure;


                objCon.Open();
                objCom.Connection = objCon;
                SqlDataAdapter Adapter = new SqlDataAdapter();
                Adapter.SelectCommand = objCom;
                DataSet dataSet = new DataSet();
                Adapter.Fill(dataSet);
                objCon.Close();
                var wb = new XLWorkbook(Server.MapPath("~/Template/UserList.xlsx"));
                // var worksheet = wb.Worksheet("ContractRatingList");
                var worksheet = wb.Worksheet(1);
                worksheet.Cell("D4").Value = "Report Generated by : " + CurrentUser.FullName;

                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    var rangeWithDataSecond = worksheet.Cell(7, 1).InsertTable(dataSet.Tables[0].AsEnumerable());
                }
                else
                {
                    worksheet.Cell("E8").Value = "No Data Found";
                }

                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename= UserList.xlsx");
                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }

            }


            return View();


        }

        [HttpGet]
        public ActionResult UpdateUserProfile(int id)
        {
            AdminBLL adminBLL = new AdminBLL();
            UserProfile userProfile = null;
            userProfile = adminBLL.GetUserProfile(id);
            userProfile.DesignationList = adminBLL.GetDesignationList();
            SessionDetails sess = new SessionDetails();
            sess = adminBLL.GetSession(CurrentUser.UserID);
            userProfile.CurrentSessionID = CurrentUser.CurrentSessionID;
            userProfile.PrevoiusSessionID = sess.SessionActive;
            if (userProfile.CurrentSessionID == userProfile.PrevoiusSessionID)
            {

            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";

            }  
            userProfile.UserFullName = CurrentUser.FullName;
            userProfile.Roles = CurrentUser.Roles;
            userProfile.ProfileImage = CurrentUser.ProfileImage;
            userProfile.IsRestrict = CurrentUser.IsRestrict;
            return View(userProfile);
        }


        [HttpPost]

        public ActionResult UpdateUserProfile(UserProfile userProfile)
        {
            AdminBLL adminBLL = null;
            try
            {
                adminBLL = new AdminBLL();
                adminBLL.UpdateUserProfile(userProfile);
                return RedirectToAction("UserList");
            }
            catch (Exception exception)
            {
                for (int i = 0; i < 3; i++)
                {
                    userProfile.Password = MonitPro.Security.Security.Instance.Decrypt(userProfile.Password);
                }
                ViewBag.IsValidationFailed = true;
                ModelState.AddModelError("ValidationError", exception.Message);
            }
            adminBLL = new AdminBLL();
           
            userProfile.RoleList = adminBLL.GetRollList();
            userProfile.DepartmentList = adminBLL.GetDepartmentList();
            userProfile.DesignationList = adminBLL.GetDesignationList();
            userProfile.UserFullName = CurrentUser.FullName;
            userProfile.Roles = CurrentUser.Roles;
            userProfile.ProfileImage = CurrentUser.ProfileImage;
            userProfile.IsRestrict = CurrentUser.IsRestrict;
            return View(userProfile);
        }


        #endregion

        [HttpGet]
        public ActionResult LoginHistory()
        {
            LoginHistory loginHistory = new LoginHistory();
            AdminBLL adminBLL = new AdminBLL();
            SessionDetails sess = new SessionDetails();
            sess = adminBLL.GetSession(CurrentUser.UserID);
            loginHistory.CurrentSessionID = CurrentUser.CurrentSessionID;
            loginHistory.PrevoiusSessionID = sess.SessionActive;
            if (loginHistory.CurrentSessionID == loginHistory.PrevoiusSessionID)
            {

            }
            else
            {
                ViewBag.SessMessage = "Session Already Exists";

            }
            loginHistory.FromDate = DateTime.Now.ToString("dd/MM/yyyy 00:00:00");
            loginHistory.ToDate = DateTime.Now.ToString("dd/MM/yyyy 23:59:00");
            ViewBag.fromdate = loginHistory.FromDate;
            ViewBag.Todate = loginHistory.ToDate;
            using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "GetLoginLogout";
                objCom.Parameters.AddWithValue("@Fromdate", loginHistory.FromDate);
                objCom.Parameters.AddWithValue("@Todate", loginHistory.ToDate);
                objCom.CommandType = CommandType.StoredProcedure;
                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();
                var loginlist = new List<LoginTimeList>();
                while (objReader.Read())
                {
                    var login = new LoginTimeList();
                    login.FirstName = objReader["FirstName"].ToString();
                    login.LastName = objReader["LastName"].ToString();
                    login.LoginTime = objReader["LoginTime"].ToString();
                    login.EmployeeID = objReader["EmployeeID"].ToString();
                    if (objReader["Logout"] != DBNull.Value)
                    {
                        login.LogOutTime = objReader["Logout"].ToString();
                    }

                    loginlist.Add(login);
                }

                objCon.Close();
                loginHistory.LoginList = loginlist;

            }
            loginHistory.Roles = CurrentUser.Roles;
            loginHistory.UserFullName = CurrentUser.FullName;
            loginHistory.ProfileImage = CurrentUser.ProfileImage;
            loginHistory.IsRestrict = CurrentUser.IsRestrict;
            return View(loginHistory);


        }

        [HttpPost]
        public ActionResult LoginHistory(LoginHistory loginHistory)
        {

            ViewBag.fromdate = loginHistory.FromDate;
            ViewBag.Todate = loginHistory.ToDate;
            using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "GetLoginLogout";
                objCom.Parameters.AddWithValue("@Fromdate", loginHistory.FromDate);
                objCom.Parameters.AddWithValue("@Todate", loginHistory.ToDate);
                objCom.CommandType = CommandType.StoredProcedure;
                objCon.Open();
                objCom.Connection = objCon;
                var objReader = objCom.ExecuteReader();
                var loginlist = new List<LoginTimeList>();
                while (objReader.Read())
                {
                    var login = new LoginTimeList();
                    login.FirstName = objReader["FirstName"].ToString();
                    login.LastName = objReader["LastName"].ToString();
                    login.LoginTime = objReader["LoginTime"].ToString();
                    login.EmployeeID = objReader["EmployeeID"].ToString();
                    if (objReader["Logout"] != DBNull.Value)
                    {
                        login.LogOutTime = objReader["Logout"].ToString();
                    }

                    loginlist.Add(login);
                }

                objCon.Close();
                loginHistory.LoginList = loginlist;

            }
            loginHistory.Roles = CurrentUser.Roles;
            loginHistory.UserFullName = CurrentUser.FullName;
            loginHistory.ProfileImage = CurrentUser.ProfileImage;
            loginHistory.IsRestrict = CurrentUser.IsRestrict;
            return View(loginHistory);


        }

        public ActionResult ExportLoginHistory(string currentFromDate, string currentTodate)
        {

            using (SqlConnection objCon = new SqlConnection(ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString))
            {
                var objCom = new SqlCommand();
                objCom.CommandText = "ExportLoginHistory";
                objCom.CommandType = CommandType.StoredProcedure;
                objCom.Parameters.AddWithValue("@Fromdate", currentFromDate);
                objCom.Parameters.AddWithValue("@Todate", currentTodate);

                objCon.Open();
                objCom.Connection = objCon;
                SqlDataAdapter Adapter = new SqlDataAdapter();
                Adapter.SelectCommand = objCom;
                DataSet dataSet = new DataSet();
                Adapter.Fill(dataSet);
                objCon.Close();
                var wb = new XLWorkbook(Server.MapPath("~/Template/LoginHistoryList.xlsx"));
                // var worksheet = wb.Worksheet("ContractRatingList");
                var worksheet = wb.Worksheet(1);
                worksheet.Cell("C4").Value = "Report Generated by : " + CurrentUser.FullName;

                worksheet.Cell("C5").Value = "Report Duration : ";
                worksheet.Cell("D5").Value = currentFromDate + " to  " + currentTodate;

                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    var rangeWithDataSecond = worksheet.Cell(7, 1).InsertTable(dataSet.Tables[0].AsEnumerable());
                }
                else
                {
                    worksheet.Cell("C8").Value = "No Data Found";
                }

                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename= LoginHistory.xlsx");
                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }

            }


            return View();


        }
    }
}