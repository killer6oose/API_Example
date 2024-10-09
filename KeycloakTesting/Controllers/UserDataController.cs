using Microsoft.AspNetCore.Mvc;
using KeycloakTesting.Services;
using KeycloakTesting.Models;

namespace KeycloakTesting.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserDataController : ControllerBase
    {
        private readonly JsonUserDataService _userDataService;

        public UserDataController(JsonUserDataService userDataService)
        {
            _userDataService = userDataService;
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
                // Log the exception
                Console.WriteLine(ex);
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
                // Log the exception
                Console.WriteLine(ex);
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
                // Log the exception
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
