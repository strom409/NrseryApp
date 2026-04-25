using System.Text.Json.Serialization;

namespace MVC_Project.Models.Topper
{
    public class TopperAddRequest
    {
        [JsonPropertyName("topperName")]
        public string TopperName { get; set; } = string.Empty;

        [JsonPropertyName("marks")]
        public string Marks { get; set; } = string.Empty;

        [JsonPropertyName("photo")]
        public string Photo { get; set; } = string.Empty;
    }
}
