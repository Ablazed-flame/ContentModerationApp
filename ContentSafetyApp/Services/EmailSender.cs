using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

public class EmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // Log to console or just skip
        Console.WriteLine($"Sending email to {email}: {subject}");
        return Task.CompletedTask;
    }
}
