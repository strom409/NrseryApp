using System.Text.Json.Serialization;

namespace MVC_Project.Models.StudentMessage
{
    public class StudentDetailDto
    {
        [JsonPropertyName("studentID")]
        public string StudentID { get; set; } = string.Empty;

        [JsonPropertyName("studentName")]
        public string StudentName { get; set; } = string.Empty;

        [JsonPropertyName("admissionNo")]
        public string AdmissionNo { get; set; } = string.Empty;

        [JsonPropertyName("classID")]
        public string ClassID { get; set; } = string.Empty;

        [JsonPropertyName("sectionID")]
        public string SectionID { get; set; } = string.Empty;

        [JsonPropertyName("className")]
        public string ClassName { get; set; } = string.Empty;

        [JsonPropertyName("sectionName")]
        public string SectionName { get; set; } = string.Empty;

        [JsonPropertyName("rollNo")]
        public string RollNo { get; set; } = string.Empty;

        [JsonPropertyName("photoPath")]
        public string PhotoPath { get; set; } = string.Empty;

        [JsonPropertyName("fatherName")]
        public string FatherName { get; set; } = string.Empty;
    }
}
