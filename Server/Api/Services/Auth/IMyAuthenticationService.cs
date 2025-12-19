using Api.Dto.Auth.Request;
using Api.Dto.Auth.Response;
using Api.Dto.User;
using DataAccess.Entities.Auth;
using Microsoft.AspNetCore.Identity.Data;

namespace Api.Services.Auth;

public interface IMyAuthenticationService
{
    Task<JwtResponseDto> Login(LoginRequestDto dto);
    Task<User> Register(RegisterRequestDto dto);
    Task<bool> ResetPassword(string resetToken, ResetPasswordRequest request);
    Task<string> RequestPasswordReset(string email);
    Task ChangePasswordForUserId(Guid id, ChangePasswordDto dto);
    Task<UserDto> GetProfileForId(Guid id);
    Task<bool> RequestMembership(RequestRegistrationDto requestRegistrationDto);
    
}
    