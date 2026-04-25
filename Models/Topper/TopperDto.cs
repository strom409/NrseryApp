using System.Text.Json.Serialization;

namespace MVC_Project.Models.Topper
{
    public class TopperDto
    {
        [JsonPropertyName("tid")]
        public long Tid { get; set; }

        [JsonPropertyName("topperName")]
        public string TopperName { get; set; } = string.Empty;

        [JsonPropertyName("marks")]
        public string Marks { get; set; } = string.Empty;

        [JsonPropertyName("photo")]
        public string Photo { get; set; } = string.Empty;
    }

    public class TopperResponse
    {
        [JsonPropertyName("isSuccess")]
        public bool IsSuccess { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("data")]
        public List<TopperDto>? Data { get; set; }
    }
}
