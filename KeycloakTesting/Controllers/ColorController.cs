using Microsoft.AspNetCore.Mvc;
using KeycloakTesting.Services;

namespace KeycloakTesting.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ColorController : ControllerBase
    {
        private readonly MappingService _mappingService;

        public ColorController(MappingService mappingService)
        {
            _mappingService = mappingService;
        }

        // GET api/color/{number}
        [HttpGet("{number}")]
        public IActionResult GetColor(int number)
        {
            var color = _mappingService.GetColorByNumber(number);
            return Ok(new { Color = color });
        }
    }
}
