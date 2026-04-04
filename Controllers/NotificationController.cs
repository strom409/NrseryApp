using Microsoft.AspNetCore.Mvc;
using MVC_Project.Extensions;
using MVC_Project.Models.Notification;
using MVC_Project.Services.Notification;

namespace MVC_Project.Controllers
{
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(INotificationService notificationService, ILogger<NotificationController> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var response = new NotificationResponse
            {
                Description = TempData["Description"]?.ToString() ?? string.Empty,
                FilePath = TempData["FilePath"]?.ToString() ?? string.Empty
            };
            return View(response);
        }

        public async Task<IActionResult> GetContactData()
        {
            var result = await _notificationService.GetContactDataAsync("6_0");
            var contacts = new List<ContactPerson>();
            if (result?.IsSuccess == true && result.ResponseData != null)
                contacts = result.ResponseData;
            return View(contacts);
        }

        [HttpPost]
        public async Task<IActionResult> AddOrUpdateContact(int Cid, string PersonalName, string Designation, string PhoneNo, string Email, string Filepath, int ActionType)
        {
            var session = HttpContext.Session.GetObject<MVC_Project.Models.Auth.UserSession>(MVC_Project.Constants.SessionKeys.UserSession);
            if (session == null) return RedirectToAction("Login", "Auth");

            var model = new ContactUpdateRequest
            {
                Cid = Cid,
                PersonalName = PersonalName ?? string.Empty,
                Designation = Designation ?? string.Empty,
                PhoneNo = PhoneNo ?? string.Empty,
                Email = Email ?? string.Empty,
                Filepath = Filepath ?? string.Empty,
                ActionType = ActionType
            };

            try
            {
                var result = await _notificationService.AddOrUpdateContactAsync(model);
                if (result?.IsSuccess == true)
                    TempData["SuccessMessage"] = result.Message ?? "Contact saved successfully!";
                else
                    TempData["ErrorMessage"] = result?.Message ?? "Operation failed.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving contact");
                TempData["ErrorMessage"] = "Connection error. Please try again.";
            }

            return RedirectToAction("GetContactData");
        }

        public async Task<IActionResult> GetAllNotifications()
        {
            var result = await _notificationService.GetAllNotificationsAsync();
            var notifications = new List<NotificationResponse>();
            if (result?.IsSuccess == true && result.ResponseData != null)
                notifications = result.ResponseData;
            return View(notifications);
        }

        [HttpPost]
        public async Task<IActionResult> AddOrUpdateNotification(int NotificationID, string Title, string Description, string Filepath, string NotificationDate, int ActionType)
        {
            var session = HttpContext.Session.GetObject<MVC_Project.Models.Auth.UserSession>(MVC_Project.Constants.SessionKeys.UserSession);
            if (session == null) return RedirectToAction("Login", "Auth");

            var model = new NotificationAddUpdateRequest
            {
                NotificationID = NotificationID,
                Title = Title ?? string.Empty,
                Description = Description ?? string.Empty,
                Filepath = Filepath ?? string.Empty,
                NotificationDate = string.IsNullOrEmpty(NotificationDate)
                    ? DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                    : NotificationDate,
                ActionType = ActionType
            };

            try
            {
                var result = await _notificationService.AddOrUpdateNotificationAsync(model);
                if (result?.IsSuccess == true)
                    TempData["SuccessMessage"] = result.Message ?? "Notification saved successfully!";
                else
                    TempData["ErrorMessage"] = result?.Message ?? "Operation failed.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving notification");
                TempData["ErrorMessage"] = "Connection error. Please try again.";
            }

            return RedirectToAction("GetAllNotifications");
        }

        public async Task<IActionResult> GetNotificationData(string notificationId)
        {
            TempData["notification"] = notificationId;

            var result = await _notificationService.GetNotificationDataAsync(notificationId);

            if (result?.IsSuccess == true && result.ResponseData != null)
            {
                var item = result.ResponseData.FirstOrDefault();
                if (item != null)
                {
                    TempData["Description"] = item.Description;
                    TempData["FilePath"] = item.FilePath;
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [RequestSizeLimit(104857600)]
        [RequestFormLimits(MultipartBodyLengthLimit = 104857600, ValueLengthLimit = 104857600)]
        public async Task<IActionResult> Update(NotificationUpdateRequest model)
        {
            var session = HttpContext.Session.GetObject<MVC_Project.Models.Auth.UserSession>(MVC_Project.Constants.SessionKeys.UserSession);
            if (session == null) return RedirectToAction("Login", "Auth");

            var notification = TempData["notification"]?.ToString() ?? string.Empty;
            TempData.Keep("notification");

            switch (notification)
            {
                case "5_0": model.Type = 1; break;
                case "4_0": model.Type = 2; break;
                case "7_0": model.Type = 3; break;
                case "8_0": model.Type = 4; break;
                default: model.Type = 4; break;
            }

            try
            {
                var updateResponse = await _notificationService.UpdateNotificationAsync(model);
                if (updateResponse?.IsSuccess == true)
                    TempData["SuccessMessage"] = updateResponse.Message ?? "Updated successfully!";
                else
                    TempData["ErrorMessage"] = updateResponse?.Message ?? "Update failed.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating notification");
                TempData["ErrorMessage"] = "Connection error. Please try again.";
            }

            var currentResult = await _notificationService.GetNotificationDataAsync(notification);
            if (currentResult?.IsSuccess == true && currentResult.ResponseData != null)
            {
                var item = currentResult.ResponseData.FirstOrDefault();
                if (item != null)
                {
                    TempData["Description"] = item.Description;
                    TempData["FilePath"] = item.FilePath;
                }
            }

            return RedirectToAction("Index");
        }
    }
}
