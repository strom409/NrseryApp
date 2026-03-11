using System.Text.Json.Serialization;

namespace MVC_Project.Models.Notification
{
    public class NotificationResponse
    {
        [JsonPropertyName("notificationID")]
        public int NotificationId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("filepath")]
        public string FilePath { get; set; } = string.Empty;

        [JsonPropertyName("notificationDate")]
        public string NotificationDate { get; set; } = string.Empty;

        [JsonPropertyName("categoryID")]
        public int CategoryID { get; set; }

        [JsonPropertyName("userTypeID")]
        public int UserTypeID { get; set; }
    }
}