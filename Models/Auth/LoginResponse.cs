using System.Text.Json.Serialization;

namespace MVC_Project.Models.Auth
{
    public class LoginResponse
    {
        [JsonPropertyName("isSuccess")]
        public bool IsSuccess { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("data")]
        public LoginResponseData? ResponseData { get; set; }
    }

    public class LoginResponseData
    {
        [JsonPropertyName("userID")]
        public int UserId { get; set; }

        [JsonPropertyName("fullName")]
        public string? UserFullName { get; set; }

        [JsonPropertyName("email")]
        public string? UserEmail { get; set; }

        [JsonPropertyName("phone")]
        public string? UserPhoneNo { get; set; }

        [JsonPropertyName("userTypeID")]
        public int UserTypeId { get; set; }

        [JsonPropertyName("photoPath")]
        public string? PhotoPath { get; set; }

        [JsonPropertyName("session")]
        public string? Session { get; set; }

        [JsonPropertyName("sessionID")]
        public int SessionId { get; set; }

        [JsonPropertyName("dashboard")]
        public string? Dashboard { get; set; }

        [JsonPropertyName("token")]
        public string? Token { get; set; }
    }
}