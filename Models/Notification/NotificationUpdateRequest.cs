namespace MVC_Project.Models.Notification
{
    public class NotificationUpdateRequest
    {
        public string Description { get; set; } = string.Empty;
        public string Base64Photo { get; set; } = string.Empty;
        public int Type { get; set; }
    }
}
    
