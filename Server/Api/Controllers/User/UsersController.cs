using Api.Dto.test;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.User;

[Authorize(Roles = "superadmin,admin")]
[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    

    [HttpGet("all")]
    public async Task<ActionResult<List<UserDto>>> GetAllUsersAsync()
    {
        return null;
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
    
    [HttpPost("register-player")]
    public async Task RegisterPlayerAsync([FromBody] CreateUserDto createUserDto)
    {
        
    }
    
    [Authorize(Roles = "superadmin,admin,player")]
    [HttpPost("update-password/{id:guid}")]
    public async Task<ActionResult> UpdatePasswordByIdAsync(Guid id,
        [FromBody] UpdatePasswordDto updatePasswordDto)
    {
        return await Task.FromResult(Ok(id));
    }

    
    
    
    
    
    
}