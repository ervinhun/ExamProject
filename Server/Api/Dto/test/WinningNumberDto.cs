namespace Api.Dto.test;

public class WinningNumberDto
{
    public Guid Id { get; set; }
    public Guid GameId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateWinningNumberDto
{
    public Guid GameId { get; set; }
}

public class UpdateWinningNumberDto
{
    public Guid? GameId { get; set; }
}
