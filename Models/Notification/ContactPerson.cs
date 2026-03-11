using System.Text.Json.Serialization;

namespace MVC_Project.Models.Notification
{
    public class ContactPerson
    {
        [JsonPropertyName("cid")]
        public int Cid { get; set; }
        [JsonPropertyName("personalName")]
        public string PersonalName { get; set; } = string.Empty;

        [JsonPropertyName("designation")]
        public string Designation { get; set; } = string.Empty;

        [JsonPropertyName("phoneNo")]
        public string PhoneNo { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("filepath")]
        public string Filepath { get; set; } = string.Empty;
    }
}
