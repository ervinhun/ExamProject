using DataAccess.Entities.Auth;

namespace DataAccess.Entities.Game;

public class LotteryTicket
{
    public Guid Id { get; set; }
    public required Guid GameInstanceId { get; set; }
    public GameInstance? GameInstance { get; set; }
    public required Guid PlayerId { get; set; }
    public Player? Player { get; set; }
    public double FullPrice { get; set; }
    public ICollection<PickedNumber> PickedNumbers { get; set; } = new HashSet<PickedNumber>();
    public bool IsWinning { get; set; }
    public bool IsPaid { get; set; }
    public DateTime BoughtAt { get; set; }
}