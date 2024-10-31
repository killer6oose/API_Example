using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiExperiment.Models
{
    /// <summary>
    /// Represents a request for user data with hierarchical access control.
    /// </summary>
    public class UserDataRequest
    {
        /// <summary>
        /// The access level of the requester.
        /// Higher access levels can access data with the same or lower access levels.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AccessLevel RequesterAccessLevel { get; set; }

        /// <summary>
        /// The email address of the user whose data is requested.
        /// </summary>
        [EmailAddress] //valid email format
        public required string UserEmail { get; set; } = string.Empty;
    }
}
