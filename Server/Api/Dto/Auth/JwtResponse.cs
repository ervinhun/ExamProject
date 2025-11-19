namespace Api.Dto.Auth;

public record JwtResponse(string Token)
{
    public string Token { get; set; } = Token;
}