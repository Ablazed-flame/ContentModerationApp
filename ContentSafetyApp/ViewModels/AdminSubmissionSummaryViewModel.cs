namespace ContentModerationApp.ViewModels
{
    public class AdminSubmissionSummaryViewModel
    {
        public int Id { get; set; }
        public string UserEmail { get; set; } // Admins need to see who submitted
        public string TextContent { get; set; }
        public string ImagePath { get; set; }
        public bool IsFlagged { get; set; } // The original AI flag
        public bool? AdminOverrideFlag { get; set; } // The admin's final decision
        public string AdminOverrideNote { get; set; } // Note from admin about the override
    }
}
