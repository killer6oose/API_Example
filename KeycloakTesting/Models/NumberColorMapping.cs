namespace KeycloakTesting.Models
{
    public class NumberColorMapping
    {
        public int Id { get; set; }      // Unique identifier
        public int Number { get; set; }  // The number sent by Ivanti ISM
        public string Color { get; set; } // The color to respond with
    }
}
