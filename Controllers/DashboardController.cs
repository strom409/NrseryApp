using Microsoft.AspNetCore.Mvc;
using MVC_Project.Constants;
using MVC_Project.Extensions;
using MVC_Project.Models.Auth;

namespace MVC_Project.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            var session = HttpContext.Session.GetObject<UserSession>(SessionKeys.UserSession);

            if (session == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewBag.FullName = session.FullName;
            return View();
        }
    }
}