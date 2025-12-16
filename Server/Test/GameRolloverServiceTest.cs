using Api.Dto.Game;
using Api.Services.Game;
using DataAccess;
using DataAccess.Entities.Game;
using DataAccess.Enums;
using Microsoft.EntityFrameworkCore;
using Test.Util;

namespace Test;

[Collection("Database collection")]
public class GameRolloverServiceTests
{
    private readonly MyDbContext _ctx;
    private readonly GameRolloverService _service;
    private readonly Seeder _seeder;

    public GameRolloverServiceTests(DatabaseFixture fixture)
    {
        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseNpgsql(fixture.ConnectionString)
            .Options;

        _ctx = new MyDbContext(options);
        _seeder = new Seeder(_ctx);
        _seeder.Seed().GetAwaiter().GetResult();

        _service = new GameRolloverService(_ctx);
    }

    [Fact]
    public async Task ExecuteAsync_ClosesExpiredGame_AndCreatesNewOne()
    {
        var game = await _ctx.GameInstances
            .SingleAsync(g => g.Id == _seeder.GameInstanceId, cancellationToken: TestContext.Current.CancellationToken);


        game.DrawDayOfWeek = 6;
        game.DrawTimeOfDay = new TimeOnly(17, 0, 0);
        game.DrawDate = DateTime.UtcNow.AddMinutes(-10);
        game.IsAutoRepeatable = true;

        await _ctx.SaveChangesAsync(TestContext.Current.CancellationToken);

        var result = await _service.ExecuteAsync(TestContext.Current.CancellationToken);

        Assert.Equal(1, result.ClosedGames);
        Assert.Equal(1, result.CreatedGames);

        var newGame = await _ctx.GameInstances
            .Where(g => g.Status == GameStatus.Active)
            .SingleAsync(cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(game.GameTemplateId, newGame.GameTemplateId);
        Assert.True(newGame.DrawDate > DateTime.UtcNow);
    }

    [Fact]
    public async Task ExecuteAsync_DoesNothing_WhenNoExpiredGames()
    {
        var result = await _service.ExecuteAsync(TestContext.Current.CancellationToken);

        Assert.Equal(0, result.ClosedGames);
        Assert.Equal(0, result.CreatedGames);
    }
}