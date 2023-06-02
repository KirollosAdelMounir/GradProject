using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System;

namespace HealthCareSysAPI.Custom_Classes
{
    public class messageservice
    {
        private readonly EmailService _emailConfiguration;

        public messageservice(IOptions<EmailService> emailConfiguration)
        {
            _emailConfiguration = emailConfiguration.Value;
        }

        public void Send(string userEmail, string confirmationLink)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("HealthCareSys", _emailConfiguration.From)); // Sender email address
            message.To.Add(new MailboxAddress("", userEmail)); // Recipient email address
            message.Subject = "Confirm your email address";

            // Set the email body
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $"<h1>Confirm Your Email Address</h1><p>Click the link below to confirm your email address:</p><a href=\"{confirmationLink}\">{confirmationLink}</a>";

            message.Body = bodyBuilder.ToMessageBody();
            using var client = new SmtpClient();
            try
            {
                client.Connect(_emailConfiguration.SmtpServer, _emailConfiguration.Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_emailConfiguration.UserName, _emailConfiguration.Password);

                client.Send(message);
            }
            catch
            {
                //log an error message or throw an exception or both.
                throw;
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }
    }
}
