using Microsoft.AspNetCore.Mvc;
using KeycloakTesting.Services;
using KeycloakTesting.Models;
using System;
using System.Linq;

namespace KeycloakTesting.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserDataController : ControllerBase
    {
        private readonly JsonUserDataService _userDataService;
        private readonly ILogger<UserDataController> _logger;

        public UserDataController(JsonUserDataService userDataService, ILogger<UserDataController> logger)
        {
            _userDataService = userDataService;
            _logger = logger;
        }

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

                // Check access levels
                if (request.RequesterAccessLevel >= user.AccessLevel)
                {
                    return Ok(user);
                }
                else
                {
                    return Ok("No Access");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user data by email with access control.");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/UserData/accesslevel/{accessLevel}
        [HttpGet("accesslevel/{accessLevel}")]
        public IActionResult GetUserDataByAccessLevel(string accessLevel)
        {
            if (string.IsNullOrEmpty(accessLevel))
                return BadRequest("Access level is required.");

            try
            {
                AccessLevel accessLevelEnum = Enum.Parse<AccessLevel>(accessLevel, true);
                var data = _userDataService.GetUserDataByAccessLevel(accessLevelEnum);
                return Ok(data);
            }
            catch (ArgumentException)
            {
                return BadRequest("Invalid access level.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user data by access level");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
