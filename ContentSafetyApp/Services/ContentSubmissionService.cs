using ContentModerationApp.Data;
using ContentModerationApp.Models;
using ContentModerationApp.ViewModels;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;

namespace ContentModerationApp.Services
{
    public class ContentSubmissionService : IContentSubmissionService
    {
        private readonly IContentModerationService _contentModerationService;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ContentSubmissionService(IContentModerationService contentModerationService
            ,ApplicationDbContext context
            ,IWebHostEnvironment environment)
        {
            _contentModerationService = contentModerationService;
            _context = context;
            _environment = environment;
        }

        public async Task<ContentSubmission> CreateAndModerateSubmissionAsync(ContentSubmissionViewModel model, string UserId)
        {
            string imagePath = null;
            if (model.ImageFile != null)
            {
                imagePath = await SaveImage(model.ImageFile);
            }

            var submission = new ContentSubmission
            {
                SubmittedAt = DateTime.UtcNow,
                UserId = UserId,
                Items = new List<ContentItem>()
            };

            TextContentItem textItem = null;
            ImageContentItem imageItem = null;
            Task<ModerationResult> textTask = null;
            Task<ModerationResult> imageTask = null;
            if (!string.IsNullOrEmpty(model.TextContent))
            {
                textItem = new TextContentItem
                {
                    Text = model.TextContent,
                };
                textTask = _contentModerationService.AnalyzeTextAsync(textItem);

                submission.Items.Add(textItem);
            }
            if (!string.IsNullOrEmpty(imagePath))
            {
                imageItem = new ImageContentItem
                {
                    ImagePath = imagePath,
                };
                imageTask = _contentModerationService.AnalyzeImageAsync(imageItem);

                submission.Items.Add(imageItem);
            }
            var tasksToRun = new List<Task>();

            if (textTask != null)
            {
                tasksToRun.Add(textTask);
            }
            if (imageTask != null)
            {
                tasksToRun.Add(imageTask);
            }
            try
            {
                await Task.WhenAll(tasksToRun);
                if (textTask != null)
                {
                    textItem.ModerationResult = textTask.Result;
                }
                if (imageTask != null)
                {
                    imageItem.ModerationResult = imageTask.Result;
                }

                foreach (var item in submission.Items)
                {
                    if (item.ModerationResult != null && item.ModerationResult.IsFlagged)
                    {
                        submission.IsFlagged = true;
                        break;
                    }
                }
                _context.ContentSubmissions.Add(submission);
                await _context.SaveChangesAsync();

                return submission;
            }
            catch (Exception) 
            {

                if (!string.IsNullOrEmpty(imagePath))
                {
                    var fullPath = Path.Combine(_environment.WebRootPath, imagePath.TrimStart('/'));
                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                    }
                }
                throw;
            }
        }

        public async Task<ContentSubmission> GetSubmissionByIdAsync(int id, string userId, bool isAdmin)
        {
            var submission = await _context.ContentSubmissions
                .Include(s => s.User)
                .Include(s => s.Items)
                    .ThenInclude(item => item.ModerationResult)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (submission == null)
            {
                return null; // Controller will handle NotFound
            }

            // This is business logic: check if the user is allowed to see this.
            if (submission.UserId != userId && !isAdmin)
            {
                // Throw an exception that the controller can catch as "Forbid"
                throw new UnauthorizedAccessException("User is not authorized to view this submission.");
            }

            return submission;
        }

        public async Task<IEnumerable<SubmissionSummaryViewModel>> GetSubmissionsForUserAsync(string userId)
        {
            var mySubmissions = await _context.ContentSubmissions
                .Include(s => s.Items)
                    .ThenInclude(i => i.ModerationResult)
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.SubmittedAt)
                .ToListAsync();

            // Move the projection logic here
            var vm = mySubmissions.Select(s => new SubmissionSummaryViewModel
            {
                Id = s.Id,
                SubmittedAt = s.SubmittedAt,

                // --- FIX 1: Use the final, overridden status ---
                IsFlagged = s.AdminOverrideFlag.HasValue ? s.AdminOverrideFlag.Value : s.IsFlagged,

                // --- FIX 2: Correct property names ---
                TextContent = s.Items.OfType<TextContentItem>().FirstOrDefault()?.Text,
                ImagePath = s.Items.OfType<ImageContentItem>().FirstOrDefault()?.ImagePath,

                // --- FIX 3: Combine all summaries ---
                ModerationSummary = string.Join("; ", s.Items
                    .Select(i => i.ModerationResult?.ModerationSummary)
                    .Where(ms => !string.IsNullOrEmpty(ms)))
            });

            return vm;
        }
        private async Task<string> SaveImage(IFormFile image)
        {
            string imagePath = null;
            string imageFullPath = null;

            if (image != null && image.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                var uploadPath = Path.Combine(_environment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadPath);
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                imagePath = "/uploads/" + fileName;
                imageFullPath = filePath;
            }
            return imagePath;
        }
    }
}
