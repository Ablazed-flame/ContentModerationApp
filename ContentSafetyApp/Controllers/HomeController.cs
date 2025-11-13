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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(ContentSubmissionViewModel model)
        {

            if (!ModelState.IsValid) 
                return View(model);


            var submission = await _contentSubmissionService.CreateAndModerateSubmissionAsync(model, _userManager.GetUserId(User));

            return RedirectToAction("Result", new { id = submission.Id });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Result(int id)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var isAdmin = User.IsInRole("Admin");

                // --- FIX 3: Delegate to service ---
                var submission = await _contentSubmissionService.GetSubmissionByIdAsync(id, userId, isAdmin);

                if (submission == null)
                {
                    return NotFound();
                }

                return View(submission);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid(); // Or AccessDenied
            }
        }
        [Authorize]
        public async Task<IActionResult> MySubmissions()
        {
            var userId = _userManager.GetUserId(User);

            // --- FIX 4: Delegate to service ---
            var vm = await _contentSubmissionService.GetSubmissionsForUserAsync(userId);

            return View(vm);
        }



    }
}
