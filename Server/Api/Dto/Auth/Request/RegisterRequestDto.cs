namespace Api.Dto.Auth.Request;

public abstract record RegisterRequestDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}