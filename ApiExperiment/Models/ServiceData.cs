﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiExperiment.Models
{
    /// <summary>
    /// Represents the data for a service.
    /// </summary>
    public class ServiceData
    {
        /// <summary>
        /// The index ID of the record
        /// </summary>
        [Required]
        public string Id { get; set; }

        public ServiceData()
        {
            // Generate a unique ID, e.g., SD1, SD2, etc.
            Id = $"SD{Guid.NewGuid().ToString("N").Substring(0, 8)}";
        }
        /// <summary>
        /// The name of the service.
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string? Service { get; set; }

        /// <summary>
        /// The physical address where the service is located.
        /// </summary>
        [Required]
        [StringLength(200, MinimumLength = 5)]
        public string? Address { get; set; }

        /// <summary>
        /// The IP address associated with the service.
        /// </summary>
        [Required]
        [RegularExpression(@"^(25[0-5]|2[0-4]\d|[01]?\d\d?)\." +
                           @"(25[0-5]|2[0-4]\d|[01]?\d\d?)\." +
                           @"(25[0-5]|2[0-4]\d|[01]?\d\d?)\." +
                           @"(25[0-5]|2[0-4]\d|[01]?\d\d?)$",
                           ErrorMessage = "Invalid IP Address format.")]
        public string? IPAddress { get; set; }

        /// <summary>
        /// The IP gateway associated with the service.
        /// </summary>
        [Required]
        [RegularExpression(@"^(25[0-5]|2[0-4]\d|[01]?\d\d?)\." +
                           @"(25[0-5]|2[0-4]\d|[01]?\d\d?)\." +
                           @"(25[0-5]|2[0-4]\d|[01]?\d\d?)\." +
                           @"(25[0-5]|2[0-4]\d|[01]?\d\d?)$",
                           ErrorMessage = "Invalid IP Gateway format.")]
        public string? IPGateway { get; set; }

        /// <summary>
        /// The access level required to view the service data.
        /// </summary>
        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AccessLevel AccessLevel { get; set; }

            // Add field-specific access levels
            public AccessLevel ServiceAccessLevel { get; set; }
            public AccessLevel AddressAccessLevel { get; set; }
            public AccessLevel IPAddressAccessLevel { get; set; }
            public AccessLevel IPGatewayAccessLevel { get; set; }
        }
    }

