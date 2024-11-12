using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ApiExperiment.Pages
{
    public class ServiceDataModel : PageModel
    {
        private readonly ILogger<ServiceDataModel> _logger;

        public ServiceDataModel(ILogger<ServiceDataModel> logger)
        {
            _logger = logger;
        }

        // Add the ErrorMessage property
        public string ErrorMessage { get; set; }

        // Existing properties and methods
        // ...

        public void OnGet()
        {
            // Example: Initialize ErrorMessage to empty
            ErrorMessage = string.Empty;

            try
            {
                // Your existing logic to load service data
                // ...

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while loading service data.");
                ErrorMessage = "An error occurred while loading service data. Please try again later.";
            }
        }

        // Similarly, handle errors in other handlers (OnPost, OnPut, etc.)
        // ...
    }
}
