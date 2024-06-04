namespace WebAca5CodeFirst.Models
{
    /// <summary>
    /// Settings for JWT (JSON Web Token).
    /// </summary>
    public class JwtSettings
    {
        /// <summary>
        /// Gets or sets the secret key used for signing the JWT.
        /// </summary>
        public string? SecretKey { get; set; } // Secret key

        /// <summary>
        /// Gets or sets the issuer of the JWT.
        /// </summary>
        public string? Issuer { get; set; }

        /// <summary>
        /// Gets or sets the audience for the JWT.
        /// </summary>
        public string? Audience { get; set; }

        /// <summary>
        /// Gets or sets the expiration time in minutes for the JWT.
        /// </summary>
        public int ExpirationMinutes { get; set; }
    }
}
