using Api.Dto.Game;
using DataAccess;
using DataAccess.Entities.Game;
using DataAccess.Enums;
using Microsoft.EntityFrameworkCore;

namespace Api.Services.Game;

public class GameRolloverService
{
    private readonly MyDbContext _ctx;

    public GameRolloverService(MyDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<RolloverResult> ExecuteAsync(CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;

        // Load only active games (cheap DB query)
        var activeGames = await _ctx.GameInstances
            .Where(g => g.Status == GameStatus.Active)
            .ToListAsync(ct);

        int closed = 0;
        int created = 0;

        foreach (var game in activeGames)
        {
            var effectiveDrawDate = ResolveDrawDate(game);

            if (effectiveDrawDate > now)
                continue;

            // Close game
            game.Status = GameStatus.PendingDraw;
            closed++;

            if (!game.IsAutoRepeatable)
                continue;

            // Resolve draw day safely
            var drawDayValue = game.DrawDayOfWeek ?? (int)DayOfWeek.Sunday;

            if (!Enum.IsDefined(typeof(DayOfWeek), drawDayValue))
                throw new ArgumentOutOfRangeException(
                    nameof(game.DrawDayOfWeek),
                    $"Invalid DayOfWeek value: {drawDayValue}");

            var drawDay = (DayOfWeek)drawDayValue;
            var drawTime = (game.DrawTimeOfDay ?? new TimeOnly(0, 0)).ToTimeSpan();

            var nextDrawDate = CalculateNextDrawDate(
                drawDay,
                drawTime,
                effectiveDrawDate);

            _ctx.GameInstances.Add(new GameInstance
            {
                GameTemplateId = game.GameTemplateId,
                Status = GameStatus.Active,
                DrawDate = nextDrawDate,
                DrawDayOfWeek = drawDayValue,
                DrawTimeOfDay = game.DrawTimeOfDay,
                IsAutoRepeatable = true,
                CreatedById = game.CreatedById
            });

            created++;
        }

        if (closed > 0 || created > 0)
            await _ctx.SaveChangesAsync(ct);

        return closed == 0 && created == 0
            ? RolloverResult.Empty
            : new RolloverResult(closed, created);
    }

    private static DateTime CalculateNextDrawDate(
        DayOfWeek drawDay,
        TimeSpan drawTime,
        DateTime fromDate)
    {
        int days =
            ((int)drawDay - (int)fromDate.DayOfWeek + 7) % 7;

        if (days == 0)
            days = 7;

        return fromDate.Date
            .AddDays(days)
            .Add(drawTime);
    }

    private static DateTime ResolveDrawDate(GameInstance game)
    {
        if (game.DrawDate.HasValue)
            return DateTime.SpecifyKind(game.DrawDate.Value, DateTimeKind.Utc);

        var now = DateTime.UtcNow;

        int daysUntilSunday =
            ((int)DayOfWeek.Sunday - (int)now.DayOfWeek + 7) % 7;

        if (daysUntilSunday == 0)
            daysUntilSunday = 7;

        return DateTime.SpecifyKind(
            now.Date.AddDays(daysUntilSunday),
            DateTimeKind.Utc);
    }
}
