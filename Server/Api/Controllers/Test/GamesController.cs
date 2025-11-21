/*
using Api.Dto;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GamesController : ControllerBase
{
    private readonly MyDbContext _context;

    public GamesController(MyDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GameDto>>> GetGames()
    {
        var games = await _context.Games
            .Select(g => new GameDto
            {
                Id = g.Id,
                StartDate = g.StartDate,
                EndDate = g.EndDate,
                IsClosed = g.IsClosed,
                CreatedAt = g.CreatedAt,
                ClosedAt = g.ClosedAt
            })
            .ToListAsync();

        return Ok(games);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GameDto>> GetGame(Guid id)
    {
        var game = await _context.Games
            .Where(g => g.Id == id)
            .Select(g => new GameDto
            {
                Id = g.Id,
                StartDate = g.StartDate,
                EndDate = g.EndDate,
                IsClosed = g.IsClosed,
                CreatedAt = g.CreatedAt,
                ClosedAt = g.ClosedAt
            })
            .FirstOrDefaultAsync();

        if (game == null)
        {
            return NotFound();
        }

        return Ok(game);
    }

    [HttpPost]
    public async Task<ActionResult<GameDto>> CreateGame(CreateGameDto createDto)
    {
        var game = new Game
        {
            StartDate = createDto.StartDate,
            EndDate = createDto.EndDate,
            CreatedAt = DateTime.UtcNow
        };

        _context.Games.Add(game);
        await _context.SaveChangesAsync();

        var gameDto = new GameDto
        {
            Id = game.Id,
            StartDate = game.StartDate,
            EndDate = game.EndDate,
            IsClosed = game.IsClosed,
            CreatedAt = game.CreatedAt,
            ClosedAt = game.ClosedAt
        };

        return CreatedAtAction(nameof(GetGame), new { id = game.Id }, gameDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGame(Guid id, UpdateGameDto updateDto)
    {
        var game = await _context.Games.FindAsync(id);

        if (game == null)
        {
            return NotFound();
        }

        if (updateDto.StartDate.HasValue)
        {
            game.StartDate = updateDto.StartDate.Value;
        }
        if (updateDto.EndDate.HasValue)
        {
            game.EndDate = updateDto.EndDate.Value;
        }
        if (updateDto.IsClosed.HasValue)
        {
            game.IsClosed = updateDto.IsClosed.Value;
        }
        if (updateDto.ClosedAt.HasValue)
        {
            game.ClosedAt = updateDto.ClosedAt.Value;
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGame(Guid id)
    {
        var game = await _context.Games.FindAsync(id);

        if (game == null)
        {
            return NotFound();
        }

        _context.Games.Remove(game);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
*/

namespace Api.Controllers.Test;