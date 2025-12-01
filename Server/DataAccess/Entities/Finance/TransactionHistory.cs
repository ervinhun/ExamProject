namespace DataAccess.Entities.Finance;

public record TransactionHistory
{
    public Guid Id { get; set; }
    public Guid TransactionId { get; set; }
    public required Transaction Transaction { get; set; }
}