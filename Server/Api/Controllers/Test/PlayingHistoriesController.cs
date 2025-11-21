/*
using Api.Dto;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayingHistoriesController : ControllerBase
{
    private readonly MyDbContext _context;

    public PlayingHistoriesController(MyDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlayingHistoryDto>>> GetPlayingHistories()
    {
        var playingHistories = await _context.PlayingHistories
            .Select(ph => new PlayingHistoryDto
            {
                Id = ph.Id,
                PublicId = ph.PublicId,
                UserId = ph.UserId,
                TicketId = ph.TicketId,
                GameId = ph.GameId,
                Title = ph.Title,
                Description = ph.Description,
                CreatedAt = ph.CreatedAt
            })
            .ToListAsync();

        return Ok(playingHistories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PlayingHistoryDto>> GetPlayingHistory(long id)
    {
        var playingHistory = await _context.PlayingHistories
            .Where(ph => ph.Id == id)
            .Select(ph => new PlayingHistoryDto
            {
                Id = ph.Id,
                PublicId = ph.PublicId,
                UserId = ph.UserId,
                TicketId = ph.TicketId,
                GameId = ph.GameId,
                Title = ph.Title,
                Description = ph.Description,
                CreatedAt = ph.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (playingHistory == null)
        {
            return NotFound();
        }

        return Ok(playingHistory);
    }

    [HttpPost]
    public async Task<ActionResult<PlayingHistoryDto>> CreatePlayingHistory(CreatePlayingHistoryDto createDto)
    {
        var playingHistory = new PlayingHistory
        {
            UserId = createDto.UserId,
            TicketId = createDto.TicketId,
            GameId = createDto.GameId,
            Title = createDto.Title,
            Description = createDto.Description,
            CreatedAt = DateTime.UtcNow
        };

        _context.PlayingHistories.Add(playingHistory);
        await _context.SaveChangesAsync();

        var playingHistoryDto = new PlayingHistoryDto
        {
            Id = playingHistory.Id,
            PublicId = playingHistory.PublicId,
            UserId = playingHistory.UserId,
            TicketId = playingHistory.TicketId,
            GameId = playingHistory.GameId,
            Title = playingHistory.Title,
            Description = playingHistory.Description,
            CreatedAt = playingHistory.CreatedAt
        };

        return CreatedAtAction(nameof(GetPlayingHistory), new { id = playingHistory.Id }, playingHistoryDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePlayingHistory(long id, UpdatePlayingHistoryDto updateDto)
    {
        var playingHistory = await _context.PlayingHistories.FindAsync(id);

        if (playingHistory == null)
        {
            return NotFound();
        }

        if (updateDto.UserId.HasValue)
        {
            playingHistory.UserId = updateDto.UserId.Value;
        }
        if (updateDto.TicketId.HasValue)
        {
            playingHistory.TicketId = updateDto.TicketId.Value;
        }
        if (updateDto.GameId.HasValue)
        {
            playingHistory.GameId = updateDto.GameId.Value;
        }
        if (!string.IsNullOrEmpty(updateDto.Title))
        {
            playingHistory.Title = updateDto.Title;
        }
        if (!string.IsNullOrEmpty(updateDto.Description))
        {
            playingHistory.Description = updateDto.Description;
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePlayingHistory(long id)
    {
        var playingHistory = await _context.PlayingHistories.FindAsync(id);

        if (playingHistory == null)
        {
            return NotFound();
        }

        _context.PlayingHistories.Remove(playingHistory);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
*/

namespace Api.Controllers.Test;