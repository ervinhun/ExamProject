using Api.Dto.Auth;
using Api.Dto.Auth.Request;
using Api.Dto.Auth.Response;

namespace api.Services;

public interface IAuthService
{
    Task<JwtClaims> VerifyAndDecodeToken(string? token);

    Task<JwtResponse> Login(LoginRequestDto dto);
    Task<JwtResponse> Register(RegisterRequestDto dto);
}