using MVC_Project.Models.Notification;
using MVC_Project.Services.Variety;

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
    }
}