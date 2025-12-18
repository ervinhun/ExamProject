using Api.Dto.test;
using Api.Dto.Transaction;
using Api.Services.Management;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utils.Exceptions;

namespace Api.Controllers.User;

[Authorize(Roles = "superadmin")]
[ApiController]
[Route("api/admin")]
public class AdminController(IUserManagementService userManagementService) : ControllerBase
{

    [HttpPost("register")]
    public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminDto createAdminDto)
    {
        try
        {
            await userManagementService.RegisterAdmin(createAdminDto);
            return Ok(200);
        }
        catch (ServiceException e)
        {
            return Conflict(new { message = e.Message });
        }
        
    }

    [HttpGet("list-admins")]
    public async Task<IActionResult> GetListAdmins()
    {
        try
        {
            var admins = await userManagementService.GetAllAdmins();
            return Ok(admins);
        }
        catch (ServiceException e)
        {
            return Conflict(new {message =  e.Message});
        }
    }
    
}