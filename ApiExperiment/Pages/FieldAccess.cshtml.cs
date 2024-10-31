using ApiExperiment.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net;

namespace ApiExperiment.Pages
{
    public class FieldAccessPageModel : PageModel
    {
        public List<FieldAccessModel> FieldAccessSettings { get; set; } = new List<FieldAccessModel>();
        public bool IsLocalhost => HttpContext.Connection.RemoteIpAddress.Equals(IPAddress.Loopback) ||
                                   HttpContext.Connection.RemoteIpAddress.Equals(IPAddress.IPv6Loopback);

        private readonly IConfiguration _configuration;
        private readonly string _filePath = Path.Combine("wwwroot", "FieldAccessSettings.json");

        public FieldAccessPageModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet()
        {
            FieldAccessSettings = LoadFieldAccessSettings();
        }

        private List<FieldAccessModel> LoadFieldAccessSettings()
        {
            if (System.IO.File.Exists(_filePath))
            {
                var json = System.IO.File.ReadAllText(_filePath);
                return JsonConvert.DeserializeObject<List<FieldAccessModel>>(json) ?? new List<FieldAccessModel>();
            }
            return new List<FieldAccessModel>();
        }
    }
}
