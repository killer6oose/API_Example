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
        /// The IP address of the user making the request.
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceDataRequest"/> class.
        /// </summary>
        /// <param name="ipAddress">The user's IP address.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="ipAddress"/> is null.</exception>
        public ServiceDataRequest(string ipAddress)
        {
            IPAddress = IPAddress ?? throw new ArgumentNullException(nameof(ipAddress));
        }
    }
}
