namespace ContentModerationApp.DTOs
{
    public class TextAnalysisResult
    {
        public bool IsFlagged { get; set; } = false;
        public string Reasons { get; set; } = string.Empty;
    }
}
