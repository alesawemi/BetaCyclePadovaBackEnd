namespace WebAca5CodeFirst.Models
{
    /// <summary>
    /// Singleton class to manage database connection.
    /// </summary>
    public class DbConnectionSingleton
    {
        /// <summary>
        /// Gets or sets the SQL connection string.
        /// </summary>
        public string? SqlConnection { get; set; }
    }
}
