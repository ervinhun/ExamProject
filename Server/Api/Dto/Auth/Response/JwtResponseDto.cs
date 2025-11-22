using DataAccess.Entities.Auth;

namespace Api.Dto.Auth.Response;

public record JwtResponseDto
{
    public required string AccessToken { get; set; } 
    public required string RefreshToken { get; set; } 
    public UserRepsonseDto user { get; set; }
}