using Microsoft.Extensions.Configuration;

namespace ApiExperiment.Helpers
{
    public class MailHelper
    {
        private readonly string _supportEmail;

        public MailHelper(IConfiguration configuration)
        {
            _supportEmail = configuration["Settings:SupportEmail"];
        }

        public string GetMailtoLink()
        {
            var subject = Uri.EscapeDataString("Contact Request for CronoTech API");
            var body = Uri.EscapeDataString("-------put your message below this line-------");

            return $"mailto:{_supportEmail}?subject={subject}&body={body}";
        }
    }
}
