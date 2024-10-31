using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace ApiExperiment.Models
{
    /// <summary>
    /// Represents a request for service data with hierarchical access control.
    /// </summary>
    public class ServiceDataRequest
    {
        /// <summary>
        /// The access level of the requester.
        /// Higher access levels can access data with the same or lower access levels.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AccessLevel RequesterAccessLevel { get; set; }

        /// <summary>
        /// The email address of the user the request is for. While yes it shows "nullable = true" you MUST include an email address thats valid (check out the /servicedata page for valid accounts
        /// </summary>
        [EmailAddress] //valid email format
        public required string UserEmail { get; set; } = string.Empty;
    }
}
 