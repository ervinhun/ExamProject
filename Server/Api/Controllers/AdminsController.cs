using Api.Dto;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminsController : ControllerBase
{
    private readonly MyDbContext _context;

    public AdminsController(MyDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AdminDto>>> GetAdmins()
    {
        var admins = await _context.Admins
            .Select(a => new AdminDto
            {
                Id = a.Id,
                UserId = a.UserId
            })
            .ToListAsync();

        return Ok(admins);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AdminDto>> GetAdmin(Guid id)
    {
        var admin = await _context.Admins
            .Where(a => a.Id == id)
            .Select(a => new AdminDto
            {
                Id = a.Id,
                UserId = a.UserId
            })
            .FirstOrDefaultAsync();

        if (admin == null)
        {
            return NotFound();
        }

        return Ok(admin);
    }

    [HttpPost]
    public async Task<ActionResult<AdminDto>> CreateAdmin(CreateAdminDto createDto)
    {
        var admin = new Admin
        {
            UserId = createDto.UserId
        };

        _context.Admins.Add(admin);
        await _context.SaveChangesAsync();

        var adminDto = new AdminDto
        {
            Id = admin.Id,
            UserId = admin.UserId
        };

        return CreatedAtAction(nameof(GetAdmin), new { id = admin.Id }, adminDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAdmin(Guid id, UpdateAdminDto updateDto)
    {
        var admin = await _context.Admins.FindAsync(id);

        if (admin == null)
        {
            return NotFound();
        }

        if (updateDto.UserId.HasValue)
        {
            admin.UserId = updateDto.UserId.Value;
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAdmin(Guid id)
    {
        var admin = await _context.Admins.FindAsync(id);

        if (admin == null)
        {
            return NotFound();
        }

        _context.Admins.Remove(admin);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
