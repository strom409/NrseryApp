using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MVC_Project.Constants;
using MVC_Project.Extensions;
using MVC_Project.Models.Auth;

namespace MVC_Project.Filters
{
    public class AuthorizeRoleAttribute : ActionFilterAttribute
    {
        private readonly int[] _allowedRoles;

        public AuthorizeRoleAttribute(params int[] allowedRoles)
        {
            _allowedRoles = allowedRoles;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var userSession = session.GetObject<UserSession>(SessionKeys.UserSession);

            if (userSession == null)
            {
                // SessionCheckAttribute often handles this, but good to be safe
                if (IsAjaxRequest(context.HttpContext.Request))
                {
                    context.Result = new JsonResult(new { isSuccess = false, message = "Session Expired", isRedirect = true, redirectUrl = "/Auth/Login" })
                    {
                        StatusCode = 401
                    };
                }
                else
                {
                    context.Result = new RedirectToActionResult("Login", "Auth", null);
                }
                return;
            }

            // Check Role
            if (int.TryParse(userSession.UserTypeId, out int userTypeId))
            {
                if (_allowedRoles.Contains(userTypeId))
                {
                    base.OnActionExecuting(context);
                    return;
                }
            }

            // Access Denied
            if (IsAjaxRequest(context.HttpContext.Request))
            {
                context.Result = new JsonResult(new { isSuccess = false, message = "Access Denied: Insufficient Permissions" }) { StatusCode = 403 };
            }
            else
            {
                // Redirect to Access Denied page or Dashboard with error
                if (context.Controller is Controller controller)
                {
                    controller.TempData["Error"] = "Access Denied: You do not have permission to view this page.";
                }
                context.Result = new RedirectToActionResult("Index", "Dashboard", null);
            }
        }

        private bool IsAjaxRequest(HttpRequest request)
        {
            return request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }
    }
}
