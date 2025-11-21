/*
using Api.Dto;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserHistoriesController : ControllerBase
{
    private readonly MyDbContext _context;

    public UserHistoriesController(MyDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserHistoryDto>>> GetUserHistories()
    {
        var userHistories = await _context.UserHistories
            .Select(uh => new UserHistoryDto
            {
                Id = uh.Id,
                PublicId = uh.PublicId,
                UserId = uh.UserId,
                Title = uh.Title,
                Description = uh.Description,
                CreatedAt = uh.CreatedAt
            })
            .ToListAsync();

        return Ok(userHistories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserHistoryDto>> GetUserHistory(long id)
    {
        var userHistory = await _context.UserHistories
            .Where(uh => uh.Id == id)
            .Select(uh => new UserHistoryDto
            {
                Id = uh.Id,
                PublicId = uh.PublicId,
                UserId = uh.UserId,
                Title = uh.Title,
                Description = uh.Description,
                CreatedAt = uh.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (userHistory == null)
        {
            return NotFound();
        }

        return Ok(userHistory);
    }

    [HttpPost]
    public async Task<ActionResult<UserHistoryDto>> CreateUserHistory(CreateUserHistoryDto createDto)
    {
        var userHistory = new UserHistory
        {
            UserId = createDto.UserId,
            Title = createDto.Title,
            Description = createDto.Description,
            CreatedAt = DateTime.UtcNow
        };

        _context.UserHistories.Add(userHistory);
        await _context.SaveChangesAsync();

        var userHistoryDto = new UserHistoryDto
        {
            Id = userHistory.Id,
            PublicId = userHistory.PublicId,
            UserId = userHistory.UserId,
            Title = userHistory.Title,
            Description = userHistory.Description,
            CreatedAt = userHistory.CreatedAt
        };

        return CreatedAtAction(nameof(GetUserHistory), new { id = userHistory.Id }, userHistoryDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUserHistory(long id, UpdateUserHistoryDto updateDto)
    {
        var userHistory = await _context.UserHistories.FindAsync(id);

        if (userHistory == null)
        {
            return NotFound();
        }

        if (updateDto.UserId.HasValue)
        {
            userHistory.UserId = updateDto.UserId.Value;
        }
        if (!string.IsNullOrEmpty(updateDto.Title))
        {
            userHistory.Title = updateDto.Title;
        }
        if (!string.IsNullOrEmpty(updateDto.Description))
        {
            userHistory.Description = updateDto.Description;
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUserHistory(long id)
    {
        var userHistory = await _context.UserHistories.FindAsync(id);

        if (userHistory == null)
        {
            return NotFound();
        }

        _context.UserHistories.Remove(userHistory);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
*/

namespace Api.Controllers.Test;