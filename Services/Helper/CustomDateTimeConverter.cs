using System.Text.Json;
using System.Text.Json.Serialization;

namespace MVC_Project.Services.Variety
{
    public class CustomDateTimeConverter : JsonConverter<DateTime?>
    {
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                var dateString = reader.GetString();
                if (string.IsNullOrWhiteSpace(dateString))
                {
                    return null;
                }

                if (DateTime.TryParse(dateString, out var date))
                {
                    return date;
                }
            }

            return null; // Handle unexpected formats gracefully by returning null
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                writer.WriteStringValue(value.Value.ToString("yyyy-MM-ddTHH:mm:ss"));
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}
