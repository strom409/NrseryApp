namespace MVC_Project.Models.Slider
{
    public class SliderRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Filepath { get; set; } = string.Empty;
        public DateTime NotificationDate { get; set; }
        public int ActionType { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FileExtension { get; set; } = string.Empty;
    }
}
