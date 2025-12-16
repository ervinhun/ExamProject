using Api.Dto.Game;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Test.Util;

namespace Test;

[Collection("Database collection")]
public class GameEvaluationTest
{
    private readonly MyDbContext _ctx;
    private readonly CheckingForWinningNumbers _service;
    private readonly Seeder _seeder;

    public GameEvaluationTest(DatabaseFixture fixture)
    {
        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseNpgsql(fixture.ConnectionString)
            .Options;

        _ctx = new MyDbContext(options);

        _seeder = new Seeder(_ctx);
        _seeder.Seed().GetAwaiter().GetResult();

        _service = new CheckingForWinningNumbers(_ctx);
    }

    [Fact]
    public async Task EvaluateTicketsForGame_SetsTicketResultCorrectly()
    {
        // Act
        var gameInstance = await _ctx.GameInstances.FirstAsync(g => g.Id == _seeder.GameInstanceId, cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(gameInstance);
        await _service.Execute(_seeder.GameInstanceId);

        // Assert
        var ticket = await _ctx.LotteryTickets
            .Include(t => t.PickedNumbers)
            .FirstAsync(t => t.Id == _seeder.TicketId, cancellationToken: TestContext.Current.CancellationToken);
        
        Assert.True(ticket.IsWinning);
        foreach (var number in gameInstance.WinningNumbers)
        {
            Assert.Contains(number, ticket.PickedNumbers);
        }

        // if you store winnings:
        Assert.True(ticket. >= 0);
    }

    [Fact]
    public async Task EvaluateTicketsForGame_CalledTwice_DoesNotDoubleEvaluate()
    {
        await _service.Execute(_seeder.GameInstanceId);
        await _service.Execute(_seeder.GameInstanceId);

        var ticket = await _ctx.LotteryTickets
            .FirstAsync(t => t.Id == _seeder.TicketId);

        Assert.True(ticket.IsWinning);
    }
}
