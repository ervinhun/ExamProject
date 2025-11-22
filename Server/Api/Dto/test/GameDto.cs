namespace Api.Dto.test;

public class CreateGameTemplateRequestDto
{
    public uint ExpirationDayOfWeek { get; set; }
    public TimeOnly ExpirationTime { get; set; }
    public uint NumbersRangeMax { get; set; }
    public uint NumbersRangeMin { get; set; }
    public uint MinPickedNumbers { get; set; }
    public uint MaxPickedNumbers { get; set; }
    public double BasePrice { get; set; }
    public uint Multiplier { get; set; }
}

public class GameTemplateResponseDto : CreateGameTemplateRequestDto
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}



public class GameInstanceDto
{
    public Guid Id { get; set; }
    public Guid GameTemplateId { get; set; }
    public Guid AdminId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int WeekNumber { get; set; }
    public bool IsExpired { get; set; }
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
