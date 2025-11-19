/*
using Api.Dto;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayingNumbersController : ControllerBase
{
    private readonly MyDbContext _context;

    public PlayingNumbersController(MyDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlayingNumberDto>>> GetPlayingNumbers()
    {
        var playingNumbers = await _context.PlayingNumbers
            .Select(pn => new PlayingNumberDto
            {
                Id = pn.Id,
                PlayingBoardId = pn.PlayingBoardId,
                CreatedAt = pn.CreatedAt
            })
            .ToListAsync();

        return Ok(playingNumbers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PlayingNumberDto>> GetPlayingNumber(Guid id)
    {
        var playingNumber = await _context.PlayingNumbers
            .Where(pn => pn.Id == id)
            .Select(pn => new PlayingNumberDto
            {
                Id = pn.Id,
                PlayingBoardId = pn.PlayingBoardId,
                CreatedAt = pn.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (playingNumber == null)
        {
            return NotFound();
        }

        return Ok(playingNumber);
    }

    [HttpPost]
    public async Task<ActionResult<PlayingNumberDto>> CreatePlayingNumber(CreatePlayingNumberDto createDto)
    {
        var playingNumber = new PlayingNumber
        {
            PlayingBoardId = createDto.PlayingBoardId,
            CreatedAt = DateTime.UtcNow
        };

        _context.PlayingNumbers.Add(playingNumber);
        await _context.SaveChangesAsync();

        var playingNumberDto = new PlayingNumberDto
        {
            Id = playingNumber.Id,
            PlayingBoardId = playingNumber.PlayingBoardId,
            CreatedAt = playingNumber.CreatedAt
        };

        return CreatedAtAction(nameof(GetPlayingNumber), new { id = playingNumber.Id }, playingNumberDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePlayingNumber(Guid id, UpdatePlayingNumberDto updateDto)
    {
        var playingNumber = await _context.PlayingNumbers.FindAsync(id);

        if (playingNumber == null)
        {
            return NotFound();
        }

        if (updateDto.PlayingBoardId.HasValue)
        {
            playingNumber.PlayingBoardId = updateDto.PlayingBoardId.Value;
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePlayingNumber(Guid id)
    {
        var playingNumber = await _context.PlayingNumbers.FindAsync(id);

        if (playingNumber == null)
        {
            return NotFound();
        }

        _context.PlayingNumbers.Remove(playingNumber);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
*/
