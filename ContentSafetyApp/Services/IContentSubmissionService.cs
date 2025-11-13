using ContentModerationApp.Models;
using ContentModerationApp.ViewModels;

namespace ContentModerationApp.Services
{
    public interface IContentSubmissionService
    {

        Task<ContentSubmission> CreateAndModerateSubmissionAsync(ContentSubmissionViewModel model, string UserId);

        Task<ContentSubmission> GetSubmissionByIdAsync(int id, string userId, bool isAdmin);

        Task<IEnumerable<SubmissionSummaryViewModel>> GetSubmissionsForUserAsync(string userId);
    }
}
