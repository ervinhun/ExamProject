using Api.Dto;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly MyDbContext _context;

        public TransactionsController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/Transactions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetTransactions()
        {
            var transactions = await _context.Transactions
                .Select(t => new TransactionDto
                {
                    Id = t.Id,
                    UserId = t.UserId,
                    TransactionNumber = t.TransactionNumber,
                    Amount = t.Amount,
                    Status = t.Status,
                    ReviewedBy = t.ReviewedBy,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                })
                .ToListAsync();

            return Ok(transactions);
        }

        // GET: api/Transactions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionDto>> GetTransaction(Guid id)
        {
            var transaction = await _context.Transactions
                .Select(t => new TransactionDto
                {
                    Id = t.Id,
                    UserId = t.UserId,
                    TransactionNumber = t.TransactionNumber,
                    Amount = t.Amount,
                    Status = t.Status,
                    ReviewedBy = t.ReviewedBy,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                })
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transaction == null)
            {
                return NotFound();
            }

            return Ok(transaction);
        }

        // POST: api/Transactions
        [HttpPost]
        public async Task<ActionResult<TransactionDto>> PostTransaction(CreateTransactionDto createTransactionDto)
        {
            var transaction = new Transaction
            {
                UserId = createTransactionDto.UserId,
                Amount = createTransactionDto.Amount,
                TransactionNumber = Guid.NewGuid().ToString(),
                Status = "Pending"
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            var transactionDto = new TransactionDto
            {
                Id = transaction.Id,
                UserId = transaction.UserId,
                TransactionNumber = transaction.TransactionNumber,
                Amount = transaction.Amount,
                Status = transaction.Status,
                ReviewedBy = transaction.ReviewedBy,
                CreatedAt = transaction.CreatedAt,
                UpdatedAt = transaction.UpdatedAt
            };

            return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transactionDto);
        }

        // PUT: api/Transactions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransaction(Guid id, UpdateTransactionDto updateTransactionDto)
        {
            var transaction = await _context.Transactions.FindAsync(id);

            if (transaction == null)
            {
                return NotFound();
            }

            transaction.Status = updateTransactionDto.Status ?? transaction.Status;
            transaction.ReviewedBy = updateTransactionDto.ReviewedBy ?? transaction.ReviewedBy;
            transaction.UpdatedAt = DateTime.UtcNow;

            _context.Entry(transaction).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionExists(id))
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

        // DELETE: api/Transactions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(Guid id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TransactionExists(Guid id)
        {
            return _context.Transactions.Any(e => e.Id == id);
        }
    }
}
