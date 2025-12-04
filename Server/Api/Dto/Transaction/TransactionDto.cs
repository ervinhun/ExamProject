using Api.Dto.User;
using DataAccess.Entities.Finance;
using DataAccess.Enums;

namespace Api.Dto.Transaction;

public record TransactionDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? Name { get; set; }
    public Guid WalletId { get; set; }
    public string? TransactionNumber { get; set; }
    public double Amount { get; set; }
    public ICollection<TransactionHistoryDto>  TransactionHistory { get; set; }
    public TransactionStatus Status { get; set; }
    public TransactionType Type { get; set; }
    public Guid? ReviewedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class TransactionHistoryDto
{
    public Guid Id { get; set; }
    public required Guid? TransactionId { get; set; }
    public TransactionDto? Transaction { get; set; }
    public required Guid ActionUser { get; set; }
    public TransactionStatus? Status { get; set; }
    public TransactionType? Type { get; set; }
}

public class CreateTransactionRequestDto
{
    public decimal Amount { get; set; }
}

public class UpdateTransactionDto
{
    public string? Status { get; set; }
    public Guid? ReviewedBy { get; set; }
}
