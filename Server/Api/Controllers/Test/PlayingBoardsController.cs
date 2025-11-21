/*
using Api.Dto;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayingBoardsController : ControllerBase
{
    private readonly MyDbContext _context;

    public PlayingBoardsController(MyDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlayingBoardDto>>> GetPlayingBoards()
    {
        var playingBoards = await _context.PlayingBoards
            .Select(pb => new PlayingBoardDto
            {
                Id = pb.Id,
                UserId = pb.UserId,
                BoardId = pb.BoardId,
                GameId = pb.GameId,
                Numbers = pb.Numbers,
                FieldCount = pb.FieldCount,
                Price = pb.Price,
                IsRepeat = pb.IsRepeat,
                RepeatCountRemaining = pb.RepeatCountRemaining,
                IsWinningBoard = pb.IsWinningBoard,
                CreatedAt = pb.CreatedAt
            })
            .ToListAsync();

        return Ok(playingBoards);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PlayingBoardDto>> GetPlayingBoard(Guid id)
    {
        var playingBoard = await _context.PlayingBoards
            .Where(pb => pb.Id == id)
            .Select(pb => new PlayingBoardDto
            {
                Id = pb.Id,
                UserId = pb.UserId,
                BoardId = pb.BoardId,
                GameId = pb.GameId,
                Numbers = pb.Numbers,
                FieldCount = pb.FieldCount,
                Price = pb.Price,
                IsRepeat = pb.IsRepeat,
                RepeatCountRemaining = pb.RepeatCountRemaining,
                IsWinningBoard = pb.IsWinningBoard,
                CreatedAt = pb.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (playingBoard == null)
        {
            return NotFound();
        }

        return Ok(playingBoard);
    }

    [HttpPost]
    public async Task<ActionResult<PlayingBoardDto>> CreatePlayingBoard(CreatePlayingBoardDto createDto)
    {
        var playingBoard = new PlayingBoard
        {
            UserId = createDto.UserId,
            BoardId = createDto.BoardId,
            GameId = createDto.GameId,
            Numbers = createDto.Numbers,
            FieldCount = createDto.FieldCount,
            Price = createDto.Price,
            IsRepeat = createDto.IsRepeat,
            RepeatCountRemaining = createDto.RepeatCountRemaining,
            CreatedAt = DateTime.UtcNow
        };

        _context.PlayingBoards.Add(playingBoard);
        await _context.SaveChangesAsync();

        var playingBoardDto = new PlayingBoardDto
        {
            Id = playingBoard.Id,
            UserId = playingBoard.UserId,
            BoardId = playingBoard.BoardId,
            GameId = playingBoard.GameId,
            Numbers = playingBoard.Numbers,
            FieldCount = playingBoard.FieldCount,
            Price = playingBoard.Price,
            IsRepeat = playingBoard.IsRepeat,
            RepeatCountRemaining = playingBoard.RepeatCountRemaining,
            IsWinningBoard = playingBoard.IsWinningBoard,
            CreatedAt = playingBoard.CreatedAt
        };

        return CreatedAtAction(nameof(GetPlayingBoard), new { id = playingBoard.Id }, playingBoardDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePlayingBoard(Guid id, UpdatePlayingBoardDto updateDto)
    {
        var playingBoard = await _context.PlayingBoards.FindAsync(id);

        if (playingBoard == null)
        {
            return NotFound();
        }

        if (updateDto.UserId.HasValue)
        {
            playingBoard.UserId = updateDto.UserId.Value;
        }
        if (updateDto.BoardId.HasValue)
        {
            playingBoard.BoardId = updateDto.BoardId.Value;
        }
        if (updateDto.GameId.HasValue)
        {
            playingBoard.GameId = updateDto.GameId.Value;
        }
        if (!string.IsNullOrEmpty(updateDto.Numbers))
        {
            playingBoard.Numbers = updateDto.Numbers;
        }
        if (updateDto.FieldCount.HasValue)
        {
            playingBoard.FieldCount = updateDto.FieldCount.Value;
        }
        if (updateDto.Price.HasValue)
        {
            playingBoard.Price = updateDto.Price.Value;
        }
        if (updateDto.IsRepeat.HasValue)
        {
            playingBoard.IsRepeat = updateDto.IsRepeat.Value;
        }
        if (updateDto.RepeatCountRemaining.HasValue)
        {
            playingBoard.RepeatCountRemaining = updateDto.RepeatCountRemaining.Value;
        }
        if (updateDto.IsWinningBoard.HasValue)
        {
            playingBoard.IsWinningBoard = updateDto.IsWinningBoard.Value;
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePlayingBoard(Guid id)
    {
        var playingBoard = await _context.PlayingBoards.FindAsync(id);

        if (playingBoard == null)
        {
            return NotFound();
        }

        _context.PlayingBoards.Remove(playingBoard);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
*/

namespace Api.Controllers.Test;