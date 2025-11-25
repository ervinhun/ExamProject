using DataAccess.Entities.Auth;

namespace Api.Dto.Auth.Response;

public record JwtResponseDto
{
    public required string AccessToken { get; set; } 
    public string RefreshToken { get; set; } = String.Empty;
    public UserResponseDto User { get; set; }
}