namespace MVC_Project.Models.Auth
{
    public class LoginViewModel
    {
        public LoginRequest LoginRequest { get; set; } = new();
        public string? ErrorMessage { get; set; }

    }
}
