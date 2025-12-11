namespace Api.Dto.Auth.Request;


public record LoginRequestDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public record RefreshTokenRequestDto
{
    public required string UserId { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
}

public record RegisterRequestDto
{
    public required string FirstName { get; set; } = null;
    public required string LastName { get; set; } = null;
    public DateTime Dob { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}