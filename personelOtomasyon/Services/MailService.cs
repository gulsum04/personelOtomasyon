using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace personelOtomasyon.Services
{
    public class MailService
    {
        private readonly string _emailAddress = "chatgptproje@gmail.com";  // kendi gmail adresin
        private readonly string _appPassword = "eitj qynj ylgt grdy";  // App password

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(_emailAddress, _appPassword),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailAddress),
                Subject = subject,
                Body = body,
                IsBodyHtml = false,
            };
            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
