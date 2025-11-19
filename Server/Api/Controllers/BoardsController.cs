using Api.Dto;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BoardsController : ControllerBase
{
    private readonly MyDbContext _context;

    public BoardsController(MyDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BoardDto>>> GetBoards()
    {
        var boards = await _context.Boards
            .Select(b => new BoardDto
            {
                Id = b.Id,
                UserId = b.UserId,
                Numbers = b.Numbers,
                FieldCount = b.FieldCount,
                CreatedAt = b.CreatedAt,
                IsDeleted = b.IsDeleted
            })
            .ToListAsync();

        return Ok(boards);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BoardDto>> GetBoard(Guid id)
    {
        var board = await _context.Boards
            .Where(b => b.Id == id)
            .Select(b => new BoardDto
            {
                Id = b.Id,
                UserId = b.UserId,
                Numbers = b.Numbers,
                FieldCount = b.FieldCount,
                CreatedAt = b.CreatedAt,
                IsDeleted = b.IsDeleted
            })
            .FirstOrDefaultAsync();

        if (board == null)
        {
            return NotFound();
        }

        return Ok(board);
    }

    [HttpPost]
    public async Task<ActionResult<BoardDto>> CreateBoard(CreateBoardDto createDto)
    {
        var board = new Board
        {
            UserId = createDto.UserId,
            Numbers = createDto.Numbers,
            FieldCount = createDto.FieldCount,
            CreatedAt = DateTime.UtcNow
        };

        _context.Boards.Add(board);
        await _context.SaveChangesAsync();

        var boardDto = new BoardDto
        {
            Id = board.Id,
            UserId = board.UserId,
            Numbers = board.Numbers,
            FieldCount = board.FieldCount,
            CreatedAt = board.CreatedAt,
            IsDeleted = board.IsDeleted
        };

        return CreatedAtAction(nameof(GetBoard), new { id = board.Id }, boardDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBoard(Guid id, UpdateBoardDto updateDto)
    {
        var board = await _context.Boards.FindAsync(id);

        if (board == null)
        {
            return NotFound();
        }

        if (updateDto.UserId.HasValue)
        {
            board.UserId = updateDto.UserId.Value;
        }
        if (!string.IsNullOrEmpty(updateDto.Numbers))
        {
            board.Numbers = updateDto.Numbers;
        }
        if (updateDto.FieldCount.HasValue)
        {
            board.FieldCount = updateDto.FieldCount.Value;
        }
        if (updateDto.IsDeleted.HasValue)
        {
            board.IsDeleted = updateDto.IsDeleted.Value;
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBoard(Guid id)
    {
        var board = await _context.Boards.FindAsync(id);

        if (board == null)
        {
            return NotFound();
        }

        _context.Boards.Remove(board);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
