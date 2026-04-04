using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MVC_Project.Services.ClassSection;
using Microsoft.AspNetCore.Authorization;
using MVC_Project.Services.Notification;

namespace MVC_Project.Controllers
{
    [Route("ClassSection")]
    public class ClassSectionController : Controller
    {
        private readonly IClassSectionService _classSectionService;
        private readonly INotificationService _notificationService;

        public ClassSectionController(IClassSectionService classSectionService, INotificationService notificationService)
        {
            _classSectionService = classSectionService;
            _notificationService = notificationService;
        }

        [HttpGet("Download")]
        public IActionResult Download()
        {
            var uploadTypes = new List<Models.UploadType>
            {
                new Models.UploadType { Id = 1, Name = "Study Material" },
                new Models.UploadType { Id = 2, Name = "Syllabus" },
                new Models.UploadType { Id = 3, Name = "Other" }
            };

            ViewBag.UploadTypes = uploadTypes;
            return View();
        }

        [HttpGet("GetClassesBySession")]
        public async Task<IActionResult> GetClassesBySession(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                return BadRequest("Session ID is required.");
            }

            var result = await _classSectionService.GetClassSectionBySessionAsync(sessionId);
            return Json(new {
                isSuccess = result.IsSuccess,
                status = result.Status,
                message = result.Message,
                responseData = result.ResponseData ?? new List<Models.ClassSectionDto>()
            });
        }

        [HttpGet("GetSubjectsByClassId")]
        public async Task<IActionResult> GetSubjectsByClassId(int classId)
        {
            if (classId <= 0)
            {
                return BadRequest("Invalid Class ID.");
            }

            var result = await _classSectionService.GetSubjectsByClassIdAsync(classId);
            return Json(new {
                isSuccess = result.IsSuccess,
                status = result.Status,
                message = result.Message,
                responseData = result.ResponseData ?? new List<Models.SubjectDto>()
            });
        }

        [HttpGet("GetSectionsByClassId")]
        public async Task<IActionResult> GetSectionsByClassId(int classId)
        {
            if (classId <= 0)
            {
                return BadRequest("Invalid Class ID.");
            }

            var result = await _classSectionService.GetSectionsByClassIdAsync(classId);
            return Json(new {
                isSuccess = result.IsSuccess,
                status = result.Status,
                message = result.Message,
                responseData = result.ResponseData ?? new List<Models.SectionDto>()
            });
        }

        [HttpPost("AddDownload")]
        public async Task<IActionResult> AddDownload([FromForm] Models.Notification.DownloadUploadRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Title) || (request.ActionType == 1 && request.File == null))
            {
                return Json(new { isSuccess = false, message = "Title and File are required." });
            }

            if (request.File != null)
            {
                var extension = System.IO.Path.GetExtension(request.File.FileName).ToLower();
                if (extension != ".pdf")
                {
                    return Json(new { isSuccess = false, message = "Only PDF files are allowed." });
                }
            }

            var result = await _notificationService.AddOrUpdateDownloadsAsync(request);
            return Json(new {
                isSuccess = result.IsSuccess,
                message = result.Message
            });
        }

        [HttpGet("GetDownloadTable")]
        public async Task<IActionResult> GetDownloadTable(int classId, int subjectId, int uploadType)
        {
            var id = $"2_{classId},{subjectId},{uploadType}";
            var result = await _notificationService.GetNotificationDataAsync(id);
            return Json(new {
                isSuccess = result.IsSuccess,
                message = result.Message,
                responseData = result.ResponseData ?? new List<Models.Notification.NotificationResponse>()
            });
        }
    }
}
