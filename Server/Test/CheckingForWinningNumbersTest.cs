using Api.Dto.Game;
using DataAccess;
using DataAccess.Entities.Game;
using Microsoft.EntityFrameworkCore;
using Test.Util;

namespace Test;

[Collection("Database collection")]
public class CheckingForWinningNumbersTest
{
    private readonly MyDbContext _ctx;
    private readonly CheckingForWinningNumbers _service;
    private readonly Seeder _seeder;

    public CheckingForWinningNumbersTest(DatabaseFixture fixture)
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
    public async Task Execute_EvaluatesTicketsCorrectly_ForGameInstance()
    {
        await using var tx = await _ctx.Database.BeginTransactionAsync();
        
        // Arrange
        var gameInstanceId = _seeder.GameInstanceId;

        // Winning numbers (3 numbers)
        _ctx.WinningNumbers.AddRange(
            new WinningNumber { GameInstanceId = gameInstanceId, Number = 1 },
            new WinningNumber { GameInstanceId = gameInstanceId, Number = 3 },
            new WinningNumber { GameInstanceId = gameInstanceId, Number = 11 }
        );

        // Losing ticket (only 2 matches)
        _ctx.LotteryTickets.Add(new LotteryTicket
        {
            Id = Guid.NewGuid(),
            PlayerId = _seeder.Player2Id,
            GameInstanceId = gameInstanceId,
            BoughtAt = DateTime.UtcNow,
            PickedNumbers =
            [
                new() { Number = 1 },
                new() { Number = 2 },
                new() { Number = 10 },
                new() { Number = 11 },
                new() { Number = 12 }
            ]
        });

        await _ctx.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        await _service.Execute(gameInstanceId);

        // Assert
        var tickets = await _ctx.LotteryTickets
            .Where(t => t.GameInstanceId == gameInstanceId)
            .Include(t => t.PickedNumbers)
            .ToListAsync(cancellationToken: TestContext.Current.CancellationToken);

        Assert.True(tickets.Count >= 2);

        var winningTicket = tickets.First(t => t.PlayerId == _seeder.Player1Id);
        var losingTicket = tickets.First(t => t.PlayerId == _seeder.Player2Id);

        Assert.True(winningTicket.IsWinning);
        Assert.False(losingTicket.IsWinning);
        await tx.RollbackAsync();
        
    }

    [Fact]
    public async Task Execute_Throws_WhenGameInstanceNotFound()
    {
        await using var tx = await _ctx.Database.BeginTransactionAsync();
        

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.Execute(Guid.NewGuid()));
        await tx.RollbackAsync();
    }

    [Fact]
    public async Task Execute_Throws_WhenGameIsNotActive()
    {
        await using var tx = await _ctx.Database.BeginTransactionAsync();
        
        // Arrange
        var gameInstance = await _ctx.GameInstances
            .FirstAsync(g => g.Id == _seeder.GameInstanceId, cancellationToken: TestContext.Current.CancellationToken);

        gameInstance.Status = DataAccess.Enums.GameStatus.Completed;
        await _ctx.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.Execute(_seeder.GameInstanceId));
        await tx.RollbackAsync();
    }
}
