using Api.Dto;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayingHistoriesController : ControllerBase
    {
        private readonly MyDbContext _context;

        public PlayingHistoriesController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/PlayingHistories
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
                    Title = ph.Title,
                    Description = ph.Description,
                    CreatedAt = ph.CreatedAt
                })
                .ToListAsync();

            return Ok(playingHistories);
        }

        // GET: api/PlayingHistories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PlayingHistoryDto>> GetPlayingHistory(long id)
        {
            var playingHistory = await _context.PlayingHistories
                .Select(ph => new PlayingHistoryDto
                {
                    Id = ph.Id,
                    PublicId = ph.PublicId,
                    UserId = ph.UserId,
                    TicketId = ph.TicketId,
                    Title = ph.Title,
                    Description = ph.Description,
                    CreatedAt = ph.CreatedAt
                })
                .FirstOrDefaultAsync(ph => ph.Id == id);

            if (playingHistory == null)
            {
                return NotFound();
            }

            return Ok(playingHistory);
        }

        // POST: api/PlayingHistories
        [HttpPost]
        public async Task<ActionResult<PlayingHistoryDto>> PostPlayingHistory(CreatePlayingHistoryDto createPlayingHistoryDto)
        {
            var playingHistory = new PlayingHistory
            {
                UserId = createPlayingHistoryDto.UserId,
                TicketId = createPlayingHistoryDto.TicketId,
                Title = createPlayingHistoryDto.Title,
                Description = createPlayingHistoryDto.Description
            };

            _context.PlayingHistories.Add(playingHistory);
            await _context.SaveChangesAsync();

            var playingHistoryDto = new PlayingHistoryDto
            {
                Id = playingHistory.Id,
                PublicId = playingHistory.PublicId,
                UserId = playingHistory.UserId,
                TicketId = playingHistory.TicketId,
                Title = playingHistory.Title,
                Description = playingHistory.Description,
                CreatedAt = playingHistory.CreatedAt
            };

            return CreatedAtAction(nameof(GetPlayingHistory), new { id = playingHistory.Id }, playingHistoryDto);
        }
    }
}
