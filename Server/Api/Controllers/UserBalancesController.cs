/*
using Api.Dto;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserBalancesController : ControllerBase
    {
        private readonly MyDbContext _context;

        public UserBalancesController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/UserBalances
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserBalanceDto>>> GetUserBalances()
        {
            var userBalances = await _context.UserBalances
                .Select(ub => new UserBalanceDto
                {
                    Id = ub.Id,
                    UserId = ub.UserId,
                    Balance = ub.Balance,
                    UpdatedAt = ub.UpdatedAt,
                    IsDeleted = ub.IsDeleted
                })
                .ToListAsync();

            return Ok(userBalances);
        }

        // GET: api/UserBalances/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserBalanceDto>> GetUserBalance(Guid id)
        {
            var userBalance = await _context.UserBalances
                .Select(ub => new UserBalanceDto
                {
                    Id = ub.Id,
                    UserId = ub.UserId,
                    Balance = ub.Balance,
                    UpdatedAt = ub.UpdatedAt,
                    IsDeleted = ub.IsDeleted
                })
                .FirstOrDefaultAsync(ub => ub.Id == id);

            if (userBalance == null)
            {
                return NotFound();
            }

            return Ok(userBalance);
        }

        // POST: api/UserBalances
        [HttpPost]
        public async Task<ActionResult<UserBalanceDto>> PostUserBalance(CreateUserBalanceDto createUserBalanceDto)
        {
            var userBalance = new UserBalance
            {
                UserId = createUserBalanceDto.UserId,
                Balance = createUserBalanceDto.Balance
            };

            _context.UserBalances.Add(userBalance);
            await _context.SaveChangesAsync();

            var userBalanceDto = new UserBalanceDto
            {
                Id = userBalance.Id,
                UserId = userBalance.UserId,
                Balance = userBalance.Balance,
                UpdatedAt = userBalance.UpdatedAt,
                IsDeleted = userBalance.IsDeleted
            };

            return CreatedAtAction(nameof(GetUserBalance), new { id = userBalance.Id }, userBalanceDto);
        }

        // PUT: api/UserBalances/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserBalance(Guid id, UpdateUserBalanceDto updateUserBalanceDto)
        {
            var userBalance = await _context.UserBalances.FindAsync(id);

            if (userBalance == null)
            {
                return NotFound();
            }

            userBalance.Balance = updateUserBalanceDto.Balance ?? userBalance.Balance;
            userBalance.IsDeleted = updateUserBalanceDto.IsDeleted ?? userBalance.IsDeleted;
            userBalance.UpdatedAt = DateTime.UtcNow;

            _context.Entry(userBalance).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserBalanceExists(id))
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

        // DELETE: api/UserBalances/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserBalance(Guid id)
        {
            var userBalance = await _context.UserBalances.FindAsync(id);
            if (userBalance == null)
            {
                return NotFound();
            }

            _context.UserBalances.Remove(userBalance);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserBalanceExists(Guid id)
        {
            return _context.UserBalances.Any(e => e.Id == id);
        }
    }
}
*/
