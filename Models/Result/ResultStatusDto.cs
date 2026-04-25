using System.Text.Json.Serialization;

namespace MVC_Project.Models.Result
{
    public class ResultStatusDto
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("classID")]
        public int ClassID { get; set; }

        [JsonPropertyName("isPublished")]
        public bool IsPublished { get; set; }
    }

    public class ResultStatusResponse
    {
        [JsonPropertyName("isSuccess")]
        public bool IsSuccess { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("data")]
        public ResultStatusDto? Data { get; set; }
    }
}
