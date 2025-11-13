using System.Text.Json.Serialization;

namespace ContentModerationApp.DTOs
{
    public class CohereContent
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}
