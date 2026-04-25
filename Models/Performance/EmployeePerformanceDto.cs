using System.Text.Json;
using System.Text.Json.Serialization;

namespace MVC_Project.Models.Performance
{
    public class EmployeePerformanceDto
    {
        [JsonPropertyName("edid")]
        public long Edid { get; set; }

        [JsonPropertyName("employeeID")]
        public long EmployeeID { get; set; }

        [JsonPropertyName("employeeName")]
        public string EmployeeName { get; set; } = string.Empty;

        [JsonPropertyName("employeeCode")]
        public JsonElement EmployeeCode { get; set; }

        [JsonPropertyName("fatherName")]
        public string FatherName { get; set; } = string.Empty;

        [JsonPropertyName("address")]
        public string Address { get; set; } = string.Empty;

        [JsonPropertyName("city")]
        public string City { get; set; } = string.Empty;

        [JsonPropertyName("phoneNo")]
        public string PhoneNo { get; set; } = string.Empty;

        [JsonPropertyName("e_Mail")]
        public string Email { get; set; } = string.Empty;

        // Additional fields from various API types
        [JsonPropertyName("performance")]
        public string Performance { get; set; } = string.Empty;

        [JsonPropertyName("edi")]
        public long Edi { get; set; }

        // Other fields from the API if needed
        [JsonPropertyName("fieldName")]
        public string FieldName { get; set; } = string.Empty;
        
        [JsonPropertyName("fieldValue")]
        public string FieldValue { get; set; } = string.Empty;
    }

    public class EmployeePerformanceResponse
    {
        [JsonPropertyName("isSuccess")]
        public bool IsSuccess { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("data")]
        public List<EmployeePerformanceDto>? Data { get; set; }
    }
}
