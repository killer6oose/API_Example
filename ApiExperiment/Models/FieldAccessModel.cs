using System.ComponentModel.DataAnnotations;

namespace ApiExperiment.Models
{
    public class FieldAccessModel
    {
        /// <summary>
        /// The index ID of the record
        /// </summary>
        [Required]
        public int Id { get; set; }

        [Required, MinLength(1)] // Ensures a non-null, non-empty string for model binding
        public string Endpoint { get; set; } = string.Empty;

        [Required, MinLength(1)]
        public string FieldName { get; set; } = string.Empty;

        [Required]
        public AccessLevel AccessLevel { get; set; }
    }
}
