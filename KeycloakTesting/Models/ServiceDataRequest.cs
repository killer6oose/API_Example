using System.Text.Json.Serialization;

namespace KeycloakTesting.Models
{
    public class ServiceDataRequest
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AccessLevel RequesterAccessLevel { get; set; }
        public string UserEmail { get; set; }
    }
}
