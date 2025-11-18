namespace Api.Dto;

public class GameDto
{
    public Guid Id { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public bool? IsClosed { get; set; }
    public int? WinningNumber1 { get; set; }
    public int? WinningNumber2 { get; set; }
    public int? WinningNumber3 { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
}
