using System.ComponentModel.DataAnnotations;

namespace MVC_Project.Models.Slider
{
    public class SliderViewModel
    {
        public List<SliderData> Sliders { get; set; } = new();

        [Required]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Description { get; set; } = string.Empty;

        public IFormFile? ImageFile { get; set; }
        public string? ExistingFilepath { get; set; }
        public string? OldDescription { get; set; }
    }
}
