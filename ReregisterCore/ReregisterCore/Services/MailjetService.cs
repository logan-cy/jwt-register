using ReregisterCore.Interfaces;
using ReregisterCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ReregisterCore.Services
{
    public class MailjetService : IEmail
    {
        public async Task SendAsync(string emailTo, string body, string subject, EmailOptionsDTO options)
        {
            var client = new SmtpClient(options.Host, options.Port)
            {
                Credentials = new NetworkCredential(options.ApiKey, options.ApiKeySecret)
            };

            var message = new MailMessage(options.SenderEmail, emailTo)
            {
                Body = body,
                IsBodyHtml = true,
                Subject = subject
            };

            await client.SendMailAsync(message);
        }
    }
}
