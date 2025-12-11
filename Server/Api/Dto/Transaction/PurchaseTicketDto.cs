namespace Api.Dto.Transaction;

public class PurchaseTicketDto
{
    public Guid GameInstanceId{ get; set; }
    public Guid PlayerId{ get; set; }
    public ICollection<int> PickedNumbers{ get; set; } = new HashSet<int>(); 
}