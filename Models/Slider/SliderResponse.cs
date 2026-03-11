namespace MVC_Project.Models.Slider
{
    public class SliderResponse
    {
        public bool IsSuccess { get; set; }
        public int Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<SliderData>? Data { get; set; }
    }

    public class SliderData
    {
        public int NotificationID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Filepath { get; set; } = string.Empty;
        public DateTime NotificationDate { get; set; }
        public int ActionType { get; set; }
        public string? Username { get; set; }
        public string? FileExtension { get; set; }
    }
}
