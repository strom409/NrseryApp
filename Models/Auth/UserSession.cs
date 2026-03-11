namespace MVC_Project.Models.Auth
{
    public class UserSession
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? UserTypeId { get; set; }
        public string? UserTypeName { get; set; }
        public string? Token { get; set; }
        public string? PhotoPath { get; set; }
        public string? Session { get; set; }
        public int SessionId { get; set; }
        public string? Dashboard { get; set; }
    }
}