using ContentModerationApp.Data;
using ContentModerationApp.Models;
using ContentModerationApp.ViewModels;
using Microsoft.AspNetCore.Http.Features;

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

        public async Task<ContentSubmission> CreateAndModerateSubmissionAsync(ContentSubmissionViewModel model,string UserId)
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

            if(!string.IsNullOrEmpty(model.TextContent))
            {
                var textItem = new TextContentItem
                {
                    Text = model.TextContent,
                };
                var textResult = await _contentModerationService.AnalyzeTextAsync(textItem);
                textItem.ModerationResult = textResult;
                submission.Items.Add(textItem);
            }
            if (!string.IsNullOrEmpty(imagePath))
            {
                var imageItem = new ImageContentItem
                {
                    ImagePath = imagePath,
                };
                var imageResult = await _contentModerationService.AnalyzeImageAsync(imageItem);
                imageItem.ModerationResult = imageResult;
                submission.Items.Add(imageItem);
            }

            //var textResult = await _contentModerationService.AnalyzeTextAsync(textItem);
            //textItem.ModerationResult = textResult;
            foreach(var item in submission.Items)
            {
                if(item.ModerationResult != null && item.ModerationResult.IsFlagged)
                {
                    submission.IsFlagged = true;
                    break;
                }
            }
            _context.ContentSubmissions.Add(submission);
            await _context.SaveChangesAsync();

            return submission;
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
