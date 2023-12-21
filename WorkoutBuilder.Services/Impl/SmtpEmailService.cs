using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace WorkoutBuilder.Services.Impl
{
    public class SmtpEmailService : IEmailService
    {
        public IConfiguration Configuration { init; protected get; } = null!;

        public void Send(string toAddress, string subject, string body)
        {
            var client = new SmtpClient(Configuration["SmtpServer"], 587);
            client.Credentials = new NetworkCredential(Configuration["SmtpUsername"], Configuration["SmtpPassword"]);
            client.EnableSsl = true;

            var message = new MailMessage(Configuration["SmtpFromAddress"], toAddress, subject, body);
            client.Send(message);
        }
    }
}
