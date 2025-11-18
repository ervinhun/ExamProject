namespace Api.Dto;

public class UpdateGameDto
{
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public bool? IsClosed { get; set; }
    public int? WinningNumber1 { get; set; }
    public int? WinningNumber2 { get; set; }
    public int? WinningNumber3 { get; set; }
}
