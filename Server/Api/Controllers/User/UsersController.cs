using Api.Dto.User;
using Api.Services.Management;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Utils.Exceptions;

namespace Api.Controllers.User;

[Authorize(Roles = "superadmin,admin")]
[ApiController]
[Route("api/users")]
public class UsersController(IUserManagementService userManagementService) : ControllerBase
{
    [HttpPost("register-user")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
    {
        try
        {
            await userManagementService.RegisterUser(createUserDto);
            return Ok(200);
        }
        catch (ServiceException e)
        {
            return Conflict(new { message = e.Message });
        }
    }


    [HttpGet("all")]

    [HttpPatch("toggle-status/{userId:guid}")]
    public async Task<IActionResult> ToggleStatusByIdAsync(Guid userId)
    {
        try
        {
            await userManagementService.ToggleStatus(userId);
            return Ok(200);
        }
        catch (Exception e)
        {
            return Conflict(new {message = e.Message});
        }
    }
    
    [HttpGet("all-users")]
    public async Task<ActionResult<List<UserDto>>> GetAllUsersAsync()
    {
        var user = await userManagementService.GetAllUsersAsync();
        return Ok(user);
    }

    [HttpPut("update/{userId:guid}")]
    public Task<ActionResult<UserDto>> UpdateUserDetailsByIdAsync(Guid userId,
        [FromBody] UpdateUserDetailsDto updateUserDetailsDto)
    [HttpPut("update-user/{userId:guid}")]
    public async Task<ActionResult<UserDto>> UpdateUserDetailsByIdAsync(Guid userId, [FromBody] UpdateUserDetailsDto updateUserDetailsDto)
    {
        return Task.FromResult<ActionResult<UserDto>>(Ok(200));
    }

    [HttpGet("get-user/{userId:guid}")]
    public async Task<ActionResult<UserDto>> GetUserByIdAsync(Guid userId)
    {
        return await Task.FromResult(Ok(200));
    }

    [HttpPut("delete-user/{userId:guid}")]
    public async Task DeleteUserByIdAsync(Guid userId)
    {
        await Task.FromResult(Ok(200));
    }


    [Authorize(Roles = "superadmin,admin,player")]
    [HttpPost("update-password/{id:guid}")]
    public async Task<IActionResult> UpdatePasswordByIdAsync(Guid id,
        [FromBody] UpdatePasswordDto updatePasswordDto)
    {
        return await Task.FromResult(Ok(id));
    }


    [HttpPost("reset-password")]
    public async Task<IActionResult> RequestPasswordReset([FromBody] string email)
    {
        var newPassToken = await userManagementService.RequestPasswordReset(email);

        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var message = $"To reset the password click on the link: {baseUrl}/reset-password/{newPassToken}";

        emailToUser(email, message);

        return Ok("Password reset email sent.");
    }

    [HttpPost("reset-password/{resetToken}")]
    public async Task<IActionResult> ResetPassword(
        [FromRoute] string resetToken,
        [FromBody] ResetPasswordRequest request)
    {
        var success = await userManagementService.ResetPassword(resetToken, request);

        if (!success)
            return BadRequest("Invalid or expired reset token.");

        return Ok("Password has been reset.");
    }


    private static void emailToUser(string email, string message)
    {
        // TODO: Implement the feature - https://easv365-team-bokczyi7.atlassian.net/browse/SEM-60
        throw new NotImplementedException();
    }
}