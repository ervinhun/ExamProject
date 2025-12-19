namespace DataAccess.Entities.Game;

public class TicketSubscription
{
    public Guid Id { get; set; }
    public Guid PlayerId { get; set; }
    public Guid GameTemplateId { get; set; }
    public ICollection<PickedNumber> PickedNumbers { get; set; } = new HashSet<PickedNumber>();
    public double Price { get; set; }
    public bool IsExpired { get; set; }
    public DateTime BoughtAt { get; set; }
}