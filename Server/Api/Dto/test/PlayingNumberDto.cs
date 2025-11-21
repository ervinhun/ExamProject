namespace Api.Dto.test;

public class PlayingNumberDto
{
    public Guid Id { get; set; }
    public Guid PlayingBoardId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreatePlayingNumberDto
{
    public Guid PlayingBoardId { get; set; }
}

public class UpdatePlayingNumberDto
{
    public Guid? PlayingBoardId { get; set; }
}
