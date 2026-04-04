using MVC_Project.Models.Notification;
using MVC_Project.Services.Helper;


namespace MVC_Project.Services.Notification
{
    public interface INotificationService
    {
        Task<ApiResponse<List<NotificationResponse>>> GetNotificationDataAsync(string notificationId);
        Task<ApiResponse<object>> UpdateNotificationAsync(NotificationUpdateRequest model);
        Task<ApiResponse<List<ContactPerson>>> GetContactDataAsync(string id);
        Task<ApiResponse<object>> AddOrUpdateContactAsync(ContactUpdateRequest model);
        Task<ApiResponse<List<NotificationResponse>>> GetAllNotificationsAsync();
        Task<ApiResponse<object>> AddOrUpdateNotificationAsync(NotificationAddUpdateRequest model);
        Task<ApiResponse<object>> AddOrUpdateDownloadsAsync(DownloadUploadRequest model);
        Task<ApiResponse<object>> AddOrUpdateCircularAsync(CircularAddRequest model);
        Task<ApiResponse<List<EventResponse>>> GetEventDataAsync(string date);
        Task<ApiResponse<object>> AddOrUpdateEventAsync(EventAddRequest model);
    }
}