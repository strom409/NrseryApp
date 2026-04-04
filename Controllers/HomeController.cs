using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MVC_Project.Models.Notification;
using MVC_Project.Options;
using MVC_Project.Services.ClassSection;
using MVC_Project.Services.Notification;
using System.Net.Http;
using System.Threading.Tasks;

namespace MVC_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly IClassSectionService _classSectionService;
        private readonly INotificationService _notificationService;
        private readonly IConfiguration _configuration;
        private readonly ApiOptions _apiOptions;

        public HomeController(IClassSectionService classSectionService, 
            INotificationService notificationService, 
            IConfiguration configuration,
            IOptions<ApiOptions> apiOptions)
        {
            _classSectionService = classSectionService;
            _notificationService = notificationService;
            _configuration = configuration;
            _apiOptions = apiOptions.Value;
        }

        public async Task<IActionResult> Circular()
        {
            var session = _configuration.GetSection("Session").Value ?? "2025-26";
            var classResult = await _classSectionService.GetClassSectionBySessionAsync(session);
            
            ViewBag.Classes = classResult.IsSuccess ? classResult.ResponseData : new List<Models.ClassSectionDto>();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddOrUpdateCircular([FromBody] CircularAddRequest model)
        {
            if (model == null)
            {
                return Json(new { isSuccess = false, message = "Invalid request data." });
            }

            var result = await _notificationService.AddOrUpdateCircularAsync(model);
            return Json(new { isSuccess = result.IsSuccess, message = result.Message });
        }

        [HttpGet]
        public async Task<IActionResult> GetCircularById(int classId, int sectionId, long notificationId)
        {
            // The API expects format like 3_classId,sectionId to return circulars
            var id = $"3_{classId},{sectionId}";
            var result = await _notificationService.GetNotificationDataAsync(id);
            
            if (result.IsSuccess && result.ResponseData != null)
            {
                // Filter the list to find the specific record we want to edit
                var circular = result.ResponseData.FirstOrDefault(x => 
                    x.NotificationId == notificationId || 
                    (x.NotificationId.ToString() == notificationId.ToString()));

                if (circular != null)
                {
                    return Json(new { isSuccess = true, data = circular });
                }
            }
            
            return Json(new { isSuccess = false, message = "Circular details not found." });
        }

        [HttpGet]
        public async Task<IActionResult> GetCircularData(int classId, int sectionId)
        {
            // Format: 3_classId,sectionId
            var id = $"3_{classId},{sectionId}";
            var result = await _notificationService.GetNotificationDataAsync(id);
            
            return Json(new { 
                isSuccess = result.IsSuccess, 
                message = result.Message, 
                responseData = result.ResponseData ?? new List<NotificationResponse>() 
            });
        }

        [HttpGet]
        public async Task<IActionResult> Image(string path, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(path)) return NotFound();

            var baseUrl = (_apiOptions.BaseUrl ?? string.Empty).TrimEnd('/');
            if (string.IsNullOrWhiteSpace(baseUrl)) return NotFound();

            var relativePath = path.TrimStart('/');
            // If the path is already a full URL, use it directly (but check if it's from our API)
            var remoteUrl = path.StartsWith("http") ? path : $"{baseUrl}/{relativePath}";

            using var client = new HttpClient();
            try
            {
                using var response = await client.GetAsync(remoteUrl, ct);
                if (!response.IsSuccessStatusCode) return NotFound();

                var contentType = response.Content.Headers.ContentType?.MediaType ?? "image/jpeg";
                var bytes = await response.Content.ReadAsByteArrayAsync(ct);

                return File(bytes, contentType);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpGet]
        public IActionResult Event()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetEventData(string date)
        {
            if (string.IsNullOrEmpty(date))
                return Json(new { isSuccess = false, message = "Date is required." });

            var result = await _notificationService.GetEventDataAsync(date);
            return Json(new { 
                isSuccess = result.IsSuccess, 
                message = result.Message, 
                responseData = result.ResponseData ?? new List<EventResponse>() 
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddOrUpdateEvent([FromBody] EventAddRequest model)
        {
            if (model == null)
                return Json(new { isSuccess = false, message = "Invalid request data." });

            var result = await _notificationService.AddOrUpdateEventAsync(model);
            return Json(new { isSuccess = result.IsSuccess, message = result.Message });
        }

        public IActionResult Index()
        {
            return RedirectToAction("Index", "Dashboard");
        }
    }
}
