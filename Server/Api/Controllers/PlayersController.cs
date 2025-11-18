using Api.Dto;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayersController : ControllerBase
{
    private readonly MyDbContext _context;

    public PlayersController(MyDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlayerDto>>> GetPlayers()
    {
        var players = await _context.Players
            .Select(p => new PlayerDto
            {
                Id = p.Id,
                UserId = p.UserId,
                WalletId = p.WalletId,
                IsActive = p.IsActive
            })
            .ToListAsync();

        return Ok(players);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PlayerDto>> GetPlayer(Guid id)
    {
        var player = await _context.Players
            .Where(p => p.Id == id)
            .Select(p => new PlayerDto
            {
                Id = p.Id,
                UserId = p.UserId,
                WalletId = p.WalletId,
                IsActive = p.IsActive
            })
            .FirstOrDefaultAsync();

        if (player == null)
        {
            return NotFound();
        }

        return Ok(player);
    }

    [HttpPost]
    public async Task<ActionResult<PlayerDto>> CreatePlayer(CreatePlayerDto createDto)
    {
        var player = new Player
        {
            UserId = createDto.UserId,
            WalletId = createDto.WalletId,
            IsActive = createDto.IsActive
        };

        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        var playerDto = new PlayerDto
        {
            Id = player.Id,
            UserId = player.UserId,
            WalletId = player.WalletId,
            IsActive = player.IsActive
        };

        return CreatedAtAction(nameof(GetPlayer), new { id = player.Id }, playerDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePlayer(Guid id, UpdatePlayerDto updateDto)
    {
        var player = await _context.Players.FindAsync(id);

        if (player == null)
        {
            return NotFound();
        }

        if (updateDto.UserId.HasValue)
        {
            player.UserId = updateDto.UserId.Value;
        }
        if (updateDto.WalletId.HasValue)
        {
            player.WalletId = updateDto.WalletId.Value;
        }
        if (updateDto.IsActive.HasValue)
        {
            player.IsActive = updateDto.IsActive.Value;
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePlayer(Guid id)
    {
        var player = await _context.Players.FindAsync(id);

        if (player == null)
        {
            return NotFound();
        }

        _context.Players.Remove(player);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
