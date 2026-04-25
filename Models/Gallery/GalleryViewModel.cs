using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace MVC_Project.Models.Gallery
{
    public class GalleryViewModel
    {
        public List<GalleryHeading>? GalleryHeadings { get; set; } = new();
        public List<GalleryItem>? GalleryItems { get; set; } = new();
        public int SelectedGid { get; set; }
        public IFormFile? Photo { get; set; }
    }
}
