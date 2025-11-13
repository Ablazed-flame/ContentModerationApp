using System.Text.Json.Serialization;

namespace ContentModerationApp.DTOs
{
    public class CohereMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("content")]
        public List<CohereContent> Content { get; set; }
    }
}
