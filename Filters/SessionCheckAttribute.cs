using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MVC_Project.Constants;
using MVC_Project.Extensions;
using MVC_Project.Models.Auth;

namespace MVC_Project.Filters
{
    public class SessionCheckAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Skip checks for AuthController (Login/Logout) to avoid infinite loops
            var controllerName = context.RouteData.Values["controller"]?.ToString();
            var actionName = context.RouteData.Values["action"]?.ToString();

            if (controllerName?.Equals("Auth", StringComparison.OrdinalIgnoreCase) == true)
            {
                base.OnActionExecuting(context);
                return;
            }

            // Check if session exists
            var session = context.HttpContext.Session;
            var userSession = session.GetObject<UserSession>(SessionKeys.UserSession);

            if (userSession == null)
            {
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
            }

            base.OnActionExecuting(context);
        }

        private bool IsAjaxRequest(HttpRequest request)
        {
            return request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }
    }
}
