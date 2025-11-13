using System.Text.Json.Serialization;

namespace ContentModerationApp.DTOs
{
    public class CohereChatResponse
    {
        [JsonPropertyName("message")]
        public CohereMessage Message { get; set; }
    }
}
