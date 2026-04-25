namespace MVC_Project.Models.Gallery
{
    public class GalleryItem
    {
        public int Gdid { get; set; }
        public int Gidfk { get; set; }
        public string? PhotoPath { get; set; }
        public string? Heading { get; set; }
        public string? AddedBy { get; set; }
    }
}
