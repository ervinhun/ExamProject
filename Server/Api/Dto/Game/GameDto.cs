using DataAccess.Enums;

namespace Api.Dto.Game;

public class CreateGameTemplateRequestDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int PoolOfNumbers { get; set; }
    public string? GameType { get; set; }
    public int MaxWinningNumbers { get; set; }
    public double BasePrice { get; set; }
    public int MinNumbersPerTicket { get; set; }
    public int MaxNumbersPerTicket { get; set; }
}

public class GameTemplateResponseDto : CreateGameTemplateRequestDto
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}



public class GameInstanceDto
{
    public Guid? Id { get; set; }
    public Guid TemplateId { get; set; }
    public GameTemplateResponseDto? Template { get; set; }
    public Guid CreatedById { get; set; }
    public bool IsAutoRepeatable {get; set;}
    public DateTime DrawDate { get; set; }
    public int Participants { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public GameStatus Status { get; set; }
    public int? ExpirationDayOfWeek { get; set; }
    public TimeOnly? ExpirationTimeOfDay { get; set; }
    public ICollection<int> WinningNumbers { get; set; } = new HashSet<int>();
    public int Week { get; set; }
    public bool IsExpired { get; set; }
    public bool IsDrawn { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class UpdateGameDto
{
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public bool? IsClosed { get; set; }
    public DateTime? ClosedAt { get; set; }
}
