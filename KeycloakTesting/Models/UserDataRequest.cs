using System.Text.Json.Serialization;
using KeycloakTesting.Models;

namespace KeycloakTesting.Models
{
    public class UserDataRequest
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AccessLevel RequesterAccessLevel { get; set; }
        public string UserEmail { get; set; }
    }
}
