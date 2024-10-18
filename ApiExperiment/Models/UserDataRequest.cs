using System.Text.Json.Serialization;

namespace ApiExperiment.Models
{
    /// <summary>
    /// Represents a request for user data with access control.
    /// </summary>
    public class UserDataRequest
    {
        /// <summary>
        /// The access level of the requester.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AccessLevel RequesterAccessLevel { get; set; }

        /// <summary>
        /// The email address of the user whose data is requested.
        /// </summary>
        public string UserEmail { get; set; }
    }
}
