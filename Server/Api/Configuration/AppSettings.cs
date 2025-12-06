
namespace Api.Configuration;

public class AppSettings
{
    public required DatabaseSettings Database { get; set; }
    public required JwtSettings Jwt { get; set; }
    public required EmailSettings Email { get; set; }
    public required CorsSettings Cors { get; set; }
    public SuperSettings Super { get; set; }
}

public class DatabaseSettings
{
    public required string ConnectionString { get; set; }
}

public class JwtSettings
{
    public required string Secret { get; set; }
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public int ExpirationMinutes { get; set; } = 60;
    public int RefreshTokenDays { get; set; } = 7;
}

public class EmailSettings
{
    public string? SmtpHost { get; set; }
    public int SmtpPort { get; set; } = 587;
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? FromAddress { get; set; }
}

public class CorsSettings
{
    public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
}

public class SuperSettings
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}
