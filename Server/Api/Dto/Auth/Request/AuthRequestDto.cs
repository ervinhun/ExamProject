namespace Api.Dto.Auth.Request;


public record LoginRequestDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public record RefreshTokenRequestDto
{
    public Guid UserId { get; set; }
    public required string RefreshToken { get; set; }
}

public record RegisterRequestDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}