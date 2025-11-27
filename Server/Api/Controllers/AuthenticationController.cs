using System.Reflection.Metadata;
using Api.Configuration;
using Api.Dto.Auth.Request;
using Api.Dto.Auth.Response;
using Api.Dto.User;
using api.Services;
using Api.Services.Auth;
using DataAccess.Enums;
using Microsoft.AspNetCore.Mvc;
using Utils;
using Utils.Exceptions;

namespace Api.Controllers.Auth;

[ApiController]
[Route("api/auth")]
public class AuthenticationController(IMyAuthenticationService authenticationService, IJwt jwt, AppSettings appSettings) : ControllerBase
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
                    MaxAge = TimeSpan.FromDays(appSettings.Jwt.RefreshTokenDays),
                    Path = "/"
                };

                var cookieOptionsAccess = new CookieOptions
                {
                    HttpOnly = true,
                    MaxAge = TimeSpan.FromMinutes(appSettings.Jwt.ExpirationMinutes),
                    Path = "/"
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
            var result = await authenticationService.Register(registerRequestDto);
            if (result == null)
            {
                return BadRequest("Registration failed");
            }
            return Ok(result);
        }
    
        [HttpPost("refresh-token")]
        public async Task<ActionResult<JwtResponseDto>> RefreshToken(Guid userId)
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if(refreshToken == null) throw new NullReferenceException("refreshToken is null");

            try
            {
                await jwt.RefreshTokenAsync(new RefreshTokenRequestDto
                {
                    RefreshToken =  refreshToken,
                    UserId = userId
                });
    
                return Ok(200);
            }
            catch
            {
                return BadRequest("refreshToken is invalid");
            }

        }

        [HttpGet("profile")]
        public ActionResult<UserDto> Profile()
        {
            if (User.Identity is { IsAuthenticated: false })
            {
                return Unauthorized(new { message = "User is not authenticated" });
            }

            try
            {
                var userId = User.FindFirst("sub")?.Value;
                var email = User.FindFirst("email")?.Value;
                var roles = User.FindAll("role")
                    .Select(c => Enum.Parse<UserRole>(c.Value))
                    .ToList();
                var name = User.FindFirst("name")?.Value;
                var surname = User.FindFirst("surname")?.Value;
                
                 
                return Ok(new UserDto
                {
                    Id = Guid.Parse(userId!),
                    FirstName = name!,
                    LastName = surname!,
                    Email = email!,
                    Roles = roles,
                });
            }catch(ArgumentNullException e)
            {
                return BadRequest(new { message = e.Message});
            }
 
        }
}