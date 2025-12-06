namespace DataAccess.Entities.Game;

public class PickedNumber
{
    public Guid Id { get; set; }
    public Guid TicketId { get; set; }
    public LotteryTicket? Ticket { get; set; }
}