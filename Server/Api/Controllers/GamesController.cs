/*
using Api.Dto;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly MyDbContext _context;

        public GamesController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/Games
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
                    WinningNumber1 = g.WinningNumber1,
                    WinningNumber2 = g.WinningNumber2,
                    WinningNumber3 = g.WinningNumber3,
                    CreatedAt = g.CreatedAt,
                    ClosedAt = g.ClosedAt
                })
                .ToListAsync();

            return Ok(games);
        }

        // GET: api/Games/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GameDto>> GetGame(Guid id)
        {
            var game = await _context.Games
                .Select(g => new GameDto
                {
                    Id = g.Id,
                    StartDate = g.StartDate,
                    EndDate = g.EndDate,
                    IsClosed = g.IsClosed,
                    WinningNumber1 = g.WinningNumber1,
                    WinningNumber2 = g.WinningNumber2,
                    WinningNumber3 = g.WinningNumber3,
                    CreatedAt = g.CreatedAt,
                    ClosedAt = g.ClosedAt
                })
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null)
            {
                return NotFound();
            }

            return Ok(game);
        }

        // POST: api/Games
        [HttpPost]
        public async Task<ActionResult<GameDto>> PostGame(CreateGameDto createGameDto)
        {
            var game = new Game
            {
                StartDate = createGameDto.StartDate,
                EndDate = createGameDto.EndDate
            };

            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            var gameDto = new GameDto
            {
                Id = game.Id,
                StartDate = game.StartDate,
                EndDate = game.EndDate,
                IsClosed = game.IsClosed,
                WinningNumber1 = game.WinningNumber1,
                WinningNumber2 = game.WinningNumber2,
                WinningNumber3 = game.WinningNumber3,
                CreatedAt = game.CreatedAt,
                ClosedAt = game.ClosedAt
            };

            return CreatedAtAction(nameof(GetGame), new { id = game.Id }, gameDto);
        }

        // PUT: api/Games/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGame(Guid id, UpdateGameDto updateGameDto)
        {
            var game = await _context.Games.FindAsync(id);

            if (game == null)
            {
                return NotFound();
            }

            game.StartDate = updateGameDto.StartDate ?? game.StartDate;
            game.EndDate = updateGameDto.EndDate ?? game.EndDate;
            game.IsClosed = updateGameDto.IsClosed ?? game.IsClosed;
            game.WinningNumber1 = updateGameDto.WinningNumber1 ?? game.WinningNumber1;
            game.WinningNumber2 = updateGameDto.WinningNumber2 ?? game.WinningNumber2;
            game.WinningNumber3 = updateGameDto.WinningNumber3 ?? game.WinningNumber3;
            
            if (updateGameDto.IsClosed == true && game.IsClosed != true)
            {
                game.ClosedAt = DateTime.UtcNow;
            }


            _context.Entry(game).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameExists(id))
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

        // DELETE: api/Games/5
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

        private bool GameExists(Guid id)
        {
            return _context.Games.Any(e => e.Id == id);
        }
    }
}
*/
