using System.ComponentModel.DataAnnotations;

namespace KeycloakTesting.Models
{
    public class UserData
    {
        [Required]
        [Phone]
        public string PhoneNum { get; set; }

        [Required]
        [EmailAddress]
        public string UserEmail { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string FullName { get; set; }

        [Required]
        public string Address { get; set; }

        // Add additional user-related properties with validation as needed
    }
}
