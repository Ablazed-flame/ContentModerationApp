using ContentModerationApp.Data;
using ContentModerationApp.Models;
using ContentModerationApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

 
    public async Task<IActionResult> Index()
    {

        var viewModels = await _context.ContentSubmissions
                .Include(s => s.User) // We need this to get the User.Email
                .OrderByDescending(s => s.SubmittedAt)
                .Select(s => new AdminSubmissionSummaryViewModel // Project on the DB server
                {
                    Id = s.Id,
                    IsFlagged = s.IsFlagged,
                    AdminOverrideFlag = s.AdminOverrideFlag,

                    // Use correct property names: .Content and .ImageUrl
                    TextContent = s.Items.OfType<TextContentItem>().FirstOrDefault().Text,
                    ImagePath = s.Items.OfType<ImageContentItem>().FirstOrDefault().ImagePath
                })
                .ToListAsync();
        return View(viewModels);
    }

    // GET: Override flag page
    public async Task<IActionResult> OverrideFlag(int id)
    {
        var submissionFromDb = await _context.ContentSubmissions
                .Include(s => s.User) // Load the user
                .Include(s => s.Items) // Load ALL items
                    .ThenInclude(item => item.ModerationResult) // Load ALL their results
                .SingleOrDefaultAsync(s => s.Id == id); // Get the single item

        if (submissionFromDb == null)
        {
            return NotFound();
        }

        // Pass the FULL domain model to the view
        return View(submissionFromDb);
    }

    // POST: Save override changes
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> OverrideFlag(AdminOverrideInputViewModel model)
    {
        var submission = await _context.ContentSubmissions.FindAsync(model.Id);
        if (submission == null) return NotFound();

        // Only override if admin selected a new override flag
       
            submission.AdminOverrideFlag = model.AdminOverrideFlag;
            submission.AdminOverrideNote = model.AdminOverrideNote;
            submission.AdminOverriddenAt = DateTime.UtcNow;
 
            // Save note even if flag wasn't changed
            submission.AdminOverrideNote = model.AdminOverrideNote;
        

        _context.ContentSubmissions.Update(submission);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var submission = await _context.ContentSubmissions
                .Include(s => s.Items)
                    .ThenInclude(item => item.ModerationResult)
                .SingleOrDefaultAsync(s => s.Id == id);

        if (submission == null)
        {
            return NotFound();
        }

        _context.ContentSubmissions.Remove(submission);
        await _context.SaveChangesAsync();

        // Redirect to wherever appropriate (admin dashboard, user list, etc.)
        return RedirectToAction("Index");
    }


}
