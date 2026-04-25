using System.Collections.Generic;
using MVC_Project.Models;

namespace MVC_Project.Models.Result
{
    public class ResultPublishViewModel
    {
        public List<ClassSectionDto> Classes { get; set; } = new List<ClassSectionDto>();
        public ResultStatusDto? CurrentStatus { get; set; }
        public int? SelectedClassId { get; set; }
    }
}
