// File: Controllers/UserDataController.cs
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Collections.Generic;
using ApiExperiment.Services;
using ApiExperiment.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ApiExperiment.Controllers
{
    /// <summary>
    /// Controller for handling user data operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UserDataController : ControllerBase
    {
        private readonly JsonUserDataService _userDataService;
        private readonly ILogger<UserDataController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserDataController"/> class.
        /// </summary>
        /// <param name="userDataService">The user data service.</param>
        /// <param name="logger">The logger instance.</param>
        public UserDataController(JsonUserDataService userDataService, ILogger<UserDataController> logger)
        {
            _userDataService = userDataService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all user data.
        /// </summary>
        /// <returns>A list of all user data.</returns>
        /// <response code="200">Returns the list of user data.</response>
        /// <response code="500">Internal server error.</response>
        // GET: api/UserData
        [HttpGet]
        public IActionResult GetAllUserData()
        {
            try
            {
                var data = _userDataService.GetAllUserData();
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all user data.");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Adds new user data.
        /// </summary>
        /// <param name="newData">The new user data to add.</param>
        /// <returns>The created user data.</returns>
        /// <response code="201">User data created successfully.</response>
        /// <response code="400">Invalid user data provided.</response>
        /// <response code="500">Internal server error.</response>
        // POST: api/UserData
        [HttpPost]
        public IActionResult AddUserData([FromBody] UserData newData)
        {
            if (newData == null)
            {
                return BadRequest("Invalid user data.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _userDataService.AddUserData(newData);
                return CreatedAtAction(nameof(GetAllUserData), new { }, newData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add user data.");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Updates existing user data at the specified index.
        /// </summary>
        /// <param name="index">The index of the user data to update.</param>
        /// <param name="updatedData">The updated user data.</param>
        /// <returns>The updated user data.</returns>
        /// <response code="200">User data updated successfully.</response>
        /// <response code="400">Invalid user data provided.</response>
        /// <response code="404">User data not found.</response>
        /// <response code="500">Internal server error.</response>
        // PUT: api/UserData/{index}
        [HttpPut("{index}")]
        public IActionResult UpdateUserData(int index, [FromBody] UserData updatedData)
        {
            if (updatedData == null)
            {
                return BadRequest("Invalid user data.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userDataList = _userDataService.GetAllUserData();
                if (index < 0 || index >= userDataList.Count)
                {
                    return NotFound("User data not found.");
                }

                userDataList[index] = updatedData;
                _userDataService.UpdateUserData(userDataList);
                return Ok(updatedData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update user data.");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Deletes user data at the specified index.
        /// </summary>
        /// <param name="index">The index of the user data to delete.</param>
        /// <response code="204">User data deleted successfully.</response>
        /// <response code="404">User data not found.</response>
        /// <response code="500">Internal server error.</response>
        // DELETE: api/UserData/{index}
        [HttpDelete("{index}")]
        public IActionResult DeleteUserData(int index)
        {
            try
            {
                var userDataList = _userDataService.GetAllUserData();
                if (index < 0 || index >= userDataList.Count)
                {
                    return NotFound("User data not found.");
                }

                userDataList.RemoveAt(index);
                _userDataService.UpdateUserData(userDataList);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete user data.");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Generates 50 new random user records.
        /// </summary>
        /// <returns>A message indicating the operation was successful.</returns>
        /// <response code="200">Records generated successfully.</response>
        /// <response code="500">Internal server error.</response>
        // POST: api/UserData/generate
        [HttpPost("generate")]
        public IActionResult GenerateRecords()
        {
            try
            {
                // Delete existing records
                _userDataService.UpdateUserData(new List<UserData>());

                // Generate 50 new records
                var newRecords = new List<UserData>();
                for (int i = 0; i < 50; i++)
                {
                    newRecords.Add(_userDataService.GenerateRandomUserData());
                }

                _userDataService.UpdateUserData(newRecords);
                return Ok(new { Message = "50 new user records generated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate records.");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Retrieves user data by email with hierarchical access control.
        /// </summary>
        /// <param name="request">The request containing the user email and requester access level.</param>
        /// <returns>The user data if access is permitted; otherwise, a forbidden response.</returns>
        /// <response code="200">Returns the user data.</response>
        /// <response code="400">Invalid request data.</response>
        /// <response code="403">Access is forbidden due to insufficient access level.</response>
        /// <response code="404">User not found.</response>
        /// <response code="500">Internal server error.</response>
        // POST: api/UserData/request
        [HttpPost("request")]
        public IActionResult GetUserDataByEmail([FromBody] UserDataRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.UserEmail))
            {
                return BadRequest("Invalid request data.");
            }

            try
            {
                var userDataList = _userDataService.GetAllUserData();
                var user = userDataList.FirstOrDefault(u => u.UserEmail.Equals(request.UserEmail, StringComparison.OrdinalIgnoreCase));

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                // Load field access settings from JSON file
                var settingsJson = System.IO.File.ReadAllText("FieldAccessSettings.json");
                var fieldAccessSettings = JsonConvert.DeserializeObject<List<FieldAccessModel>>(settingsJson);

                var response = new Dictionary<string, object>();

                foreach (var field in fieldAccessSettings)
                {
                    var fieldValue = user.GetType().GetProperty(field.FieldName)?.GetValue(user, null);
                    response[field.FieldName] = request.RequesterAccessLevel >= field.AccessLevel ? fieldValue : "No Access";
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user data by email with access control.");
                return StatusCode(500, "Internal server error");
            }
        }


        /// <summary>
        /// Retrieves user data filtered by access level with hierarchical access control.
        /// </summary>
        /// <param name="accessLevel">The access level of the requester (Public, Confidential, Secret, TopSecret).</param>
        /// <returns>A list of user data accessible by the specified access level, or higher.</returns>
        /// <response code="200">Returns the filtered user data.</response>
        /// <response code="400">Invalid access level provided.</response>
        /// <response code="500">Internal server error.</response>
        // GET: api/UserData/accesslevel/{accessLevel}
        [HttpGet("accesslevel/{accessLevel}")]
        public IActionResult GetUserDataByAccessLevel(string accessLevel)
        {
            if (string.IsNullOrEmpty(accessLevel))
                return BadRequest("Access level is required.");

            try
            {
                AccessLevel accessLevelEnum = Enum.Parse<AccessLevel>(accessLevel, true);

                // Retrieve all user data records and filter based on access level
                var data = _userDataService.GetAllUserData()
                    .Select(user => FilterFieldsBasedOnAccess(user, accessLevelEnum))
                    .ToList();

                return Ok(data);
            }
            catch (ArgumentException)
            {
                return BadRequest("Invalid access level.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user data by access level.");
                return StatusCode(500, "Internal server error");
            }
        }
        private object FilterFieldsBasedOnAccess(UserData user, AccessLevel requesterAccessLevel)
        {
            // Check if requester has full access to the record based on the record's access level
            if (requesterAccessLevel >= user.AccessLevel)
            {
                return user; // Full access, return the entire record
            }

            // Partial access: return only fields with access levels the requester is permitted to see
            return new
            {
                phoneNum = user.PhoneNumAccessLevel <= requesterAccessLevel ? user.PhoneNum : null,
                userEmail = user.UserEmailAccessLevel <= requesterAccessLevel ? user.UserEmail : null,
                fullName = user.FullNameAccessLevel <= requesterAccessLevel ? user.FullName : null,
                address = user.AddressAccessLevel <= requesterAccessLevel ? user.Address : null,
                accessLevel = user.AccessLevel // Always include the record's access level
            };
        }
    }
}

