namespace MVC_Project.Models.Gallery
{
    public class GalleryAddRequest
    {
        public int Gdid { get; set; }
        public int Gidfk { get; set; }
        public string? PhotoPath { get; set; } // Base64
        public string? AddedBy { get; set; }
    }
}
