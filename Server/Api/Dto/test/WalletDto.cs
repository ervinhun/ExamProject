namespace Api.Dto.test;

public class WalletDto
{
    public Guid Id { get; set; }
    public Guid PlayerId { get; set; }
    public decimal Balance { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool? IsDeleted { get; set; }
}
