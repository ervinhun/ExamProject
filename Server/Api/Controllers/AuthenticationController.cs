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

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginRequestDto loginRequestDto)
        {

            // result.RefreshToken contains the refresh token created by your service
            // result.AccessToken contains the access token
            // result.User contains user info

            try
            {
                var result = await authenticationService.Login(loginRequestDto);
                var isDev = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

                Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.None,
                    Secure = !isDev, // Secure=false in localhost, true in production
                    Expires = DateTime.UtcNow.AddDays(7),
                    Path = "/"
                });
                return Ok(new
                {
                    accessToken = result.AccessToken,
                    User = result.User
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            // Never return the refresh token in the response body!

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
    
}