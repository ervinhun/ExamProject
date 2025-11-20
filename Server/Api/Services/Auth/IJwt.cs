using Api.Dto.Auth.Request;
using Api.Dto.Auth.Response;
using DataAccess.Entities.Auth;

namespace Api.Services.Auth;

public interface IJwt
{
    Task<JwtResponseDto> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequestDto);
    Task<JwtResponseDto> CreateTokenResponse(User user);
}