using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Infrastructure.Email
{
    public class EmailSender
    {
        private readonly IConfiguration _config;
        public EmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmalAsync(string userEmail, string emailSubject, string msg)
        {
            //spelling is important at this point
            var client = new SendGridClient(_config["SendGrid:Key"]);
            var message = new SendGridMessage
            {
               From = new EmailAddress("ecos073@hotmail.com", _config["Sendgrid:User"]),
                Subject = emailSubject,
                PlainTextContent = msg,
                HtmlContent = msg
            };
            message.AddTo(new EmailAddress(userEmail));
            message.SetClickTracking(false, false);

            await client.SendEmailAsync(message);

        }
    }
}