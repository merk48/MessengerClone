using System.Net;
using System.Net.Mail;
using MessengerClone.API.ConfigurationOptions;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;

namespace MessengerClone.Domain.Utils.Constants.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _settings;

        public EmailSender(IOptions<EmailSettings> cfg)
        {
            _settings = cfg.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            using (var mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(_settings.FromEmail);
                mailMessage.To.Add(email);
                mailMessage.Subject = subject;
                mailMessage.Body = htmlMessage;
                mailMessage.IsBodyHtml = true;

                using (var smtpClient = new SmtpClient(_settings.SmtpHost,_settings.SmtpPort))
                {
                    smtpClient.EnableSsl = true;
                    smtpClient.Credentials = new NetworkCredential(_settings.SmtpUser, _settings.SmtpPass);
                    await smtpClient.SendMailAsync(mailMessage);
                }
            }
        }  
        //public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        //{
        //    using (var mailMessage = new MailMessage())
        //    {
        //        mailMessage.From = new MailAddress(_configuration["EmailSettings:fromEmail"]);
        //        mailMessage.To.Add(email);
        //        mailMessage.Subject = subject;
        //        mailMessage.Body = htmlMessage;
        //        mailMessage.IsBodyHtml = true;

        //        using (var smtpClient = new SmtpClient(_configuration["EmailSettings:smtpHost"], Convert.ToInt32(_configuration["EmailSettings:smtpPort"])))
        //        {
        //            smtpClient.EnableSsl = true;
        //            smtpClient.Credentials = new NetworkCredential(_configuration["EmailSettings:smtpUser"], _configuration["EmailSettings:smtpPass"] );
        //            await smtpClient.SendMailAsync(mailMessage);
        //        }
        //    }
        //}
    }
}
