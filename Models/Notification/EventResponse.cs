using System.Text.Json.Serialization;

namespace MVC_Project.Models.Notification
{
    public class EventResponse
    {
        [JsonPropertyName("eventId")]
        public long EventId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("eventDate")]
        public string EventDate { get; set; } = string.Empty;

        [JsonPropertyName("createdBy")]
        public string? CreatedBy { get; set; }

        [JsonPropertyName("actionType")]
        public int ActionType { get; set; }
    }
}
