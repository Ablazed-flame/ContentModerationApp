
namespace ContentModerationApp.Models
{
    public abstract class ContentItem
    {
        public int Id { get; set; }
        public int ContentSubmissionId { get; set; }
        public ContentSubmission ContentSubmission { get; set; }
        public ModerationResult ModerationResult { get; set; }
    }
}
