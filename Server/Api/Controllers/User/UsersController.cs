using System.Security.Claims;
using Api.Dto.test;
using Api.Dto.User;
using Api.Services.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.User;

[Authorize(Roles = "superadmin,admin")]
[ApiController]
[Route("api/users")]
public class UsersController(IUserManagementService userManagementService) : ControllerBase
{
    [HttpPost("create")]
    public async Task<ActionResult<UserDto>> CreateUserAsync([FromBody] CreateUserDto createUserDto)
    {
        var user = await userManagementService.RegisterUser(createUserDto);
        return Ok(user);
    }
    
    
    [HttpGet("all")]
    public async Task<ActionResult<List<UserDto>>> GetAllUsersAsync()
    {
        var user = await userManagementService.GetAllUsersAsync();
        return Ok(user);
    }

    [HttpPut("update/{userId:guid}")]
    public async Task<ActionResult<UserDto>> UpdateUserDetailsByIdAsync(Guid userId, [FromBody] UpdateUserDetailsDto updateUserDetailsDto)
    {
        return Ok(200);
    }

    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<UserDto>> GetUserByIdAsync(Guid userId)
    {
        return await Task.FromResult(Ok(200));

    }

    [HttpPut("delete/{userId:guid}")]
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