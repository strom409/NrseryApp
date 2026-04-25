using Microsoft.AspNetCore.Mvc;
using MVC_Project.Constants;
using MVC_Project.Extensions;
using MVC_Project.Models.Auth;
using MVC_Project.Models.StudentMessage;
using MVC_Project.Services.StudentMessage;
using MVC_Project.Services.ClassSection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MVC_Project.Controllers
{
    public class StudentMessageController : Controller
    {
        private readonly IStudentMessageService _studentMessageService;
        private readonly IClassSectionService _classSectionService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public StudentMessageController(
            IStudentMessageService studentMessageService, 
            IClassSectionService classSectionService,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
        {
            _studentMessageService = studentMessageService;
            _classSectionService = classSectionService;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            string session = _configuration["Session"] ?? "2025-26";
            var classesResponse = await _classSectionService.GetClassSectionBySessionAsync(session);
            ViewBag.Classes = classesResponse.ResponseData ?? new List<Models.ClassSectionDto>();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetSections(int classId)
        {
            var result = await _classSectionService.GetSectionsByClassIdAsync(classId);
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetStudents(string sectionId)
        {
            var result = await _studentMessageService.GetStudentsBySectionAsync(sectionId);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] StudentMessageAddRequest model)
        {
            var session = _httpContextAccessor.HttpContext?.Session.GetObject<UserSession>(SessionKeys.UserSession);
            if (session == null) return Json(new { isSuccess = false, message = "Session expired." });

            if (model == null) return Json(new { isSuccess = false, message = "Invalid request." });

            model.AddedBy = session.UserName ?? "Admin";
            model.AddedOn = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            
            // If Whole School (Phase 1), rollNo is already "0" by default or from the form.

            try
            {
                var result = await _studentMessageService.AddStudentMessageAsync(model);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, message = "Connection error." });
            }
        }

        public async Task<IActionResult> ViewMessages()
        {
            string session = _configuration["Session"] ?? "2025-26";
            var classesResponse = await _classSectionService.GetClassSectionBySessionAsync(session);
            ViewBag.Classes = classesResponse.ResponseData ?? new List<Models.ClassSectionDto>();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetMessagesList(int classId, int sectionId)
        {
            // Format: 1_{classId},{sectionId}
            var id = $"1_{classId},{sectionId}";
            var result = await _studentMessageService.GetStudentMessagesAsync(id);
            return Json(result);
        }
    }
}
