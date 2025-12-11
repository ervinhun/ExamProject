namespace DataAccess.Entities.Auth;

public class PlayerWhoApplied
{
    public Guid id { get; set; }
    
    public Guid playerId { get; set; }
    
    public Player Player { get; set; }
    
    public String status { get; set; }
    public DateTime createdAt { get; set; }
    public DateTime updatedAt { get; set; }
    public Guid? reviewedBy { get; set; }
}