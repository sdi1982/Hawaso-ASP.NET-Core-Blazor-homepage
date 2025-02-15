﻿using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace Hawaso.Services
{
    // ASP.NET Core Identity 인증과 권한 
    // Abstractions: Interfaces => IEmailSender
    // Implementations: Classes => EmailSender, SendGridEmailSender, ...
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Task.CompletedTask;
        }
    }
}
