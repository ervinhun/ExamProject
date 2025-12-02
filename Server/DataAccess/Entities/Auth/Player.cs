using DataAccess.Entities.Finance;
using DataAccess.Entities.Game;

namespace DataAccess.Entities.Auth;

public class Player : User
{
    public Wallet? Wallet { get; set; }
    public ICollection<LotteryTicket> LotteryTickets { get; set; } = new  List<LotteryTicket>();

}