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
    public async Task<ActionResult<JwtResponseDto>> Login(LoginRequestDto loginRequestDto)
    {
        var result = await authenticationService.Login(loginRequestDto);
        if (result == null)
        {
            return BadRequest("Username or password is incorrect");
        }
        return Ok(result);
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