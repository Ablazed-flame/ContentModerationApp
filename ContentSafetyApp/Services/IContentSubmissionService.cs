using ContentModerationApp.Models;
using ContentModerationApp.ViewModels;

namespace ContentModerationApp.Services
{
    public interface IContentSubmissionService
    {

        Task<ContentSubmission> CreateAndModerateSubmissionAsync(ContentSubmissionViewModel model, string UserId);
       
    }
}
