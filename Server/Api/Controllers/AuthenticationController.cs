using System.Reflection.Metadata;
using Api.Dto.Auth.Request;
using Api.Dto.Auth.Response;
using api.Services;
using Api.Services.Auth;
using Microsoft.AspNetCore.Mvc;
using Utils;
using Utils.Exceptions;

namespace Api.Controllers.Auth;

[ApiController]
[Route("api/auth")]
public class AuthenticationController(IMyAuthenticationService authenticationService, IJwt jwt) : ControllerBase
{
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            var u = User.FindFirst("sub");
            var isDev = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
                
         
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                MaxAge = TimeSpan.Zero,
                Path = "/"
            };
            
            if (isDev)
            {
                // Localhost (HTTP) configuration
                cookieOptions.Secure = false;
                cookieOptions.SameSite = SameSiteMode.Lax; 
            }
            else
            {
                // Production (HTTPS) configuration
                cookieOptions.Secure = true;
                cookieOptions.SameSite = SameSiteMode.None; 
            }

            Console.Out.WriteLine(u);
            Response.Cookies.Append("refreshToken", "", cookieOptions);
            Response.Cookies.Append("accessToken", "", cookieOptions);
            return Ok(200);
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
                var isDev = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
                
         
                var cookieOptionsRefresh = new CookieOptions
                {
                    HttpOnly = true,
                    MaxAge = TimeSpan.FromDays(7),
                    Path = "/"
                };

                var cookieOptionsAccess = new CookieOptions
                {
                    HttpOnly = true,
                    MaxAge = TimeSpan.FromMinutes(60),
                    Path = "/",
                };
                
                if (isDev)
                {
                    // Localhost (HTTP) configuration
                    cookieOptionsAccess.Secure = false;
                    cookieOptionsRefresh.Secure = false;
                    cookieOptionsAccess.SameSite = SameSiteMode.Lax; 
                    cookieOptionsRefresh.SameSite = SameSiteMode.Lax; 
                }
                else
                {
                    // Production (HTTPS) configuration
                    cookieOptionsAccess.Secure = true;
                    cookieOptionsRefresh.Secure = true;
                    cookieOptionsAccess.SameSite = SameSiteMode.None;
                    cookieOptionsRefresh.SameSite = SameSiteMode.None;
                }
                

                Response.Cookies.Append("refreshToken", result.RefreshToken, cookieOptionsRefresh);
                Response.Cookies.Append("accessToken", result.AccessToken, cookieOptionsAccess);
                
                return Ok(new
                {
                    // Never return the refresh token in the response body!
                    accessToken = result.AccessToken,
                    result.User
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
            var result = await authenticationService.Register(registerRequestDto);
            if (result == null)
            {
                return BadRequest("Registration failed");
            }
            return Ok(result);
        }
    
        [HttpPost("refresh-token")]
        public async Task<ActionResult<JwtResponseDto>> RefreshToken(RefreshTokenRequestDto request)
        {
            var result = await jwt.RefreshTokenAsync(request);
        
            if (result is null || result.AccessToken is null || result.RefreshToken is null)
                return Unauthorized("Invalid refresh token.");

            return Ok(result);
        }

        [HttpGet("me")]
        public async Task<ActionResult<JwtResponseDto>> Me()
        {
            Console.Out.WriteLine(User.Identity?.Name);
            if (User.Identity is { IsAuthenticated: true })
            {
                var user = User;
                Console.Out.WriteLine(user);
            }
            return null;
        }
}