using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
namespace HealthCareSysAPI.Custom_Classes
{
    public class EmailService
    {
        public void SendConfirmationEmail(string userEmail, string confirmationLink)
        {
            // Create the email message
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("HealthCareSys", "healthcaresysgp@outlook.com")); // Sender email address
            message.To.Add(new MailboxAddress("", userEmail)); // Recipient email address
            message.Subject = "Confirm your email address";

            // Set the email body
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $"<h1>Confirm Your Email Address</h1><p>Click the link below to confirm your email address:</p><a href=\"{confirmationLink}\">{confirmationLink}</a>";

            message.Body = bodyBuilder.ToMessageBody();

            // Send the email
            using (var client = new SmtpClient())
            {
                client.Connect("smtp.office365.com", 587, SecureSocketOptions.StartTls); // SMTP server details
                client.Authenticate("healthcaresysgp@outlook.com", "11355000gG"); // Sender email and password
                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}
