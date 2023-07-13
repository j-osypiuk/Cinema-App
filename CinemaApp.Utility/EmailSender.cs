using Microsoft.AspNetCore.Identity.UI.Services;

namespace CinemaApp.Utility
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {   
            // in future
            return Task.CompletedTask;
        }
    }
}
