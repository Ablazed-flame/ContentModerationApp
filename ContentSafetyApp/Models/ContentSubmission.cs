using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ContentModerationApp.Models
{
    public class ContentSubmission
    {
        public int Id { get; set; }
        public string TextContent { get; set; }
        public string? ImagePath { get; set; }
        public string ModerationSummary { get; set; }
        public bool IsFlagged { get; set; }
        public string FlagReasons { get; set; }
        public DateTime SubmittedAt { get; set; }

        public string? UserId { get; set; }
        public IdentityUser? User { get; set; }
        // New admin override properties
        public bool? AdminOverrideFlag { get; set; } // Nullable: null means no override
        public string? AdminOverrideNote { get; set; }
        public DateTime? AdminOverriddenAt { get; set; }
    }

}
