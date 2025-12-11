using System.Security.Claims;
using Api.Dto.User;
using api.Services;
using api.Services.Auth;
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
            return Conflict(new { message = e.Message });
        }
    }

    [HttpGet("all-users")]
    public async Task<ActionResult<List<UserDto>>> GetAllUsersAsync()
    {
        var user = await userManagementService.GetAllUsersAsync();
        return Ok(user);
    }

    [HttpPut("update-user/{userId:guid}")]
    public async Task<ActionResult<UserDto>> UpdateUserDetailsByIdAsync(Guid userId,
        [FromBody] UpdateUserDetailsDto updateUserDetailsDto)
    {
        return await Task.FromResult<ActionResult<UserDto>>(Ok(200));
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

    [HttpGet("get-applied-users")]
    public async Task<ActionResult<List<UserDto>>> GetAppliedUsersAsync()
    {
        var result = await userManagementService.GetAppliedUsers();
        return Ok(result);
    }

    [HttpPut("approve-user/{userId:guid}")]
    public async Task<IActionResult> ApproveUser(Guid userId, bool isApproved, bool isActive)

    {
        var adminId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (adminId == null) return BadRequest("Admin id not found");
        if (!User.Identity!.IsAuthenticated ||
            (!User.IsInRole("admin") && !User.IsInRole("superadmin")))
        {
            return Unauthorized(new { message = "User is not authorized" });
        }

        var success = await userManagementService.ConfirmMembership(userId, isApproved, isActive, new Guid(adminId));

        if (!success)
            return BadRequest("Failed to approve user");

        return Ok(new { success = true});
    }


    [Authorize]
    [HttpPost("update-password/{id:guid}")]
    public async Task<IActionResult> UpdatePasswordByIdAsync(Guid id,
        [FromBody] UpdatePasswordDto updatePasswordDto)
    {
        return await Task.FromResult(Ok(id));
    }
}