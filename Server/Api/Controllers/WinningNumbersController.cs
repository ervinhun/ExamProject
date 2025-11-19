/*
using Api.Dto;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WinningNumbersController : ControllerBase
{
    private readonly MyDbContext _context;

    public WinningNumbersController(MyDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WinningNumberDto>>> GetWinningNumbers()
    {
        var winningNumbers = await _context.WinningNumbers
            .Select(wn => new WinningNumberDto
            {
                Id = wn.Id,
                GameId = wn.GameId,
                CreatedAt = wn.CreatedAt
            })
            .ToListAsync();

        return Ok(winningNumbers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WinningNumberDto>> GetWinningNumber(Guid id)
    {
        var winningNumber = await _context.WinningNumbers
            .Where(wn => wn.Id == id)
            .Select(wn => new WinningNumberDto
            {
                Id = wn.Id,
                GameId = wn.GameId,
                CreatedAt = wn.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (winningNumber == null)
        {
            return NotFound();
        }

        return Ok(winningNumber);
    }

    [HttpPost]
    public async Task<ActionResult<WinningNumberDto>> CreateWinningNumber(CreateWinningNumberDto createDto)
    {
        var winningNumber = new WinningNumber
        {
            GameId = createDto.GameId,
            CreatedAt = DateTime.UtcNow
        };

        _context.WinningNumbers.Add(winningNumber);
        await _context.SaveChangesAsync();

        var winningNumberDto = new WinningNumberDto
        {
            Id = winningNumber.Id,
            GameId = winningNumber.GameId,
            CreatedAt = winningNumber.CreatedAt
        };

        return CreatedAtAction(nameof(GetWinningNumber), new { id = winningNumber.Id }, winningNumberDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateWinningNumber(Guid id, UpdateWinningNumberDto updateDto)
    {
        var winningNumber = await _context.WinningNumbers.FindAsync(id);

        if (winningNumber == null)
        {
            return NotFound();
        }

        if (updateDto.GameId.HasValue)
        {
            winningNumber.GameId = updateDto.GameId.Value;
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWinningNumber(Guid id)
    {
        var winningNumber = await _context.WinningNumbers.FindAsync(id);

        if (winningNumber == null)
        {
            return NotFound();
        }

        _context.WinningNumbers.Remove(winningNumber);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
*/
