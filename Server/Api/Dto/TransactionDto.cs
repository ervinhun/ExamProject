namespace Api.Dto;

public class TransactionDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string TransactionNumber { get; set; }
    public int Amount { get; set; }
    public string Status { get; set; }
    public Guid? ReviewedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
