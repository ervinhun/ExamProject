using System.ComponentModel.DataAnnotations;
using DataAccess.Entities.Auth;
using DataAccess.Enums;

namespace DataAccess.Entities.Finance;

public class Transaction
{
    public Guid Id { get; set; }
    public required Guid PlayerId { get; set; }
    public User? User { get; set; }
    public required Guid WalletId { get; set; }
    public Wallet? Wallet { get; set; }
    public TransactionStatus Status { get; set; }
    public TransactionType Type { get; set; }
    public List<TransactionHistory>? TransactionHistory { get; set; }
    public Guid ActionUser { get; set; } = Guid.Empty;
    [Range(0, double.MaxValue)]
    public double Amount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
    
}