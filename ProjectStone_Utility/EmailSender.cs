using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace ProjectStone_Utility
{
  public class EmailSender : IEmailSender // Use this paired with MailJet
    {
        // Use Dependency Injection to access the api keys directly.
        private readonly IConfiguration _config;
        public MailJetSettings MailJetSettings { get; set; }

        public EmailSender(IConfiguration configuration)
        {
            _config = configuration;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Execute(email, subject, htmlMessage);
        }

        public async Task Execute(string email, string subject, string body)
        {
            // Populate MailJet property.
            MailJetSettings = _config.GetSection("MailJet").Get<MailJetSettings>();

            // API keys located in Master API Key and Sub API Key management in https://app.mailjet.com/account/
            var client = new MailjetClient(MailJetSettings.ApiKey, MailJetSettings.ApiSecret);

            var request = new MailjetRequest { Resource = Send.Resource }
                          .Property(Send.FromEmail, "wolfiemk6@protonmail.com") // In future endeavors, replace this email with domain email. Using ProtonMail for demo.
                          .Property(Send.FromName, "Bryan")
                          .Property(Send.Subject, subject)
                          .Property(Send.HtmlPart, body)
                          .Property(Send.Recipients, new JArray
                          {
                            new JObject
                            {
                                { "Email", email }
                            }
                          });
            await client.PostAsync(request);
        }
    }
}