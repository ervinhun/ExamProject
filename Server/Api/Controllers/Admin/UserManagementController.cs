using Api.Dto.test;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Admin;

[ApiController]
[Authorize(Roles = "Admin")]
[Authorize(Roles = "SuperAdmin")]
[Route("api/admin")]
public class UserManagementController
{
    [HttpPost("register-player")]
    public async Task CreateUserAsync([FromBody] CreateUserDto createUserDto)
    {
        
    }
    
}