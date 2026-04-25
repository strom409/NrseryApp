using System.Text.Json.Serialization;

namespace MVC_Project.Models.Performance
{
    public class PerformanceUpdateRequest
    {
        [JsonPropertyName("performance")]
        public string Performance { get; set; } = string.Empty;

        [JsonPropertyName("employeeCode")]
        public string EmployeeCode { get; set; } = string.Empty;

        [JsonPropertyName("edi")]
        public long Edi { get; set; } // employeeID
    }
}
