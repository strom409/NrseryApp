using System.Text.Json.Serialization;

namespace MVC_Project.Models.Result
{
    public class ResultStatusRequest
    {
        [JsonPropertyName("classID")]
        public int ClassID { get; set; }

        [JsonPropertyName("isPublished")]
        public bool IsPublished { get; set; }
    }
}
