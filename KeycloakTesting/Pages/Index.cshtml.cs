using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using KeycloakTesting.Models;
using KeycloakTesting.Services;

namespace KeycloakTesting.Pages
{
    public class IndexModel : PageModel
    {
        private readonly MappingService _mappingService;

        public IndexModel(MappingService mappingService)
        {
            _mappingService = mappingService;
        }

        public List<NumberColorMapping> Mappings { get; set; }

        [BindProperty]
        public int Id { get; set; }

        [BindProperty]
        public int Number { get; set; }

        [BindProperty]
        public string Color { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet()
        {
            Mappings = _mappingService.GetAllMappings();
            ErrorMessage = TempData["ErrorMessage"] as string;
        }

        public IActionResult OnPostUpdate()
        {
            var id = int.Parse(Request.Form["Id"]);
            var number = int.Parse(Request.Form[$"Number_{id}"]);
            var color = Request.Form[$"Color_{id}"];

            try
            {
                _mappingService.UpdateMapping(id, number, color);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToPage();
        }

        public IActionResult OnPostAdd()
        {
            try
            {
                _mappingService.AddMapping(Number, Color);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            _mappingService.DeleteMapping(id);
            return RedirectToPage();
        }
    }
}
