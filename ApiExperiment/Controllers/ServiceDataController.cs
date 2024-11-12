// File: Controllers/ServiceDataController.cs
using ApiExperiment.Models;
using ApiExperiment.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ApiExperiment.Controllers
{
    /// <summary>
    /// Controller for handling service data operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceDataController : ControllerBase
    {
        private readonly JsonServiceDataService _serviceDataService;
        private readonly JsonUserDataService _userDataService;
        private readonly ILogger<ServiceDataController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceDataController"/> class.
        /// </summary>
        /// <param name="serviceDataService">The service data service.</param>
        /// <param name="userDataService">The user data service.</param>
        /// <param name="logger">The logger instance.</param>
        public ServiceDataController(
            JsonServiceDataService serviceDataService,
            JsonUserDataService userDataService,
            ILogger<ServiceDataController> logger)
        {
            _serviceDataService = serviceDataService;
            _userDataService = userDataService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all service data.
        /// </summary>
        /// <returns>A list of all service data.</returns>
        /// <response code="200">Returns the list of service data.</response>
        /// <response code="500">Internal server error.</response>
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

        /// <summary>
        /// Adds new service data.
        /// </summary>
        /// <param name="newData">The new service data to add.</param>
        /// <returns>The created service data.</returns>
        /// <response code="201">Service data created successfully.</response>
        /// <response code="400">Invalid service data provided.</response>
        /// <response code="500">Internal server error.</response>
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

        /// <summary>
        /// Updates existing service data at the specified index.
        /// </summary>
        /// <param name="id">The ID of the service data to update.</param>
        /// <param name="updatedData">The updated service data.</param>
        /// <returns>The updated service data.</returns>
        /// <response code="200">Service data updated successfully.</response>
        /// <response code="400">Invalid service data provided.</response>
        /// <response code="404">Service data not found.</response>
        /// <response code="500">Internal server error.</response>
        // PUT: api/ServiceData/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateServiceData(string id, [FromBody] ServiceData updatedData)
        {
            try
            {
                var serviceDataList = _serviceDataService.GetAllServiceData();
                var serviceData = serviceDataList.FirstOrDefault(u => u.Id == id);

                if (serviceData == null)
                {
                    return NotFound("Service data not found.");
                }

                // Copy updated fields but keep the same ID
                updatedData.Id = id;
                serviceDataList[serviceDataList.IndexOf(serviceData)] = updatedData;

                // Update the file with the modified list
                _serviceDataService.UpdateServiceData(serviceDataList);
                return Ok(updatedData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update service data.");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Deletes service data at the specified index.
        /// </summary>
        /// <param name="id">The ID of the service data to delete.</param>
        /// <response code="204">Service data deleted successfully.</response>
        /// <response code="404">Service data not found.</response>
        /// <response code="500">Internal server error.</response>
        // DELETE: api/ServiceData/{index}
        [HttpDelete("{id}")]
        public IActionResult DeleteServiceData(string id)
        {
            try
            {
                var serviceDataList = _serviceDataService.GetAllServiceData();
                var serviceData = serviceDataList.FirstOrDefault(u => u.Id == id);

                if (serviceData == null)
                {
                    return NotFound("Service data not found.");
                }

                // Remove the service data from the list
                serviceDataList.Remove(serviceData);

                // Update the file with the modified list
                _serviceDataService.UpdateServiceData(serviceDataList);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete service data.");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Retrieves combined user and service data accessible by the requester's access level using hierarchical access control.
        /// </summary>
        /// <param name="request">The request containing the requester access level and user email.</param>
        /// <returns>The combined data accessible by the specified access level.</returns>
        /// <response code="200">Returns the combined data.</response>
        /// <response code="400">Invalid request data.</response>
        /// <response code="500">Internal server error.</response>
        // POST: api/ServiceData/request
        [HttpPost("request")]
        public IActionResult GetServiceDataByAccessLevel([FromBody] ServiceDataRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.IPAddress))
            {
                return BadRequest("Invalid request data.");
            }

            try
            {
                var serviceDataList = _serviceDataService.GetAllServiceData();
                var serviceData = serviceDataList.FirstOrDefault(s => s.IPAddress.Equals(request.IPAddress, StringComparison.OrdinalIgnoreCase));

                if (serviceData == null)
                {
                    return NotFound("Service data not found.");
                }

                // Load field access settings from JSON file
                var settingsJson = System.IO.File.ReadAllText("FieldAccessSettings.json");
                var fieldAccessSettings = JsonConvert.DeserializeObject<List<FieldAccessModel>>(settingsJson);

                if (request.RequesterAccessLevel >= serviceData.AccessLevel)
                {
                    // Full access to the entire record
                    return Ok(serviceData);
                }

                // Partial access: filter fields based on field access level
                var response = new Dictionary<string, object>();

                foreach (var field in fieldAccessSettings)
                {
                    var fieldValue = serviceData.GetType().GetProperty(field.FieldName)?.GetValue(serviceData, null);
                    response[field.FieldName] = request.RequesterAccessLevel >= field.AccessLevel ? fieldValue : "No Access";
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get service data by access level.");
                return StatusCode(500, "Internal server error");
            }
        }


        /// <summary>
        /// Generates 50 new random service records.
        /// </summary>
        /// <returns>A message indicating the operation was successful.</returns>
        /// <response code="200">Records generated successfully.</response>
        /// <response code="500">Internal server error.</response>
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

    /// <summary>
    /// Represents a request to retrieve service data with hierarchical access control.
    /// </summary>
    public class ServiceDataRequest
    {
        /// <summary>
        /// The access level of the requester.
        /// Higher access levels can access data with the same or lower access levels.
        /// </summary>
        public AccessLevel RequesterAccessLevel { get; set; }

        /// <summary>
        /// The email address of the user making the request.
        /// </summary>
        public string UserEmail { get; set; }
        public string? IPAddress { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceDataRequest"/> class.
        /// </summary>
        /// <param name="userEmail">The user's email address.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="userEmail"/> is null.</exception>
        public ServiceDataRequest(string userEmail)
        {
            UserEmail = userEmail ?? throw new ArgumentNullException(nameof(userEmail));
        }
    }
}
