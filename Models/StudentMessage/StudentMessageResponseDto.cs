using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace MVC_Project.Models.StudentMessage
{
    public class StudentMessageResponseDto
    {
        [JsonPropertyName("smid")]
        [JsonProperty("smid")]
        public int Smid { get; set; }

        [JsonPropertyName("studentID")]
        [JsonProperty("studentID")]
        public object StudentID { get; set; } // Could be string or int in JSON

        [JsonPropertyName("classID")]
        [JsonProperty("classID")]
        public int ClassID { get; set; }

        [JsonPropertyName("sectionID")]
        [JsonProperty("sectionID")]
        public int SectionID { get; set; }

        [JsonPropertyName("title")]
        [JsonProperty("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        [JsonProperty("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("addedBy")]
        [JsonProperty("addedBy")]
        public string AddedBy { get; set; } = string.Empty;

        [JsonPropertyName("addedOn")]
        [JsonProperty("addedOn")]
        public string AddedOn { get; set; } = string.Empty;

        [JsonPropertyName("studentName")]
        [JsonProperty("studentName")]
        public string StudentName { get; set; } = string.Empty;

        [JsonPropertyName("className")]
        [JsonProperty("className")]
        public string ClassName { get; set; } = string.Empty;

        [JsonPropertyName("sectionName")]
        [JsonProperty("sectionName")]
        public string SectionName { get; set; } = string.Empty;

        [JsonPropertyName("rollNo")]
        [JsonProperty("rollNo")]
        public string RollNo { get; set; } = string.Empty;
    }
}
