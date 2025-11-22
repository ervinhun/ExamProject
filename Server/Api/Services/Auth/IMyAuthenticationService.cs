using Api.Dto.Auth;
using Api.Dto.Auth.Request;
using Api.Dto.Auth.Response;
using DataAccess.Entities.Auth;

namespace api.Services;

public interface IMyAuthenticationService
{
    Task<JwtResponseDto> Login(LoginRequestDto dto);
    Task<User> Register(RegisterRequestDto dto);
}