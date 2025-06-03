using ContentModerationApp.Data;
using ContentModerationApp.Models;
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

    // View all submissions
    public async Task<IActionResult> Index()
    {
        var submissions = await _context.ContentSubmissions.ToListAsync();
        return View(submissions);
    }

    // GET: Override flag page
    public async Task<IActionResult> OverrideFlag(int id)
    {
        var submission = await _context.ContentSubmissions.FindAsync(id);
        if (submission == null) return NotFound();
        return View(submission);
    }

    // POST: Save override changes
    [HttpPost]
    public async Task<IActionResult> OverrideFlag(ContentSubmission model)
    {
        var submission = await _context.ContentSubmissions.FindAsync(model.Id);
        if (submission == null) return NotFound();

        // Only override if admin selected a new override flag
        if (model.AdminOverrideFlag.HasValue && model.AdminOverrideFlag != submission.IsFlagged)
        {
            submission.AdminOverrideFlag = model.AdminOverrideFlag;
            submission.IsFlagged = model.AdminOverrideFlag.Value;
            submission.AdminOverrideNote = model.AdminOverrideNote;
            submission.AdminOverriddenAt = DateTime.UtcNow;
        }
        else
        {
            // Save note even if flag wasn't changed
            submission.AdminOverrideNote = model.AdminOverrideNote;
        }

        _context.ContentSubmissions.Update(submission);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var submission = await _context.ContentSubmissions.FindAsync(id);
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
