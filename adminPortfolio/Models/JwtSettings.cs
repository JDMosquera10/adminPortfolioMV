namespace adminProfolio.Models
{
    public class JwtSettings
    {
        public string accessTokenSecret { get; set; } = null!;
        public int accessTokenExpirationMinutes { get; set; }
        public string refreshTokenSecret { get; set; } = null!;
        public int refreshTokenExpirationDays { get; set; }
        public string issuer { get; set; } = null!;
        public string audience { get; set; } = null!;
    }
}
