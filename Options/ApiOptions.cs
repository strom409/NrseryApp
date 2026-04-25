namespace MVC_Project.Options
{
    public class ApiOptions
    {
        public string BaseUrl { get; set; } = string.Empty;
        public ApiEndpoints Endpoints { get; set; } = new();
    }

    public class ApiEndpoints
    {
        public string Login { get; set; } = string.Empty;
        public string Notification { get; set; } = string.Empty;
        public string Slider { get; set; } = string.Empty;
        public string StudentMessage { get; set; } = string.Empty;
        public string StudentDetail { get; set; } = string.Empty;
    }
}