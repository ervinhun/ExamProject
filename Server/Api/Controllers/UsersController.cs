/*
using Api.Dto;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly MyDbContext _context;

    public UsersController(MyDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var users = await _context.Users
            .Select(u => new UserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                PhoneNo = u.PhoneNo,
                ActiveStatusExpiryDate = u.ActiveStatusExpiryDate,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
                IsDeleted = u.IsDeleted
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(Guid id)
    {
        var user = await _context.Users
            .Where(u => u.Id == id)
            .Select(u => new UserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                PhoneNo = u.PhoneNo,
                ActiveStatusExpiryDate = u.ActiveStatusExpiryDate,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
                IsDeleted = u.IsDeleted
            })
            .FirstOrDefaultAsync();

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto createDto)
    {
        var user = new User
        {
            FullName = createDto.FullName,
            Email = createDto.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(createDto.Password),
            PhoneNo = createDto.PhoneNo,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var userDto = new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNo = user.PhoneNo,
            ActiveStatusExpiryDate = user.ActiveStatusExpiryDate,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            IsDeleted = user.IsDeleted
        };

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, userDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(Guid id, UpdateUserDto updateDto)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        if (!string.IsNullOrEmpty(updateDto.FullName))
        {
            user.FullName = updateDto.FullName;
        }
        if (!string.IsNullOrEmpty(updateDto.Email))
        {
            user.Email = updateDto.Email;
        }
        if (!string.IsNullOrEmpty(updateDto.Password))
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(updateDto.Password);
        }
        if (!string.IsNullOrEmpty(updateDto.PhoneNo))
        {
            user.PhoneNo = updateDto.PhoneNo;
        }
        if (updateDto.ActiveStatusExpiryDate.HasValue)
        {
            user.ActiveStatusExpiryDate = updateDto.ActiveStatusExpiryDate.Value;
        }
        if (updateDto.IsDeleted.HasValue)
        {
            user.IsDeleted = updateDto.IsDeleted.Value;
        }
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
*/
