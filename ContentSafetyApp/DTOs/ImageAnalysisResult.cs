namespace ContentModerationApp.DTOs
{
    public class ImageAnalysisResult
    {
        public bool IsFlagged { get; set; }
        public string Reasons { get; set; } = string.Empty;
    }
}
