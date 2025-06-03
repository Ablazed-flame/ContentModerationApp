using Microsoft.AspNetCore.Mvc;
using ContentModerationApp.Models;
using ContentModerationApp.ViewModels;
using ContentModerationApp.Data;
using Azure;
using Azure.AI.ContentSafety;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ContentModerationApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ApplicationDbContext _context;
        private readonly ContentSafetyClient _safetyClient;
        private readonly IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;
        public HomeController(IWebHostEnvironment environment, ApplicationDbContext context, IConfiguration configuration,UserManager<IdentityUser> userManager)
        {
            _environment = environment;
            _context = context;
            _configuration = configuration;
            _userManager = userManager;
            var endpoint = new Uri(_configuration["AzureContentSafety:Endpoint"]);
            var apiKey = _configuration["AzureContentSafety:ApiKey"];
            var credential = new AzureKeyCredential(apiKey);
            _safetyClient = new ContentSafetyClient(endpoint, credential);
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult Submit()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Submit(ContentSubmissionViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string imagePath = null;
            string imageFullPath = null;

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(model.ImageFile.FileName);
                var uploadPath = Path.Combine(_environment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadPath);
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                imagePath = "/uploads/" + fileName;
                imageFullPath = filePath;
            }

            string reasons = "";
            bool isFlagged = false;

            // Text moderation with Azure AI Content Safety
            if (!string.IsNullOrWhiteSpace(model.TextContent))
            {
                var textOptions = new AnalyzeTextOptions(model.TextContent);
                var textResponse = await _safetyClient.AnalyzeTextAsync(textOptions);

                foreach (var blocklist in textResponse.Value.BlocklistsMatch)
                {
                    reasons += $"Blocked Term: '{blocklist.BlocklistItemText}' in category {blocklist.BlocklistName}. ";
                    isFlagged = true;
                }
                foreach (var category in textResponse.Value.CategoriesAnalysis)
                {
                    if (category.Severity > 0)
                    {
                        reasons += $"Text flagged for {category.Category} (Severity: {category.Severity}). ";
                        isFlagged = true;
                    }
                }
                Console.WriteLine(reasons);
            }

            // Image moderation with Azure AI Content Safety
            if (!string.IsNullOrEmpty(imageFullPath))
            {
                var imageBytes = await System.IO.File.ReadAllBytesAsync(imageFullPath);
                var imageData = new ContentSafetyImageData(BinaryData.FromBytes(imageBytes));
                var imageOptions = new AnalyzeImageOptions(imageData);
                var imageResponse = await _safetyClient.AnalyzeImageAsync(imageOptions);

                foreach (var category in imageResponse.Value.CategoriesAnalysis)
                {
                    if (category.Severity > 0)
                    {
                        reasons += $"Image flagged for {category.Category} (Severity: {category.Severity}). ";
                        isFlagged = true;
                    }
                }
            }

            var submission = new ContentSubmission
            {
                TextContent = model.TextContent,
                ImagePath = imagePath,
                ModerationSummary = string.IsNullOrEmpty(reasons) ? "Clean" : reasons,
                IsFlagged = isFlagged,
                FlagReasons = reasons,
                SubmittedAt = DateTime.UtcNow,
                UserId = _userManager.GetUserId(User)
            };

            _context.ContentSubmissions.Add(submission);
            await _context.SaveChangesAsync();

            return View("Result", submission);
        }

        [Authorize]
        public async Task<IActionResult> MySubmissions()
        {
            var userId = _userManager.GetUserId(User);
            var mySubmissions = await _context.ContentSubmissions
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.SubmittedAt)
                .ToListAsync();

            return View(mySubmissions);
        }

    }
}
