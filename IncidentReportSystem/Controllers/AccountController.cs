using System;
using System.Web.Mvc;
using MonitPro.Models;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using MonitPro.BLL;
using MonitPro.Validations;
using System.Net;
using System.DirectoryServices.Protocols;
using MonitPro.Common.Library;
using System.Collections.Generic;
using System.Configuration;

namespace IncidentReportSystem.Controllers
{
   
    public class AccountController : Controller
    {
        List<IPADDressList> iplist = new List<IPADDressList>();
        AccountBLL accountBLL = new AccountBLL();
        public AccountController()
        {
            iplist = accountBLL.GetIPAddressList();
        }
    
        public ActionResult InvalidLicense()
        {
            return View();
        }

        public ActionResult LicenseAlert()
        {
            ViewBag.DaysLeft = MonitPro.Security.Security.Instance.GetLicenseDaysLeft();
            return View();
        }
        
        

        public ActionResult Login()
        {
            if (!MonitPro.Security.Security.Instance.IsLicenseValid())
                Response.RedirectToRoute("Default", new { Controller = "Account", action = "InvalidLicense" });


            return View();
        }
        public ActionResult Logout()
        {
            int current;
            current = CurrentUser.UserID;
            AccountBLL accountBLL = new AccountBLL();
            accountBLL.InsertLogout(current);
            CurrentUser.Logout();
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        
        public ActionResult Login(LoginViewModel loginModel, string returnUrl)
        {
            string ip2;
            int tim = HttpContext.Session.Timeout;
            var sesstimeout = DateTime.Now.AddMinutes(tim).ToString("dd-MM-yyyy h:mm:ss tt");
            Session["time"] = sesstimeout;
            
            UserEntityModel userEntity = new UserEntityModel();

            loginModel.SessionActiveID = HttpContext.Session.SessionID;
            loginModel.INIPList = accountBLL.GetIPAddressList();

            string ip1 = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip1))
            {
                ip1 = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            }
            ip2 = ip1.ToString();

            loginModel.OutIpAddress = ip2;

            if (ModelState.IsValid)
            {
                AccountBLL accountBLL = new AccountBLL();
                userEntity = accountBLL.ValidateUser(loginModel);
                LogManager.Instance.Info("REMOTE_ADDR" + ip2);
                if (userEntity.ID != 0)
                {
                    //Set the User Information in Session
                    CurrentUser.UserInfo = userEntity;
                    ViewBag.UserName = userEntity.UserName;

                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        Redirect(returnUrl);
                    }
                    else
                    {
                        if (((userEntity.Roles.Find(a => a.RoleName == "Administrator") != null) ||
                            (userEntity.Roles.Find(a => a.RoleName == "HSE Manager") != null)) 
                            && MonitPro.Security.Security.Instance.GetLicenseDaysLeft() <= 30)
                        { 
                            return RedirectToAction("LicenseAlert", "Account");
                        }
                        else
                        {
                            foreach (var i in loginModel.INIPList)
                            {

                                if (i.INIPADdress == loginModel.OutIpAddress)
                                {

                                    return RedirectToAction("HomePage", "Incident");
                                }
                                else
                                if ((i.INIPADdress != loginModel.OutIpAddress) && (userEntity.IsActive == "Y"))
                                {
                                    return RedirectToAction("HomePage", "Incident");
                                }
                                else
                                {
                                    ViewBag.Message = "User does not have the login privileges.Check with Admin";
                                }

                            }
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("ValidationError", "Username or Password is incorrect.");
                }
            }

            return View(loginModel);
        }
        
        public static bool fnValidateUser(LoginViewModel loginViewModel)
        {
            bool validation;
            try
            {
                LdapConnection lcon = new LdapConnection
                        (new LdapDirectoryIdentifier((string)null, false, false));
                NetworkCredential nc = new NetworkCredential(loginViewModel.UserName,
                               loginViewModel.Password, Environment.UserDomainName);
                lcon.Credential = nc;
                lcon.AuthType = AuthType.Negotiate;
                // user has authenticated at this point,
                // as the credentials were used to login to the dc.
                lcon.Bind(nc);
                validation = true;
            }
            catch (LdapException objException)
            {
                validation = false;
                LogManager.Instance.Error(objException);              
            }
            return validation;
        }
        [HttpGet]
        public ActionResult MyProfile()
        {
            AdminBLL accountBLL = new AdminBLL();
            UserProfile userProfile = accountBLL.GetUserProfile(CurrentUser.UserID);
            SessionDetails sess = new SessionDetails();
            sess = accountBLL.GetSession(CurrentUser.UserID);
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
        public ActionResult MyProfile(UserProfile userProfile)
        {
            try
            {
               
                if (userProfile.UserProfileImage!= null)
                {
                    if (userProfile.UserProfileImage.ContentLength>100000)
                    {
                        throw new Exception("Profile image should be less than or equal to 100KB.");
                    }

                    var img = Image.FromStream(userProfile.UserProfileImage.InputStream);
                 
                   if(!img.RawFormat.Equals(ImageFormat.Jpeg))
                   {
                       throw new Exception("Profile image should be JPEG format.");
                   }
 
                    var path = Path.Combine(Server.MapPath("~/Users/"), CurrentUser.UserID+".jpg");
                    userProfile.UserProfileImage.SaveAs(path);
                    userProfile.UserImage = CurrentUser.UserID + ".jpg";
                }

                AdminBLL accountBLL = new AdminBLL();
                int recordCount=accountBLL.UpdateUserProfile(userProfile);
                if (recordCount > 0)
                    ViewBag.UpdateStatus = true;
                 
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
            AdminBLL adminBLL = new AdminBLL();
            userProfile.DepartmentList = adminBLL.GetDepartmentList();
            userProfile.RoleList = adminBLL.GetRollList();
            userProfile.UserFullName = CurrentUser.FullName;
            userProfile.Roles = CurrentUser.Roles;
            userProfile.ProfileImage = CurrentUser.ProfileImage;
            userProfile.IsRestrict = CurrentUser.IsRestrict;
            return View(userProfile);
        }
         
        [HttpGet]
        public ActionResult ViewProfile(int id)
        {
            AdminBLL accountBLL = new AdminBLL();
            UserProfile userProfile = accountBLL.GetUserProfile(id);
            userProfile.UserFullName = CurrentUser.FullName;
            userProfile.Roles = CurrentUser.Roles;
            userProfile.ProfileImage = CurrentUser.ProfileImage;
            userProfile.IsRestrict = CurrentUser.IsRestrict;
            return View(userProfile);
        }

        public JsonResult GetSessionTime()
        {
            int sessionTimeoutMinutes = int.Parse(ConfigurationManager.AppSettings["SessionTimeout"]);
            var sessionTime = Session["time"] as DateTime?;
            if (!sessionTime.HasValue)
            {
                sessionTime = DateTime.Now.AddMinutes(sessionTimeoutMinutes); // Initial session timeout
                Session["time"] = sessionTime;
            }

            return Json(new { sessionTime = sessionTime.Value.ToString("o") }, JsonRequestBehavior.AllowGet);
        }

        // Extend session expiration time
        [HttpPost]
        public JsonResult ExtendSession()
        {
            int SessionExtendTime = int.Parse(ConfigurationManager.AppSettings["SessionExtendTime"]);
            var sessionTime = Session["time"] as DateTime?;
            if (sessionTime.HasValue)
            {
                sessionTime = DateTime.Now.AddMinutes(SessionExtendTime); // Extend session by minutes
                Session["time"] = sessionTime;
                return Json(new { newExpirationTime = sessionTime.Value.ToString("o") });
            }

            return Json(new { error = "Session time not found." });
        }



    }
}
