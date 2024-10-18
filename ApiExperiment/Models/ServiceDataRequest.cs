using System.Text.Json.Serialization;

namespace ApiExperiment.Models
{
    /// <summary>
    /// Represents a request for service data with access control.
    /// </summary>
    public class ServiceDataRequest
    {
        /// <summary>
        /// The access level of the requester.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AccessLevel RequesterAccessLevel { get; set; }

        /// <summary>
        /// The email address of the user making the request.
        /// </summary>
        public string UserEmail { get; set; }
    }
}
