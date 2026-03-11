using System.ComponentModel.DataAnnotations;

namespace MVC_Project.Models.Auth
{
    public class LoginRequest
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public string? IpAddress { get; set; }
    }
}
