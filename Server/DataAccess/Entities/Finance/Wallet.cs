using System.ComponentModel.DataAnnotations;
using DataAccess.Entities.Auth;

namespace DataAccess.Entities.Finance;

public class Wallet
{
    public Guid Id { get; set; }
    public Guid PlayerId { get; set; }
    public string? AccountNumber { get; set; }
    public required Player Player { get; set; }
    [Range(0, double.MaxValue)] public double Balance { get; set; }

    public ICollection<Transaction> Transactions { get; set; } =  new  List<Transaction>();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
