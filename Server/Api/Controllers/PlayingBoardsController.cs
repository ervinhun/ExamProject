using Api.Dto;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayingBoardsController : ControllerBase
    {
        private readonly MyDbContext _context;

        public PlayingBoardsController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/PlayingBoards
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

        // GET: api/PlayingBoards/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PlayingBoardDto>> GetPlayingBoard(Guid id)
        {
            var playingBoard = await _context.PlayingBoards
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
                .FirstOrDefaultAsync(pb => pb.Id == id);

            if (playingBoard == null)
            {
                return NotFound();
            }

            return Ok(playingBoard);
        }

        // POST: api/PlayingBoards
        [HttpPost]
        public async Task<ActionResult<PlayingBoardDto>> PostPlayingBoard(CreatePlayingBoardDto createPlayingBoardDto)
        {
            var playingBoard = new PlayingBoard
            {
                UserId = createPlayingBoardDto.UserId,
                BoardId = createPlayingBoardDto.BoardId,
                GameId = createPlayingBoardDto.GameId,
                Numbers = createPlayingBoardDto.Numbers,
                FieldCount = createPlayingBoardDto.FieldCount,
                Price = createPlayingBoardDto.Price,
                IsRepeat = createPlayingBoardDto.IsRepeat,
                RepeatCountRemaining = createPlayingBoardDto.RepeatCountRemaining
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

        // PUT: api/PlayingBoards/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlayingBoard(Guid id, UpdatePlayingBoardDto updatePlayingBoardDto)
        {
            var playingBoard = await _context.PlayingBoards.FindAsync(id);

            if (playingBoard == null)
            {
                return NotFound();
            }

            playingBoard.IsWinningBoard = updatePlayingBoardDto.IsWinningBoard ?? playingBoard.IsWinningBoard;
            playingBoard.RepeatCountRemaining = updatePlayingBoardDto.RepeatCountRemaining ?? playingBoard.RepeatCountRemaining;

            _context.Entry(playingBoard).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlayingBoardExists(id))
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

        // DELETE: api/PlayingBoards/5
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

        private bool PlayingBoardExists(Guid id)
        {
            return _context.PlayingBoards.Any(e => e.Id == id);
        }
    }
}
