using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ApiExperiment.Models;
using System.Diagnostics;

namespace ApiExperiment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FieldAccessController : ControllerBase
    {
        private readonly string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "FieldAccessSettings.json");
        private readonly ILogger<FieldAccessController> _logger;

        public FieldAccessController(ILogger<FieldAccessController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Adds a new field access setting to the configuration.
        /// </summary>
        /// <param name="newAccess">The field access setting to add.</param>
        /// <returns>A success response upon addition.</returns>
        /// <response code="200">Field access setting added successfully.</response>
        /// <response code="500">An error occurred while adding the field access setting.</response>
        [HttpPost]
        public IActionResult AddFieldAccessSetting([FromBody] FieldAccessModel newAccess)
        {
            var settings = LoadSettings();
            newAccess.Id = settings.Any() ? settings.Max(s => s.Id) + 1 : 1; // Assign unique Id
            settings.Add(newAccess);
            SaveSettings(settings);
            return Ok();
        }

        /// <summary>
        /// Updates an existing field access setting by ID.
        /// </summary>
        /// <param name="id">The ID of the access setting to update.</param>
        /// <param name="updatedAccess">The updated access level setting.</param>
        /// <returns>A success response upon update.</returns>
        /// <response code="200">Field access setting updated successfully.</response>
        /// <response code="404">Field access setting with specified ID not found.</response>
        [HttpPut("{id}")]
        public IActionResult UpdateFieldAccessSetting(int id, [FromBody] FieldAccessModel updatedAccess)
        {
            var settings = LoadSettings();
            var accessSetting = settings.FirstOrDefault(s => s.Id == id);
            if (accessSetting == null)
            {
                return NotFound("Field access setting not found.");
            }

            accessSetting.AccessLevel = updatedAccess.AccessLevel;
            SaveSettings(settings);
            return Ok();
        }

        /// <summary>
        /// Deletes a specific field access setting by ID.
        /// </summary>
        /// <param name="id">The ID of the access setting to delete.</param>
        /// <returns>No content response upon deletion.</returns>
        /// <response code="204">Field access setting deleted successfully.</response>
        /// <response code="404">Field access setting with specified ID not found.</response>
        [HttpDelete("{id}")]
        public IActionResult DeleteFieldAccessSetting(int id)
        {
            var settings = LoadSettings();
            var accessSetting = settings.FirstOrDefault(s => s.Id == id);
            if (accessSetting == null)
            {
                return NotFound("Field access setting not found.");
            }

            settings.Remove(accessSetting);
            SaveSettings(settings);
            return NoContent();
        }

        /// <summary>
        /// Opens the file explorer at the project root (localhost only).
        /// </summary>
        /// <returns>A success response if executed on localhost; unauthorized otherwise.</returns>
        /// <response code="200">File explorer opened successfully.</response>
        /// <response code="401">Unauthorized request - action allowed only on localhost.</response>
        [HttpGet("browse-explorer")]
        public IActionResult BrowseFileExplorer()
        {
            if (Request.HttpContext.Connection.RemoteIpAddress?.ToString() == "127.0.0.1" ||
                Request.HttpContext.Connection.RemoteIpAddress?.ToString() == "::1")
            {
                Process.Start("explorer.exe", Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"));
                return Ok();
            }
            return Unauthorized("This action is only allowed on localhost.");
        }

        /// <summary>
        /// Generates or resets field access settings to default values.
        /// </summary>
        /// <returns>A success message on default generation.</returns>
        /// <response code="200">Field access settings generated or reset to defaults.</response>
        /// <response code="500">An error occurred while resetting the field access settings.</response>
        [HttpPost("generate")]
        public IActionResult GenerateDefaultFieldAccess()
        {
            try
            {
                // Define the default access settings
                var defaultAccessSettings = new List<FieldAccessModel>
                {
                    // UserData settings
                    new FieldAccessModel { Id = 1, Endpoint = "UserData", FieldName = "PhoneNum", AccessLevel = AccessLevel.Public },
                    new FieldAccessModel { Id = 2, Endpoint = "UserData", FieldName = "UserEmail", AccessLevel = AccessLevel.Confidential },
                    new FieldAccessModel { Id = 3, Endpoint = "UserData", FieldName = "FullName", AccessLevel = AccessLevel.Secret },
                    new FieldAccessModel { Id = 4, Endpoint = "UserData", FieldName = "Address", AccessLevel = AccessLevel.TopSecret },
                    // ServiceData settings
                    new FieldAccessModel { Id = 5, Endpoint = "ServiceData", FieldName = "Service", AccessLevel = AccessLevel.Public },
                    new FieldAccessModel { Id = 6, Endpoint = "ServiceData", FieldName = "Address", AccessLevel = AccessLevel.Confidential },
                    new FieldAccessModel { Id = 7, Endpoint = "ServiceData", FieldName = "IPAddress", AccessLevel = AccessLevel.Secret },
                    new FieldAccessModel { Id = 8, Endpoint = "ServiceData", FieldName = "IPGateway", AccessLevel = AccessLevel.TopSecret }
                };

                // Write the default settings to the JSON file, overwriting any existing settings
                var settingsJson = JsonConvert.SerializeObject(defaultAccessSettings, Formatting.Indented);
                System.IO.File.WriteAllText(_filePath, settingsJson);

                return Ok(new { Message = "Field access settings reset to default successfully." });
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "Failed to reset field access settings.");
                return StatusCode(500, "Error resetting field access settings.");
            }
        }

        /// <summary>
        /// Retrieves all configured field access settings.
        /// </summary>
        /// <returns>A list of field access settings including endpoint, field name, and required access level.</returns>
        /// <response code="200">Returns the list of field access settings.</response>
        [HttpGet]
        public IActionResult GetFieldAccessSettings()
        {
            var settings = LoadSettings();
            return Ok(settings);
        }

        /// <summary>
        /// Retrieves a list of available fields for the specified endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint for which available fields are requested.</param>
        /// <returns>A list of fields not yet configured for access control on the specified endpoint.</returns>
        /// <response code="200">Returns the list of available fields.</response>
        /// <response code="404">No fields available for the specified endpoint.</response>
        [HttpGet("available-fields/{endpoint}")]
        public IActionResult GetAvailableFields(string endpoint)
        {
            var allFields = GetAllFieldsForEndpoint(endpoint);
            var settings = LoadSettings();

            // Filter out fields that have already been configured
            var availableFields = allFields
                .Where(f => !settings.Any(s => s.Endpoint == endpoint && s.FieldName == f))
                .ToList();

            return availableFields.Any() ? Ok(availableFields) : NotFound("No available fields for this endpoint.");
        }

        /// <summary>
        /// Retrieves all fields associated with a specific endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint to retrieve fields for.</param>
        /// <returns>A list of fields for the specified endpoint.</returns>
        private List<string> GetAllFieldsForEndpoint(string endpoint)
        {
            // Define fields for each endpoint
            var endpointFields = new Dictionary<string, List<string>>
            {
                { "UserData", new List<string> { "PhoneNum", "UserEmail", "FullName", "Address" } },
                { "ServiceData", new List<string> { "Service", "Address", "IPAddress", "IPGateway" } }
            };
            return endpointFields.TryGetValue(endpoint, out var fields) ? fields : new List<string>();
        }

        /// <summary>
        /// Loads the current field access settings from a JSON file.
        /// </summary>
        /// <returns>A list of field access settings.</returns>
        private List<FieldAccessModel> LoadSettings()
        {
            if (!System.IO.File.Exists(_filePath))
            {
                return new List<FieldAccessModel>();
            }

            var settingsJson = System.IO.File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<List<FieldAccessModel>>(settingsJson) ?? new List<FieldAccessModel>();
        }

        /// <summary>
        /// Saves the field access settings to a JSON file.
        /// </summary>
        /// <param name="settings">A list of field access settings to save.</param>
        private void SaveSettings(List<FieldAccessModel> settings)
        {
            try
            {
                var settingsJson = JsonConvert.SerializeObject(settings, Formatting.Indented);
                System.IO.File.WriteAllText(_filePath, settingsJson);
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "Failed to save field access settings.");
            }
        }
    }
}
