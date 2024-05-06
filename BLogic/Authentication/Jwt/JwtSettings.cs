namespace WebAca5CodeFirst.Models
{
    public class JwtSettings
    {
        public string? SecretKey { get; set; } //Chiave segreta
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public int ExpirationMinutes { get; set; }
    }
}
