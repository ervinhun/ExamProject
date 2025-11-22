using Api.Dto.test;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.User;

[Authorize(Roles = "superadmin")]
[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{

    [HttpPost("create")]
    public async Task<ActionResult> CreateAdmin([FromBody] CreateAdminDto createAdminDto)
    {
        return await Task.FromResult(Ok());
    }
}