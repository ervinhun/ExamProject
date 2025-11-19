using Api.Dto;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserRolesController : ControllerBase
{
    private readonly MyDbContext _context;

    public UserRolesController(MyDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserRoleDto>>> GetUserRoles()
    {
        var userRoles = await _context.UserRoles
            .Select(ur => new UserRoleDto
            {
                Id = ur.Id,
                UserId = ur.UserId,
                RoleId = ur.RoleId,
                CreatedAt = ur.CreatedAt,
                CreatedBy = ur.CreatedBy
            })
            .ToListAsync();

        return Ok(userRoles);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserRoleDto>> GetUserRole(Guid id)
    {
        var userRole = await _context.UserRoles
            .Where(ur => ur.Id == id)
            .Select(ur => new UserRoleDto
            {
                Id = ur.Id,
                UserId = ur.UserId,
                RoleId = ur.RoleId,
                CreatedAt = ur.CreatedAt,
                CreatedBy = ur.CreatedBy
            })
            .FirstOrDefaultAsync();

        if (userRole == null)
        {
            return NotFound();
        }

        return Ok(userRole);
    }

    [HttpPost]
    public async Task<ActionResult<UserRoleDto>> CreateUserRole(CreateUserRoleDto createDto)
    {
        var userRole = new UserRole
        {
            UserId = createDto.UserId,
            RoleId = createDto.RoleId,
            CreatedBy = createDto.CreatedBy,
            CreatedAt = DateTime.UtcNow
        };

        _context.UserRoles.Add(userRole);
        await _context.SaveChangesAsync();

        var userRoleDto = new UserRoleDto
        {
            Id = userRole.Id,
            UserId = userRole.UserId,
            RoleId = userRole.RoleId,
            CreatedAt = userRole.CreatedAt,
            CreatedBy = userRole.CreatedBy
        };

        return CreatedAtAction(nameof(GetUserRole), new { id = userRole.Id }, userRoleDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUserRole(Guid id, UpdateUserRoleDto updateDto)
    {
        var userRole = await _context.UserRoles.FindAsync(id);

        if (userRole == null)
        {
            return NotFound();
        }

        if (updateDto.UserId.HasValue)
        {
            userRole.UserId = updateDto.UserId.Value;
        }
        if (updateDto.RoleId.HasValue)
        {
            userRole.RoleId = updateDto.RoleId.Value;
        }
        if (updateDto.CreatedBy.HasValue)
        {
            userRole.CreatedBy = updateDto.CreatedBy.Value;
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUserRole(Guid id)
    {
        var userRole = await _context.UserRoles.FindAsync(id);

        if (userRole == null)
        {
            return NotFound();
        }

        _context.UserRoles.Remove(userRole);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
