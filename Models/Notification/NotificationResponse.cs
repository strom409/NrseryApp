using System.Text.Json.Serialization;

namespace MVC_Project.Models.Notification
{
    public class NotificationResponse
    {
        [JsonPropertyName("cid")]
        public long NotificationId { get; set; }

        [JsonPropertyName("classidID")]
        public int ClassId { get; set; }

        [JsonPropertyName("sectionID")]
        public int SectionId { get; set; }

        [JsonPropertyName("className")]
        public string? ClassName { get; set; }

        [JsonPropertyName("sectionName")]
        public string? SectionName { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("detail")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string? AltDescription { set { if (string.IsNullOrEmpty(Description)) Description = value ?? string.Empty; } }

        [JsonPropertyName("filePath")]
        public string FilePath { get; set; } = string.Empty;

        [JsonPropertyName("notificationDate")]
        public string NotificationDate { get; set; } = string.Empty;

        [JsonPropertyName("updatedBy")]
        public string? UpdatedBy { get; set; }

        [JsonPropertyName("fileExtension")]
        public string? FileExtension { get; set; }

        [JsonPropertyName("actionType")]
        public int ActionType { get; set; }
    }
}