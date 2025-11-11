namespace ContentModerationApp.Models
{
    public class ModerationResult
    {
        public int Id { get; set; }
        
        public int ContentItemId { get; set; }
        public ContentItem ContentItem { get; set; }

        public bool IsFlagged { get; set; }

        public string ModerationSummary { get; set; } = string.Empty;

        public string FlagReasons { get; set; } = string.Empty;

    }
}
