namespace DataAccess.Entities.Game;

public class WinningNumber
{
    public Guid Id { get; set; }
    public required Guid GameInstanceId { get; set; }
    public GameInstance GameInstance { get; set; }
    public int Number { get; set; }
}