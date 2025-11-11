using ContentModerationApp.Models;

namespace ContentModerationApp.Services
{
    public interface IContentModerationService
    {
        Task<ModerationResult> AnalyzeTextAsync(TextContentItem textItem);
        Task<ModerationResult> AnalyzeImageAsync(ImageContentItem imageItem);
    }
}
