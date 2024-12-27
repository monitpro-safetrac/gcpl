using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MonitPro.BLL;
using System.Security.Principal;
using System.Threading;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;
using System.Web.SessionState;

namespace MonitPro.Models
{

    public class CurrentUser : Controller
    {
        public static UserEntityModel UserInfo
        {
            set
            {
                System.Web.HttpContext.Current.Session["UserInfo"] = value;
            }
            get
            {
                return (UserEntityModel)System.Web.HttpContext.Current.Session["UserInfo"];
            }
        }
        public static string CurrentSessionID
        {
            get
            {
                string sessionid = System.Web.HttpContext.Current.Session.SessionID;

                return sessionid;
            }
        }
        public static bool IsInRole(string RoleName)
        {

            UserEntityModel userEntity = (UserEntityModel)System.Web.HttpContext.Current.Session["UserInfo"];
            if (userEntity != null)
            {
                return userEntity.Roles.Find(a => a.RoleName == RoleName) != null ? true : false;
            }
            else
            {
                return false;
            }
        }

        public static int UserID
        {
            get
            {
                UserEntityModel userEntity = (UserEntityModel)System.Web.HttpContext.Current.Session["UserInfo"];

                return userEntity != null ? userEntity.ID : 0;
            }
        }

        public static string UserName
        {
            get
            {
                UserEntityModel userEntity = (UserEntityModel)System.Web.HttpContext.Current.Session["UserInfo"];

                return userEntity != null ? userEntity.UserName : string.Empty;
            }
        }

        public static string FirstName
        {
            get
            {
                UserEntityModel userEntity = (UserEntityModel)System.Web.HttpContext.Current.Session["UserInfo"];

                return userEntity != null ? userEntity.FirstName : string.Empty;
            }
        }


        public static string LastName
        {
            get
            {
                UserEntityModel userEntity = (UserEntityModel)System.Web.HttpContext.Current.Session["UserInfo"];

                return userEntity != null ? userEntity.LastName : string.Empty;
            }
        }

        public static string FullName
        {
            get
            {
                UserEntityModel userEntity = (UserEntityModel)System.Web.HttpContext.Current.Session["UserInfo"];

                return userEntity != null ? userEntity.FullName : string.Empty;
            }
        }

        public static string ProfileImage
        {
            get
            {
                UserEntityModel userEntity = (UserEntityModel)System.Web.HttpContext.Current.Session["UserInfo"];

                return userEntity != null ? userEntity.ProfileImage : "notfound.jpg";
            }
        }
        public static string IsRestrict
        {
            get
            {
                UserEntityModel userEntity = (UserEntityModel)System.Web.HttpContext.Current.Session["UserInfo"];
                return userEntity != null ? userEntity.IsRestrict : string.Empty;
            }
        }
        public static int DepartmentID
        {
            get
            {
                UserEntityModel userEntity = (UserEntityModel)System.Web.HttpContext.Current.Session["UserInfo"];

                return userEntity != null ? userEntity.Departmentid : 0;
            }
        }
        public static int Designation
        {
            get
            {
                UserEntityModel userEntity = (UserEntityModel)System.Web.HttpContext.Current.Session["UserInfo"];

                return userEntity != null ? userEntity.DesigID : 0;
            }
        }

        public static List<Role> Roles
        {
            get
            {
                UserEntityModel userEntity = (UserEntityModel)System.Web.HttpContext.Current.Session["UserInfo"];
                return userEntity.Roles;
            }
        }

        public static void Logout()
        {
            System.Web.HttpContext.Current.Session.Abandon();
        }
    }
}