using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace ApiExperiment.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public string PublicUrl { get; private set; }

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet()
        {
            // Load the dynamic URL from the appsettings.json file
            PublicUrl = _configuration["Settings:PublicUrl"];
        }
    }
}
