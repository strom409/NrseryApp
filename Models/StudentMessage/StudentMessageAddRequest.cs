using System.Text.Json.Serialization;

namespace MVC_Project.Models.StudentMessage
{
    public class StudentMessageAddRequest
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("addedBy")]
        public string AddedBy { get; set; } = string.Empty;

        [JsonPropertyName("addedOn")]
        public string AddedOn { get; set; } = string.Empty;

        [JsonPropertyName("rollNo")]
        public string RollNo { get; set; } = "0";

        // Phase 2 Fields (Optional for Whole School)
        [JsonPropertyName("studentID")]
        public long? StudentID { get; set; }

        [JsonPropertyName("classID")]
        public int? ClassID { get; set; }

        [JsonPropertyName("sectionID")]
        public int? SectionID { get; set; }

        [JsonPropertyName("studentName")]
        public string? StudentName { get; set; }

        [JsonPropertyName("className")]
        public string? ClassName { get; set; }

        [JsonPropertyName("sectionName")]
        public string? SectionName { get; set; }
    }
}
