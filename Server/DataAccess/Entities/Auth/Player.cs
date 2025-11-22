using DataAccess.Entities.Finance;

namespace DataAccess.Entities.Auth;

public class Player : User
{
    public Wallet Wallet { get; set; } = null!;
    public ICollection<Transaction> Transactions { get; set; } = new  List<Transaction>();
    public Boolean Activated { get; set; }
    
}