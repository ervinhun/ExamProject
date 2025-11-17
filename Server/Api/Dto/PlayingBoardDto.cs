namespace Api.Dto;

public class PlayingBoardDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid BoardId { get; set; }
    public Guid GameId { get; set; }
    public string Numbers { get; set; }
    public int FieldCount { get; set; }
    public int Price { get; set; }
    public bool? IsRepeat { get; set; }
    public int? RepeatCountRemaining { get; set; }
    public bool? IsWinningBoard { get; set; }
    public DateTime CreatedAt { get; set; }
}
