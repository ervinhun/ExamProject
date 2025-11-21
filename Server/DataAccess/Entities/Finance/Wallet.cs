using System.ComponentModel.DataAnnotations;
using DataAccess.Entities.Auth;

namespace DataAccess.Entities.Finance;

public class Wallet
{
    public Guid Id { get; set; }
    public required Guid PlayerId { get; set; }
    
    public Player Player { get; set; } = null!;

    [Range(0, double.MaxValue)]
    public double Balance { get; set; }
    public List<Transaction> Transactions { get; set; } = new();
}
