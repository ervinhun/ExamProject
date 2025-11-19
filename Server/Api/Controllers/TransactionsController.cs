/*
using Api.Dto;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly MyDbContext _context;

    public TransactionsController(MyDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransactionDto>>> GetTransactions()
    {
        var transactions = await _context.Transactions
            .Select(t => new TransactionDto
            {
                Id = t.Id,
                PlayerId = t.PlayerId,
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

    [HttpGet("{id}")]
    public async Task<ActionResult<TransactionDto>> GetTransaction(Guid id)
    {
        var transaction = await _context.Transactions
            .Where(t => t.Id == id)
            .Select(t => new TransactionDto
            {
                Id = t.Id,
                PlayerId = t.PlayerId,
                TransactionNumber = t.TransactionNumber,
                Amount = t.Amount,
                Status = t.Status,
                ReviewedBy = t.ReviewedBy,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            })
            .FirstOrDefaultAsync();

        if (transaction == null)
        {
            return NotFound();
        }

        return Ok(transaction);
    }

    [HttpPost]
    public async Task<ActionResult<TransactionDto>> CreateTransaction(CreateTransactionDto createDto)
    {
        var transaction = new Transaction
        {
            PlayerId = createDto.PlayerId,
            Amount = createDto.Amount,
            Status = createDto.Status,
            TransactionNumber = Guid.NewGuid().ToString(),
            CreatedAt = DateTime.UtcNow
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        var transactionDto = new TransactionDto
        {
            Id = transaction.Id,
            PlayerId = transaction.PlayerId,
            TransactionNumber = transaction.TransactionNumber,
            Amount = transaction.Amount,
            Status = transaction.Status,
            ReviewedBy = transaction.ReviewedBy,
            CreatedAt = transaction.CreatedAt,
            UpdatedAt = transaction.UpdatedAt
        };

        return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transactionDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTransaction(Guid id, UpdateTransactionDto updateDto)
    {
        var transaction = await _context.Transactions.FindAsync(id);

        if (transaction == null)
        {
            return NotFound();
        }

        if (updateDto.PlayerId.HasValue)
        {
            transaction.PlayerId = updateDto.PlayerId.Value;
        }
        if (updateDto.Amount.HasValue)
        {
            transaction.Amount = updateDto.Amount.Value;
        }
        if (!string.IsNullOrEmpty(updateDto.Status))
        {
            transaction.Status = updateDto.Status;
        }
        if (updateDto.ReviewedBy.HasValue)
        {
            transaction.ReviewedBy = updateDto.ReviewedBy.Value;
            transaction.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

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
}
*/
