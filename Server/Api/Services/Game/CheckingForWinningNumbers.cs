using DataAccess;
using DataAccess.Enums;
using Microsoft.EntityFrameworkCore;

namespace Api.Dto.Game;

public class CheckingForWinningNumbers(
    MyDbContext ctx)
{
    public async Task Execute(Guid gameInstanceId)
    {
        // Load game instance with template and winning numbers
        var gameInstance = await ctx.GameInstances
            .Include(g => g.GameTemplate)
            .Include(g => g.WinningNumbers)
            .FirstOrDefaultAsync(g => g.Id == gameInstanceId);

        if (gameInstance is null)
            throw new InvalidOperationException("GameInstance not found");
        
        if (gameInstance.WinningNumbers.Count < gameInstance.GameTemplate.MaxWinningNumbers)
            throw new InvalidOperationException("Winning numbers not loaded");

        if (gameInstance.Status != GameStatus.Active)
            throw new InvalidOperationException("Game is not active");

        var requiredMatches = gameInstance.GameTemplate.MaxWinningNumbers;

        var winningNumbers = gameInstance.WinningNumbers
            .Select(w => w.Number)
            .ToHashSet();

        // Load all tickets for this game instance
        var tickets = await ctx.LotteryTickets
            .Include(t => t.PickedNumbers)
            .Where(t => t.GameInstanceId == gameInstanceId)
            .ToListAsync();

        foreach (var ticket in tickets)
        {
            var matchedCount = ticket.PickedNumbers
                .Count(pn => winningNumbers.Contains(pn.Number));

            ticket.IsWinning = matchedCount >= requiredMatches;
        }

        await ctx.SaveChangesAsync();
    }
}