using System.Text.Json.Serialization;

namespace ApiExperiment.Models
{
    public class UserDataRequest
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AccessLevel RequesterAccessLevel { get; set; }
        public string UserEmail { get; set; }
    }
}
