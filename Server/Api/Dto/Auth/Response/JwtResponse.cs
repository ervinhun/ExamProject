namespace Api.Dto.Auth.Response;

public record JwtResponse(string Token)
{
    public string Token { get; set; } = Token;
}