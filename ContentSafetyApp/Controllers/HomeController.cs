using Microsoft.AspNetCore.Mvc;
using ContentModerationApp.Models;
using ContentModerationApp.ViewModels;
using ContentModerationApp.Data;
using Azure;
using Azure.AI.ContentSafety;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ContentModerationApp.Services;

namespace ContentModerationApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<HomeController> _logger;
        private readonly IContentSubmissionService _contentSubmissionService;
        public HomeController(IWebHostEnvironment environment, ApplicationDbContext context
            ,UserManager<IdentityUser> userManager
            ,ILogger<HomeController> logger
            ,IContentSubmissionService contentSubmissionService)
        {
            _environment = environment;
            _context = context; 
            _userManager = userManager;
            _contentSubmissionService = contentSubmissionService;
            _logger = logger;
         
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

            var submission = await _contentSubmissionService.CreateAndModerateSubmissionAsync(model, _userManager.GetUserId(User));
        
            return View("Result", submission);
        }

        [Authorize]
        public async Task<IActionResult> MySubmissions()
        {
            var userId = _userManager.GetUserId(User);
            var mySubmissions = await _context.ContentSubmissions
                .Include(s => s.Items)
                .ThenInclude(i => i.ModerationResult)
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.SubmittedAt)
                .ToListAsync();

            var vm = mySubmissions.Select(s=> new SubmissionSummaryViewModel
            {
                Id = s.Id,
                SubmittedAt = s.SubmittedAt,
                IsFlagged = s.IsFlagged,
                TextContent = s.Items.OfType<TextContentItem>().FirstOrDefault()?.Text,
                ImagePath = s.Items.OfType<ImageContentItem>().FirstOrDefault()?.ImagePath,
                ModerationSummary = string.Join("; ", s.Items.Select(i => i.ModerationResult?.ModerationSummary).Where(ms => !string.IsNullOrEmpty(ms)))
            });
            return View(vm);
        }



    }
}
