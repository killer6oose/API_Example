using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiExperiment.Models
{
    /// <summary>
    /// Represents the data for a user.
    /// </summary>
    public class UserData
    {
        /// <summary>
        /// The index ID of the record
        /// </summary>
        [Required]
        public string Id { get; set; }

        public UserData()
        {
            // Generate a unique ID, e.g., UD1, UD2, etc.
            Id = $"UD{Guid.NewGuid().ToString("N").Substring(0, 8)}";
        }
        /// <summary>
        /// The user's phone number.
        /// </summary>
        [Required]
        [Phone]
        public string? PhoneNum { get; set; }

        /// <summary>
        /// The user's email address.
        /// </summary>
        [Required]
        [EmailAddress]
        public string? UserEmail { get; set; }

        /// <summary>
        /// The user's full name.
        /// </summary>
        [Required]
        [StringLength(150, MinimumLength = 2)]
        public string? FullName { get; set; }

        /// <summary>
        /// The user's physical address.
        /// </summary>
        [Required]
        public string? Address { get; set; }

        /// <summary>
        /// The access level assigned to the user.
        /// </summary>
        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AccessLevel AccessLevel { get; set; }

        // Field-specific access levels
        public AccessLevel PhoneNumAccessLevel { get; set; }
        public AccessLevel UserEmailAccessLevel { get; set; }
        public AccessLevel FullNameAccessLevel { get; set; }
        public AccessLevel AddressAccessLevel { get; set; }
    }
}
