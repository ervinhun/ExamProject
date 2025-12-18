namespace Api.Dto.Transaction;

public class DepositRequestDto
{
    public Guid? PlayerId { get; set; }
    public Guid WalletId { get; set; }
    public double Amount { get; set; }
    public string? MobilePayTransactionNumber { get; set; }
}