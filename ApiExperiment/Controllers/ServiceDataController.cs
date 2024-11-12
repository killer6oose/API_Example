// File: Controllers/ServiceDataController.cs
using ApiExperiment.Models;
using ApiExperiment.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

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
        /// <param name="index">The index of the service data to update.</param>
        /// <param name="updatedData">The updated service data.</param>
        /// <returns>The updated service data.</returns>
        /// <response code="200">Service data updated successfully.</response>
        /// <response code="400">Invalid service data provided.</response>
        /// <response code="404">Service data not found.</response>
        /// <response code="500">Internal server error.</response>
        // PUT: api/ServiceData/{index}
        [HttpPut("{index}")]
        public IActionResult UpdateServiceData(int index, [FromBody] ServiceData updatedData)
        {
            _logger.LogInformation("Received request to update service data at index {Index}", index);

            if (updatedData == null)
            {
                _logger.LogWarning("Update failed: Service data is null");
                return BadRequest("Invalid service data.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Update failed: Model state is invalid");
                return BadRequest(ModelState);
            }

            try
            {
                _serviceDataService.UpdateExistingServiceData(index, updatedData);
                _logger.LogInformation("Service data updated successfully at index {Index}", index);
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

        /// <summary>
        /// Deletes service data at the specified index.
        /// </summary>
        /// <param name="index">The index of the service data to delete.</param>
        /// <response code="204">Service data deleted successfully.</response>
        /// <response code="404">Service data not found.</response>
        /// <response code="500">Internal server error.</response>
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
                var service = serviceDataList.FirstOrDefault(s => s.IPAddress.Equals(request.IPAddress, StringComparison.OrdinalIgnoreCase));

                if (service == null)
                {
                    return NotFound("Service data not found.");
                }

                var response = FilterFieldsBasedOnAccess(service, request.RequesterAccessLevel);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get service data by IP address with access control.");
                return StatusCode(500, "Internal server error");
            }
        }

        private object FilterFieldsBasedOnAccess(ServiceData service, AccessLevel requesterAccessLevel)
        {
            // Check if requester has full access to the record based on the record's access level
            if (requesterAccessLevel >= service.AccessLevel)
            {
                return service; // Full access, return the entire record
            }

            // Partial access: return only fields with access levels the requester is permitted to see
            return new
            {
                serviceType = service.ServiceAccessLevel <= requesterAccessLevel ? service.Service : null,
                address = service.AddressAccessLevel <= requesterAccessLevel ? service.Address : null,
                ipAddress = service.IPAddressAccessLevel <= requesterAccessLevel ? service.IPAddress : null,
                ipGateway = service.IPGatewayAccessLevel <= requesterAccessLevel ? service.IPGateway : null,
                accessLevel = service.AccessLevel // Always include the record's access level
            };
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
        public string IPAddress { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceDataRequest"/> class.
        /// </summary>
        /// <param name="ipAddress">The service's IP address.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="ipAddress"/> is null.</exception>
        public ServiceDataRequest(string ipAddress)
        {
            IPAddress = ipAddress ?? throw new ArgumentNullException(nameof(ipAddress));
        }
    }
}
