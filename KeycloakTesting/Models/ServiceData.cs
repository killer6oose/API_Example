using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiExperiment.Models
{
    public class ServiceData
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string? Service { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 5)]
        public string? Address { get; set; }

        [Required]
        [RegularExpression(@"^(25[0-5]|2[0-4]\d|[01]?\d\d?)\." +
                           @"(25[0-5]|2[0-4]\d|[01]?\d\d?)\." +
                           @"(25[0-5]|2[0-4]\d|[01]?\d\d?)\." +
                           @"(25[0-5]|2[0-4]\d|[01]?\d\d?)$",
                           ErrorMessage = "Invalid IP Address format.")]
        public string? IPAddress { get; set; }

        [Required]
        [RegularExpression(@"^(25[0-5]|2[0-4]\d|[01]?\d\d?)\." +
                           @"(25[0-5]|2[0-4]\d|[01]?\d\d?)\." +
                           @"(25[0-5]|2[0-4]\d|[01]?\d\d?)\." +
                           @"(25[0-5]|2[0-4]\d|[01]?\d\d?)$",
                           ErrorMessage = "Invalid IP Gateway format.")]
        public string? IPGateway { get; set; }

        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AccessLevel AccessLevel { get; set; }
    }

}
