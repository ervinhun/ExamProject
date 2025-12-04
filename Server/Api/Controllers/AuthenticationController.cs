using System.Security.Claims;
using Api.Configuration;
using Api.Dto.Auth.Request;
using Api.Dto.Auth.Response;
using Api.Dto.User;
using Api.Helpers;
using api.Services;
using Api.Services.Auth;
using DataAccess.Enums;
using Microsoft.AspNetCore.Mvc;
using Utils.Exceptions;

namespace Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthenticationController(IMyAuthenticationService authenticationService, IJwt jwt, AppSettings appSettings) : ControllerBase
{
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            if (User.Identity is { IsAuthenticated: true })
            {
                var cookieOptions = CookieHelper.CreateExpiredCookieOptions();
                Response.Cookies.Append("accessToken", "", cookieOptions);
                Response.Cookies.Append("refreshToken", "", cookieOptions);
                return Ok(200);
            }
            return BadRequest("User is not authenticated");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
        {
            try
            {
                if (User.Identity is { IsAuthenticated: true })
                {
                    throw new Exception("Already logged in");
                }
                
                var result = await authenticationService.Login(loginRequestDto);
                
                var cookieOptionsAccess = CookieHelper.CreateAccessTokenCookieOptions(appSettings.Jwt.ExpirationMinutes);
                var cookieOptionsRefresh = CookieHelper.CreateRefreshTokenCookieOptions(appSettings.Jwt.RefreshTokenDays);

                Response.Cookies.Append("accessToken", result.AccessToken, cookieOptionsAccess);
                Response.Cookies.Append("refreshToken", result.RefreshToken, cookieOptionsRefresh);
                
                return Ok(new
                {
                    // Tokens are securely stored in HttpOnly cookies - no need to expose in body
                    user = result.User
                });
            }
            catch (AuthenticationException e)
            {
                return Unauthorized(new {message = e.Message});
            }
            
        }
        

        [HttpPost("register")]
        public async Task<ActionResult<JwtResponseDto>> Register(RegisterRequestDto registerRequestDto)
        {
            return NoContent();
        }
    
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if(refreshToken == null) return Unauthorized(new { message = "Refresh token not found" });
            try
            {
                var result = await jwt.RefreshTokenAsync(new RefreshTokenRequestDto
                {
                    RefreshToken = refreshToken,
                    UserId = request.UserId
                });

                var cookieOptionsAccess = CookieHelper.CreateAccessTokenCookieOptions(appSettings.Jwt.ExpirationMinutes);
                var cookieOptionsRefresh = CookieHelper.CreateRefreshTokenCookieOptions(appSettings.Jwt.RefreshTokenDays);

                Response.Cookies.Append("accessToken", result.AccessToken, cookieOptionsAccess);
                Response.Cookies.Append("refreshToken", result.RefreshToken, cookieOptionsRefresh);
    
                return Ok(new { user = result.User });
            }
            catch
            {
                return BadRequest(new { message = "Refresh token is invalid" });
            }
        }

        [HttpGet("profile")]
        public IActionResult Profile()
        {
            if (User.Identity is not { IsAuthenticated: true })
            {
                return Unauthorized(new { message = "User is not authenticated" });
            }

            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                           ?? User.FindFirst("sub")?.Value;
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                var firstName = User.FindFirst(ClaimTypes.Name)?.Value;
                var lastName = User.FindFirst(ClaimTypes.Surname)?.Value;
                
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest(new { message = "User ID claim not found in token" });
                }
                
                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest(new { message = "Email claim not found in token" });
                }
                
                var roles = User.FindAll(ClaimTypes.Role)
                    .Select(c => Enum.Parse<UserRole>(c.Value, ignoreCase: true))
                    .ToList();
                
                return Ok(new UserDto
                {
                    Id = Guid.Parse(userId),
                    FirstName = firstName ?? "",
                    LastName = lastName ?? "",
                    Email = email,
                    Roles = roles
                });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = $"Profile error: {e.Message}" });
            }
        }
}