using Microsoft.AspNetCore.Mvc;
using MVC_Project.Constants;
using MVC_Project.Extensions;
using MVC_Project.Models.Auth;
using MVC_Project.Services.Auth;

namespace MVC_Project.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            HttpContext.Session.Clear();
            return View(new LoginRequest());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequest request, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(request);

            request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            var result = await _authService.LoginAsync(request, ct);

            if (result?.IsSuccess == true && result.ResponseData != null)
            {
                var data = result.ResponseData;

                var session = new UserSession
                {
                    UserId = data.UserId,
                    UserName = request.Username,
                    FullName = data.UserFullName,
                    Email = data.UserEmail,
                    Phone = data.UserPhoneNo,
                    UserTypeId = data.UserTypeId.ToString(),
                    UserTypeName = data.UserTypeId == 1 ? "Admin" : "User",
                    Token = data.Token,
                    PhotoPath = data.PhotoPath,
                    Session = data.Session,
                    SessionId = data.SessionId,
                    Dashboard = data.Dashboard
                };

                HttpContext.Session.SetObject(SessionKeys.UserSession, session);

                return RedirectToAction("Index", "Dashboard");
            }

            ModelState.AddModelError("", result?.Message ?? "Login failed");
            return View(request);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}