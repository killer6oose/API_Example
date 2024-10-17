using KeycloakTesting.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog.Core;

namespace KeycloakTesting.Pages
{
    public class ContactModel(IConfiguration configuration, ILogger<ContactModel> logger) : PageModel
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly GraphHelper _graphHelper = new GraphHelper(configuration);
        private readonly ILogger<ContactModel> _logger = logger;

        [BindProperty]
        public ContactFormData FormData { get; set; } = new ContactFormData();

        public int CaptchaOperand1 { get; set; }
        public int CaptchaOperand2 { get; set; }

        [TempData]
        public bool MessageSent { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
            GenerateCaptcha();
        }

        public async Task<IActionResult> OnPostSendMessage()
        {
            // Retrieve operands from TempData
            if (!int.TryParse(Request.Form["CaptchaOperand1"], out int operand1) ||
                !int.TryParse(Request.Form["CaptchaOperand2"], out int operand2))
            {
                ErrorMessage = "Captcha validation failed. Please try again.";
                GenerateCaptcha();
                return Page();
            }

            // Validate Captcha
            if (!int.TryParse(Request.Form["CaptchaAnswer"], out int userAnswer))
            {
                ModelState.AddModelError(string.Empty, "Please provide a valid answer to the math question.");
                GenerateCaptcha();
                return Page();
            }

            int correctAnswer = operand1 + operand2;

            if (userAnswer != correctAnswer)
            {
                ModelState.AddModelError(string.Empty, "Incorrect answer to the math question. Please try again.");
                GenerateCaptcha();
                return Page();
            }

            if (!ModelState.IsValid)
            {
                GenerateCaptcha();
                return Page();
            }

            // Send Email using Microsoft Graph
            try
            {
                var fromEmail = _configuration["AzureAd:SenderEmail"]; // Your email address
                var toEmail = "support@cronotech.us"; // Recipient email address
                var subject = "Contact Form Submission";
                var content = $"Name: {FormData.Name}\nEmail: {FormData.Email}\n\nQuestion:\n{FormData.Question}";

                await _graphHelper.SendEmailAsync(
                     toEmail: toEmail,
                     subject: subject,
                     content: content
                );



                MessageSent = true;
                return RedirectToPage();
            }
            catch (System.Exception ex)
            {
                ErrorMessage = "An error occurred while sending your message. Please try again later.";
                GenerateCaptcha();
                // Optionally log the exception
                _logger.LogError(ex, "Error sending contact form email.");
                return Page();
            }
        }

        private void GenerateCaptcha()
        {
            var random = new System.Random();
            CaptchaOperand1 = random.Next(1, 10);
            CaptchaOperand2 = random.Next(1, 10);

            // Store the operands in TempData
            TempData["CaptchaOperand1"] = CaptchaOperand1;
            TempData["CaptchaOperand2"] = CaptchaOperand2;
        }

    }

    public class ContactFormData
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Question { get; set; }
    }


}
