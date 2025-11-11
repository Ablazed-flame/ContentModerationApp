using Microsoft.AspNetCore.Identity;
using NuGet.ContentModel;
using System.ComponentModel.DataAnnotations;

namespace ContentModerationApp.Models
{

    public class ContentSubmission
    { 
        public int Id { get; set; }
        public DateTime SubmittedAt { get; set; }
        public bool IsFlagged { get; set; }

        public string UserId { get; set; }
        public IdentityUser? User { get; set; }
        public bool? AdminOverrideFlag { get; set; } // Nullable: null means no override
        public string? AdminOverrideNote { get; set; }
        public DateTime? AdminOverriddenAt { get; set; }

        public ICollection<ContentItem> Items { get; set; } = new List<ContentItem>();

    }
}
