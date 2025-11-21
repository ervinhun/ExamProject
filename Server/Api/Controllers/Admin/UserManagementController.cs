using Api.Dto.test;
using DataAccess.Entities.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Admin;

[ApiController]

[Route("api/admin")]
public class UserManagementController
{
    
    [Authorize(Roles = "SuperAdmin,Admin")]
    [HttpPost("register-player")]
    public async Task RegisterPlayerAsync([FromBody] CreateUserDto createUserDto)
    {
        
    }
    
}