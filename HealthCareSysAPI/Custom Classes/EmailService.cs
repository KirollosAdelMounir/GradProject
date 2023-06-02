
namespace HealthCareSysAPI.Custom_Classes
{
   
    public class EmailService
    {
        //public void SendConfirmationEmail(string userEmail, string confirmationLink)
        //{
        //    // Create the email message


        //    // Send the email
        //    using (var client = new SmtpClient())
        //    {
        //        client.Connect("smtp.office365.com", 587, SecureSocketOptions.StartTls); // SMTP server details
        //        client.Authenticate("healthcaresysgp@outlook.com", "11355000gG"); // Sender email and password
        //        client.Send(message);
        //        client.Disconnect(true);
        //    }
        //}

        public string From { get; set; }
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

    }
}
