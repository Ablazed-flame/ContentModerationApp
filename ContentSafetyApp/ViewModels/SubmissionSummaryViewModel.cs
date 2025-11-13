namespace ContentModerationApp.ViewModels
{
    public class SubmissionSummaryViewModel
    {
        public int Id { get; set; } // For the details link
        public DateTime SubmittedAt { get; set; }
        public bool IsFlagged { get; set; } // The final, overall status

        public bool? AdminOverrideFlag { get; set; } 
        public string TextContent { get; set; }
        public string ImagePath { get; set; }
        public string ModerationSummary { get; set; }

        
    }
}
