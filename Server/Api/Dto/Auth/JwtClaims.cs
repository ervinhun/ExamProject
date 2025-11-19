namespace Api.Dto.Auth;

public record JwtClaims(string Id, string Email, string Role)
{
    public string Id { get; set; } = Id;
    public string Email { get; set; } = Email;
    public string Role { get; set; } = Role;
}