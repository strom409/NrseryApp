using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace MVC_Project.Models.Topper
{
    public class TopperViewModel
    {
        public List<TopperDto> Toppers { get; set; } = new List<TopperDto>();

        // For Create/Edit
        public long Tid { get; set; }
        public string TopperName { get; set; } = string.Empty;
        public string Marks { get; set; } = string.Empty;
        public IFormFile? PhotoFile { get; set; }
        public string? ExistingPhoto { get; set; }
    }
}
