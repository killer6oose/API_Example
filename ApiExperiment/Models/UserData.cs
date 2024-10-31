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

        // Add access levels for each field (these could be set dynamically in the admin page)
        /// <summary>
        ///  The individual field's access level, if you have Access Level 'Public' you will be able to retrieve the phoneNum field data
        /// </summary>
        public static Dictionary<string, AccessLevel> FieldAccessLevels = new Dictionary<string, AccessLevel>
        {
            { nameof(PhoneNum), AccessLevel.Public },
            { nameof(UserEmail), AccessLevel.Confidential },
            { nameof(FullName), AccessLevel.Secret },
            { nameof(Address), AccessLevel.TopSecret }
        };
    }
}
