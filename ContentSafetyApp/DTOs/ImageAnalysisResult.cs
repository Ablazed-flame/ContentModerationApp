using Azure.AI.ContentSafety;

namespace ContentModerationApp.DTOs
{
    public class ImageAnalysisResult
    {
        public Dictionary<ImageCategory, int> SeverityScores { get; set; } = new();
    }
}
