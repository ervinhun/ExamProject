using DataAccess.Entities.Auth;

namespace DataAccess.Entities.Finance;

public class Transaction
{
    public Guid Id { get; set; }
    public required Guid PlayerId { get; set; }
    public Player Player { get; set; }
    public required Guid WalletId { get; set; }
    public Wallet Wallet { get; set; }
    public Guid ReviewedBy { get; set; } = Guid.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Amount { get; set; } = decimal.Zero;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
   
    
}