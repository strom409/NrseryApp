namespace MVC_Project.Models.Notification
{
    public class NotificationAddUpdateRequest
    {
        public int NotificationID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Filepath { get; set; } = string.Empty;
        public string NotificationDate { get; set; } = string.Empty;
        public int ActionType { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FileExtension { get; set; } = string.Empty;
    }
}
