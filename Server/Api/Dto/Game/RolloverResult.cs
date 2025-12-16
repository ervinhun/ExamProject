namespace Api.Dto.Game;

public readonly record struct RolloverResult(
    int ClosedGames,
    int CreatedGames)
{
    public static RolloverResult Empty => new(0, 0);
}