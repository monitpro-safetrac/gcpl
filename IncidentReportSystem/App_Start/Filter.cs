using System;
using System.Diagnostics;
using System.Web.Mvc;
using System.Web.Routing;
using MonitPro.Common.Library;
using System.Web;
using MonitPro.Models;
namespace MonitPro.Validations
{
    //[ValidateSession]
    public class AuthorizedUsersAttribute : AuthorizeAttribute
    {

        private readonly string[] AllowedRoles;
        public AuthorizedUsersAttribute(params string[] roles)
        {
            this.AllowedRoles = roles;
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {


            UserEntityModel userEntity = (UserEntityModel)System.Web.HttpContext.Current.Session["UserInfo"];

            if (userEntity != null)
            {
                foreach (string Role in AllowedRoles)
                {
                    if (userEntity.Roles.Find(x => x.RoleName == Role) != null)
                        return true;
                }
            }

            return false;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (System.Web.HttpContext.Current.Session["UserInfo"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                   new RouteValueDictionary {
                                                   { "action", "Login" },
                                                    { "Controller", "Account" }
                                                }
                                             );
            }

            filterContext.Result = new RedirectToRouteResult(
                      new RouteValueDictionary {
                                                   { "action", "Unauthorized" },
                                                    { "Controller", "Incident" }
                                                }
                                                );
        }
    }


    public class ValidateSession : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (System.Web.HttpContext.Current.Session["UserInfo"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                   new RouteValueDictionary {
                                                   { "action", "Login" },
                                                    { "Controller", "Account" }
                                             }
                                             );
            }
            else
            {

                Log("OnActionExecuting", filterContext.RouteData);
            }
        }

        //public override void OnActionExecuted(ActionExecutedContext filterContext)
        //{
        //    Log("OnActionExecuted", filterContext.RouteData);
        //}

        //public override void OnResultExecuting(ResultExecutingContext filterContext)
        //{
        //    Log("OnResultExecuting", filterContext.RouteData);
        //}

        //public override void OnResultExecuted(ResultExecutedContext filterContext)
        //{
        //    Log("OnResultExecuted", filterContext.RouteData);
        //}


        private void Log(string methodName, RouteData routeData)
        {
            //     var contRoleerName = routeData.Values["contRoleer"];
            //     var actionName = routeData.Values["action"];
            //     var message = String.Format("{0} Controller:{1} action:{2}", methodName, contRoleerName, actionName);
            //     Debug.WriteLine(message, "Action Filter Log");
            //     LogManager.Instance.Info(message);
        }

    }
}