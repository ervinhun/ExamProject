using DataAccess.Enums;

namespace DataAccess.Entities.Finance;

public record TransactionHistory
{
    public Guid Id { get; set; }
    public required Guid TransactionId { get; set; }
    public Transaction? Transaction { get; set; }
    public required Guid ActionUser { get; set; } = Guid.Empty;
    public TransactionStatus Status { get; set; }
    public TransactionType Type { get; set; }
}