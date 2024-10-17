using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Graph.Models;
using Microsoft.Graph.Users.Item.SendMail;

namespace KeycloakTesting.Helpers
{
    public class GraphHelper
    {
        private readonly IConfiguration _configuration;
        private readonly GraphServiceClient _graphClient;

        public GraphHelper(IConfiguration configuration)
        {
            _configuration = configuration;

            var clientId = _configuration["AzureAd:ClientId"];
            var tenantId = _configuration["AzureAd:TenantId"];
            var clientSecret = _configuration["AzureAd:ClientSecret"];

            var clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);

            _graphClient = new GraphServiceClient(clientSecretCredential);
        }

        public async Task SendEmailAsync(string toEmail, string subject, string content)
        {
            var senderEmail = _configuration["AzureAd:SenderEmail"];

            var message = new Message
            {
                Subject = subject,
                Body = new ItemBody
                {
                    ContentType = BodyType.Text,
                    Content = content
                },
                ToRecipients = new List<Recipient>
                {
                    new Recipient
                    {
                        EmailAddress = new EmailAddress
                        {
                            Address = toEmail
                        }
                    }
                }
            };

            var sendMailBody = new SendMailPostRequestBody
            {
                Message = message,
                SaveToSentItems = true
            };

            try
            {
                await _graphClient.Users[senderEmail]
                    .SendMail
                    .PostAsync(sendMailBody);
            }
            catch (ServiceException ex)
            {
                // Log the exception or rethrow it
                throw new ApplicationException($"Error sending email: {ex.Message}", ex);
            }
        }
    }
}
