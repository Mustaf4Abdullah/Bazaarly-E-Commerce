using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace Bazaarly.Services
{
    public class MockEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // For now, just simulate sending an email by outputting to the console or log.
            Console.WriteLine($"Sending email to: {email}");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Message: {htmlMessage}");

            // Simulate an asynchronous operation
            return Task.CompletedTask;
        }
    }
}
