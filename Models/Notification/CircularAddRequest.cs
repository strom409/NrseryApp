using System.Text.Json.Serialization;

namespace MVC_Project.Models.Notification
{
    public class CircularAddRequest
    {
        [JsonPropertyName("cid")]
        public long NotificationID { get; set; }
        
        [JsonPropertyName("classidID")]
        public int ClassidID { get; set; }
        
        [JsonPropertyName("sectionID")]
        public int SectionID { get; set; }
        
        [JsonPropertyName("className")]
        public string ClassName { get; set; } = string.Empty;
        
        [JsonPropertyName("sectionName")]
        public string SectionName { get; set; } = string.Empty;
        
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;
        
        [JsonPropertyName("detail")]
        public string Detail { get; set; } = string.Empty;
        
        [JsonPropertyName("updatedBy")]
        public string UpdatedBy { get; set; } = string.Empty;
        
        [JsonPropertyName("filePath")]
        public string FilePath { get; set; } = string.Empty; // Base64
        
        [JsonPropertyName("fileExtension")]
        public string FileExtension { get; set; } = string.Empty;
        
        [JsonPropertyName("actionType")]
        public int ActionType { get; set; }
    }
}
