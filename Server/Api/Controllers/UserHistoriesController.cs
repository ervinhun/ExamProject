using Api.Dto;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserHistoriesController : ControllerBase
    {
        private readonly MyDbContext _context;

        public UserHistoriesController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/UserHistories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserHistoryDto>>> GetUserHistories()
        {
            var userHistories = await _context.UserHistories
                .Select(uh => new UserHistoryDto
                {
                    Id = uh.Id,
                    PublicId = uh.PublicId,
                    UserId = uh.UserId,
                    Title = uh.Title,
                    Description = uh.Description,
                    CreatedAt = uh.CreatedAt
                })
                .ToListAsync();

            return Ok(userHistories);
        }

        // GET: api/UserHistories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserHistoryDto>> GetUserHistory(long id)
        {
            var userHistory = await _context.UserHistories
                .Select(uh => new UserHistoryDto
                {
                    Id = uh.Id,
                    PublicId = uh.PublicId,
                    UserId = uh.UserId,
                    Title = uh.Title,
                    Description = uh.Description,
                    CreatedAt = uh.CreatedAt
                })
                .FirstOrDefaultAsync(uh => uh.Id == id);

            if (userHistory == null)
            {
                return NotFound();
            }

            return Ok(userHistory);
        }

        // POST: api/UserHistories
        [HttpPost]
        public async Task<ActionResult<UserHistoryDto>> PostUserHistory(CreateUserHistoryDto createUserHistoryDto)
        {
            var userHistory = new UserHistory
            {
                UserId = createUserHistoryDto.UserId,
                Title = createUserHistoryDto.Title,
                Description = createUserHistoryDto.Description
            };

            _context.UserHistories.Add(userHistory);
            await _context.SaveChangesAsync();

            var userHistoryDto = new UserHistoryDto
            {
                Id = userHistory.Id,
                PublicId = userHistory.PublicId,
                UserId = userHistory.UserId,
                Title = userHistory.Title,
                Description = userHistory.Description,
                CreatedAt = userHistory.CreatedAt
            };

            return CreatedAtAction(nameof(GetUserHistory), new { id = userHistory.Id }, userHistoryDto);
        }
    }
}
