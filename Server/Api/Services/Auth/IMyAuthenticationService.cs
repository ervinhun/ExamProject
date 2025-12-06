using Api.Dto.Auth;
using Api.Dto.Auth.Request;
using Api.Dto.Auth.Response;
using Api.Dto.User;
using DataAccess.Entities.Auth;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace api.Services;

public interface IMyAuthenticationService
{
    Task<JwtResponseDto> Login(LoginRequestDto dto);
    Task<User> Register(RegisterRequestDto dto);
    Task<bool> ResetPassword(string resetToken, ResetPasswordRequest request);
    Task<string> RequestPasswordReset(string email);
    Task<bool> RequestMembership(RequestRegistrationDto requestRegistrationDto);
    
}
    