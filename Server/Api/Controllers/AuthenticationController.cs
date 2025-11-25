using System.Reflection.Metadata;
using Api.Dto.Auth.Request;
using Api.Dto.Auth.Response;
using api.Services;
using Api.Services.Auth;
using Microsoft.AspNetCore.Mvc;
namespace Api.Controllers.Auth;

[ApiController]
[Route("api/auth")]
public class AuthenticationController(IMyAuthenticationService authenticationService, IJwt jwt) : ControllerBase
{
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        try
        {
            var user = User.Claims;
            
            var isDev = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
                
         
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddDays(-1),
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

<<<<<<< Updated upstream

<<<<<<< Updated upstream
            Response.Cookies.Append("refreshToken", "", cookieOptions);
                
                
            return Ok();
        }
        catch (Exception e)
=======
=======
>>>>>>> Stashed changes
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            var u = User.FindFirst("sub");
            var isDev = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
                
         
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddDays(-1),
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
            return Ok(200);
        }
        
        [HttpPost("login")]
        public async Task<ActionResult<JwtResponseDto>> Login(LoginRequestDto loginRequestDto)
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginRequestDto loginRequestDto)
        {
            
            // result.RefreshToken contains the refresh token created by your service
            // result.AccessToken contains the access token
            // result.User contains user info

            try
            {
                if (User.Identity is { IsAuthenticated: true })
                {
                    throw new Exception("Already logged in");
                }
                
                var result = await authenticationService.Login(loginRequestDto);
                var isDev = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
                
         
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTimeOffset.UtcNow.AddDays(1),
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

<<<<<<< Updated upstream
<<<<<<< Updated upstream

=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
                Response.Cookies.Append("refreshToken", result.RefreshToken, cookieOptions);
                
                
                return Ok(new
                {
                    // Never return the refresh token in the response body!
                    accessToken = result.AccessToken,
                    result.User
<<<<<<< Updated upstream
<<<<<<< Updated upstream
=======
                    
>>>>>>> Stashed changes
=======
                    
>>>>>>> Stashed changes
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
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