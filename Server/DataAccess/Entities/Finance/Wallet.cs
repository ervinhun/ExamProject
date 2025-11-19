using DataAccess.Entities.Auth;

namespace DataAccess.Entities.Finance;

public class Wallet
{
    public Guid Id { get; set; }
    public required Guid PlayerId { get; set; }
    public Player Player { get; set; } = null!;
    public required UInt32 Balance { get; set; }
}
