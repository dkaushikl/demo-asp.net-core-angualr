namespace Demo.API.Utility
{
    using System;
    using System.Net;
    using System.Net.Mail;
    using System.Threading.Tasks;

    using Demo.API.Models;

    using Microsoft.Extensions.Options;

    public class EmailSender : IEmailSender
    {
        private readonly EmailConfig _ec;

        public EmailSender(IOptions<EmailConfig> emailConfig)
        {
            this._ec = emailConfig.Value;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            var servername = this._ec.MailServerAddress;
            var serverport = this._ec.MailServerPort;
            var username = this._ec.UserId;
            var password = this._ec.UserPassword;

            var mail = new MailMessage
                           {
                               From = new MailAddress(this._ec.FromAddress),
                               IsBodyHtml = true,
                               Subject = subject,
                               Body = message
                           };

            var listofEmails = email.Split(',');
            foreach (var emailAddr in listofEmails) mail.To.Add(new MailAddress(emailAddr));

            var smtp = new SmtpClient
                           {
                               Host = servername,
                               Port = Convert.ToInt32(serverport),
                               Credentials = new NetworkCredential(username, password),
                               EnableSsl = true
                           };
            smtp.Send(mail);

            return Task.CompletedTask;
        }
    }
}