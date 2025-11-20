namespace DataAccess.Entities.Auth;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public byte[] PasswordHash { get; set; } =  null!;
    public byte[] PasswordSalt { get; set; } = null!;
    public string RefreshTokenHash { get; set; } = string.Empty;
    public DateTime RefreshTokenExpires { get; set; }
}