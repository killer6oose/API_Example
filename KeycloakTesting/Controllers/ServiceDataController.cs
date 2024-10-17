// File: Controllers/ServiceDataController.cs
using KeycloakTesting.Models;
using KeycloakTesting.Services;
using Microsoft.AspNetCore.Mvc;

namespace KeycloakTesting.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceDataController : ControllerBase
    {
        private readonly JsonServiceDataService _serviceDataService;
        private readonly JsonUserDataService _userDataService;
        private readonly ILogger<ServiceDataController> _logger;

        public ServiceDataController(JsonServiceDataService serviceDataService, JsonUserDataService userDataService, ILogger<ServiceDataController> logger)
        {
            _serviceDataService = serviceDataService;
            _userDataService = userDataService;
            _logger = logger;
        }

        // GET: api/ServiceData
        [HttpGet]
        public IActionResult GetAllServiceData()
        {
            try
            {
                var data = _serviceDataService.GetAllServiceData();
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all service data.");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/ServiceData
        [HttpPost]
        public IActionResult AddServiceData([FromBody] ServiceData newData)
        {
            if (newData == null)
            {
                return BadRequest("Invalid service data.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _serviceDataService.AddServiceData(newData);
                return CreatedAtAction(nameof(GetAllServiceData), new { }, newData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add service data.");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/ServiceData/{index}
        [HttpPut("{index}")]
        public IActionResult UpdateServiceData(int index, [FromBody] ServiceData updatedData)
        {
            if (updatedData == null)
            {
                return BadRequest("Invalid service data.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _serviceDataService.UpdateExistingServiceData(index, updatedData);
                return Ok(updatedData);
            }
            catch (IndexOutOfRangeException ex)
            {
                _logger.LogError(ex, "Service data index out of range.");
                return NotFound("Service data not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update service data.");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/ServiceData/{index}
        [HttpDelete("{index}")]
        public IActionResult DeleteServiceData(int index)
        {
            try
            {
                _serviceDataService.DeleteServiceData(index);
                return NoContent();
            }
            catch (IndexOutOfRangeException ex)
            {
                _logger.LogError(ex, "Service data index out of range.");
                return NotFound("Service data not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete service data.");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/ServiceData/request
        [HttpPost("request")]
        public IActionResult GetServiceDataByAccessLevel([FromBody] ServiceDataRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.UserEmail))
            {
                return BadRequest("Invalid request data.");
            }

            try
            {
                // Retrieve service data based on access level
                var serviceDataList = _serviceDataService.GetServiceDataByAccessLevel(request.RequesterAccessLevel);

                // Retrieve user data based on access level
                var userDataList = _userDataService.GetUserDataByAccessLevel(request.RequesterAccessLevel);

                // Combine both data sets
                var combinedData = new
                {
                    UserData = userDataList,
                    ServiceData = serviceDataList
                };

                return Ok(combinedData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get combined user and service data by access level.");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/ServiceData/generate
        [HttpPost("generate")]
        public IActionResult GenerateRecords()
        {
            try
            {
                // Delete existing records
                _serviceDataService.UpdateServiceData(new List<ServiceData>());

                // Generate 50 new records
                var newRecords = new List<ServiceData>();
                for (int i = 0; i < 50; i++)
                {
                    newRecords.Add(_serviceDataService.GenerateRandomServiceData());
                }

                _serviceDataService.UpdateServiceData(newRecords);
                return Ok(new { Message = "50 new service records generated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate service records.");
                return StatusCode(500, "Internal server error");
            }
        }
    }

    // Define the ServiceDataRequest model
    public class ServiceDataRequest
    {
        public AccessLevel RequesterAccessLevel { get; set; }
        public string UserEmail { get; set; }

        public ServiceDataRequest(string userEmail)
        {
            UserEmail = userEmail ?? throw new ArgumentNullException(nameof(userEmail));
        }
    }

}
