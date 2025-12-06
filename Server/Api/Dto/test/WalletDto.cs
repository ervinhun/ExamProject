using Api.Dto.Transaction;
using DataAccess.Enums;

namespace Api.Dto.test;

public class WalletDto
{
    public Guid Id { get; set; }
    public Guid? PlayerId { get; set; }
    public string? AccountNumber { get; set; }
    public double Balance { get; set; }
    public ICollection<TransactionDto> Transactions { get; set; } = new  List<TransactionDto>();
    public DateTime? UpdatedAt { get; set; }
    public bool? IsDeleted { get; set; }
}
