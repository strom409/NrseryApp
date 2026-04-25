using System.Text.Json.Serialization;

namespace MVC_Project.Models.Gallery
{
    public class GalleryHeadingRequest
    {
        [JsonPropertyName("galleryHeadingName")]
        public string? GalleryHeadingName { get; set; }
    }
}
