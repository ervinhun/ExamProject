/*
using Api.Dto;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoardsController : ControllerBase
    {
        private readonly MyDbContext _context;

        public BoardsController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/Boards
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

        // GET: api/Boards/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BoardDto>> GetBoard(Guid id)
        {
            var board = await _context.Boards
                .Select(b => new BoardDto
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    Numbers = b.Numbers,
                    FieldCount = b.FieldCount,
                    CreatedAt = b.CreatedAt,
                    IsDeleted = b.IsDeleted
                })
                .FirstOrDefaultAsync(b => b.Id == id);

            if (board == null)
            {
                return NotFound();
            }

            return Ok(board);
        }

        // POST: api/Boards
        [HttpPost]
        public async Task<ActionResult<BoardDto>> PostBoard(CreateBoardDto createBoardDto)
        {
            var board = new Board
            {
                UserId = createBoardDto.UserId,
                Numbers = createBoardDto.Numbers,
                FieldCount = createBoardDto.FieldCount
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

        // PUT: api/Boards/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBoard(Guid id, UpdateBoardDto updateBoardDto)
        {
            var board = await _context.Boards.FindAsync(id);

            if (board == null)
            {
                return NotFound();
            }

            board.Numbers = updateBoardDto.Numbers ?? board.Numbers;
            board.FieldCount = updateBoardDto.FieldCount ?? board.FieldCount;
            board.IsDeleted = updateBoardDto.IsDeleted ?? board.IsDeleted;

            _context.Entry(board).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BoardExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Boards/5
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

        private bool BoardExists(Guid id)
        {
            return _context.Boards.Any(e => e.Id == id);
        }
    }
}
*/
