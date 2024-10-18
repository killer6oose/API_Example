namespace ApiExperiment.Models
{
    /// <summary>
    /// Defines the various access levels for data security.
    /// Higher numerical values represent higher security clearance.
    /// </summary>
    public enum AccessLevel
    {
        /// <summary>
        /// Public access level. Accessible by anyone.
        /// </summary>
        Public = 1,

        /// <summary>
        /// Confidential access level. Restricted to authorized personnel.
        /// </summary>
        Confidential = 2,

        /// <summary>
        /// Secret access level. Highly restricted access.
        /// </summary>
        Secret = 3,

        /// <summary>
        /// Top Secret access level. Maximum security clearance required.
        /// </summary>
        TopSecret = 4
    }
}
