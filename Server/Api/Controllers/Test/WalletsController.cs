/*
using Api.Dto;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WalletsController : ControllerBase
{
    private readonly MyDbContext _context;

    public WalletsController(MyDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WalletDto>>> GetWallets()
    {
        var wallets = await _context.Wallets
            .Select(w => new WalletDto
            {
                Id = w.Id,
                Balance = w.Balance,
                UserId = w.UserId,
                UpdatedAt = w.UpdatedAt,
                IsDeleted = w.IsDeleted
            })
            .ToListAsync();

        return Ok(wallets);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WalletDto>> GetWallet(Guid id)
    {
        var wallet = await _context.Wallets
            .Where(w => w.Id == id)
            .Select(w => new WalletDto
            {
                Id = w.Id,
                Balance = w.Balance,
                UserId = w.UserId,
                UpdatedAt = w.UpdatedAt,
                IsDeleted = w.IsDeleted
            })
            .FirstOrDefaultAsync();

        if (wallet == null)
        {
            return NotFound();
        }

        return Ok(wallet);
    }

    [HttpPost]
    public async Task<ActionResult<WalletDto>> CreateWallet(CreateWalletDto createDto)
    {
        var wallet = new Wallet
        {
            Balance = createDto.Balance,
            UserId = createDto.UserId,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Wallets.Add(wallet);
        await _context.SaveChangesAsync();

        var walletDto = new WalletDto
        {
            Id = wallet.Id,
            Balance = wallet.Balance,
            UserId = wallet.UserId,
            UpdatedAt = wallet.UpdatedAt,
            IsDeleted = wallet.IsDeleted
        };

        return CreatedAtAction(nameof(GetWallet), new { id = wallet.Id }, walletDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateWallet(Guid id, UpdateWalletDto updateDto)
    {
        var wallet = await _context.Wallets.FindAsync(id);

        if (wallet == null)
        {
            return NotFound();
        }

        if (updateDto.Balance.HasValue)
        {
            wallet.Balance = updateDto.Balance.Value;
        }
        if (updateDto.IsDeleted.HasValue)
        {
            wallet.IsDeleted = updateDto.IsDeleted.Value;
        }
        wallet.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWallet(Guid id)
    {
        var wallet = await _context.Wallets.FindAsync(id);

        if (wallet == null)
        {
            return NotFound();
        }

        _context.Wallets.Remove(wallet);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
*/

namespace Api.Controllers.Test;