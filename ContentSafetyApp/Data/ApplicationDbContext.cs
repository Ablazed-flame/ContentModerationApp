using Microsoft.EntityFrameworkCore;
using ContentModerationApp.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace ContentModerationApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<ContentSubmission> ContentSubmissions { get; set; }
        public DbSet<ContentItem> ContentItems { get; set; }
        public DbSet<TextContentItem> TextContentItems { get; set; }
        public DbSet<ImageContentItem> ImageContentItems { get; set; }
        public DbSet<ModerationResult> ModerationResults { get; set; }
    }
}