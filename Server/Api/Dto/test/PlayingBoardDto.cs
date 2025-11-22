namespace Api.Dto.test;

public class PlayingBoardDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid BoardId { get; set; }
    public Guid GameId { get; set; }
    public string Numbers { get; set; } = null!;
    public int FieldCount { get; set; }
    public decimal Price { get; set; }
    public bool? IsRepeat { get; set; }
    public int? RepeatCountRemaining { get; set; }
    public bool? IsWinningBoard { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreatePlayingBoardDto
{
    public Guid UserId { get; set; }
    public Guid BoardId { get; set; }
    public Guid GameId { get; set; }
    public string Numbers { get; set; } = null!;
    public int FieldCount { get; set; }
    public decimal Price { get; set; }
    public bool? IsRepeat { get; set; }
    public int? RepeatCountRemaining { get; set; }
}

public class UpdatePlayingBoardDto
{
    public Guid? UserId { get; set; }
    public Guid? BoardId { get; set; }
    public Guid? GameId { get; set; }
    public string? Numbers { get; set; }
    public int? FieldCount { get; set; }
    public decimal? Price { get; set; }
    public bool? IsRepeat { get; set; }
    public int? RepeatCountRemaining { get; set; }
    public bool? IsWinningBoard { get; set; }
}
