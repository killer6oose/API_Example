using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Users.Item.SendMail;

namespace ApiExperiment.Helpers
{
    public class GraphHelper
    {
        private readonly IConfiguration _configuration;
        private readonly GraphServiceClient _graphClient;
        private readonly bool _isGraphClientConfigured;

        public GraphHelper(IConfiguration configuration)
        {
            _configuration = configuration;

            var clientId = _configuration["AzureAd:ClientId"];
            var tenantId = _configuration["AzureAd:TenantId"];
            var clientSecret = _configuration["AzureAd:ClientSecret"];

            if (!string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(tenantId) && !string.IsNullOrEmpty(clientSecret))
            {
                var clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);
                _graphClient = new GraphServiceClient(clientSecretCredential);
                _isGraphClientConfigured = true;
            }
            else
            {
                _isGraphClientConfigured = false;
            }
        }

        public async Task SendEmailAsync(string toEmail, string subject, string content)
        {
            if (!_isGraphClientConfigured)
            {
                throw new InvalidOperationException("Graph API credentials are not configured. Please check your settings.");
            }

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
                throw new ApplicationException($"Error sending email: {ex.Message}", ex);
            }
        }

        public bool IsGraphClientConfigured => _isGraphClientConfigured;
    }
}
