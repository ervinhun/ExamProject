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
    
    [HttpPut("update-user/{userId:guid}")]
    public async Task<ActionResult<UserDto>> UpdateUserDetailsByIdAsync(Guid userId, [FromBody] UpdateUserDetailsDto updateUserDetailsDto)
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


    [Authorize(Roles = "superadmin,admin,player")]
    [HttpPost("update-password/{id:guid}")]
    public async Task<IActionResult> UpdatePasswordByIdAsync(Guid id,
        [FromBody] UpdatePasswordDto updatePasswordDto)
    {
        return await Task.FromResult(Ok(id));
    }
}