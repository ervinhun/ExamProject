using DataAccess.Entities.Finance;

namespace DataAccess.Entities.Auth;

public class Player : User
{
    public required Wallet Wallet { get; set; }
    public List<Transaction> Transactions { get; set; } = new();
}