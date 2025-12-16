using DataAccess;
using DataAccess.Entities.Game;
using DataAccess.Enums;
using Microsoft.EntityFrameworkCore;

namespace Api.Dto.Game;

public class GameRolloverService
{
    private readonly MyDbContext _ctx;

    public GameRolloverService(MyDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<RolloverResult> ExecuteAsync()
    {
        var now = DateTime.UtcNow;

        var expiredGames = await _ctx.GameInstances
            .Where(g =>
                g.Status == GameStatus.Active &&
                g.DrawDate <= now)
            .ToListAsync();

        if (expiredGames.Count == 0)
            return RolloverResult.Empty;

        var closed = 0;
        var created = 0;

        foreach (var game in expiredGames)
        {
            game.Status = GameStatus.PendingDraw;
            closed++;

            if (!game.IsAutoRepeatable)
                continue;
            var drawDay = game.DrawDayOfWeek ?? 1;

            if (!Enum.IsDefined(typeof(DayOfWeek), game.DrawDayOfWeek))
                throw new ArgumentOutOfRangeException(nameof(game.DrawDayOfWeek), "Invalid DayOfWeek value");

            DayOfWeek dayOfWeek = (DayOfWeek)game.DrawDayOfWeek;
            TimeSpan timeSpan = (game.DrawTimeOfDay ?? new TimeOnly(0, 0)).ToTimeSpan();


            var nextDrawDate = CalculateNextDrawDate(
                dayOfWeek,
                timeSpan,
                ResolveDrawDate(game.DrawDate));

            _ctx.GameInstances.Add(new GameInstance
            {
                GameTemplateId = game.GameTemplateId,
                Status = GameStatus.Active,
                DrawDate = nextDrawDate,
                DrawDayOfWeek = game.DrawDayOfWeek,
                DrawTimeOfDay = game.DrawTimeOfDay,
                IsAutoRepeatable = true,
                CreatedById = game.CreatedById
            });

            created++;
        }

        await _ctx.SaveChangesAsync();

        return new RolloverResult(closed, created);
    }

    private static DateTime CalculateNextDrawDate(
        DayOfWeek drawDay,
        TimeSpan drawTime,
        DateTime fromDate)
    {
        var next = fromDate.Date.AddDays(1);

        while (next.DayOfWeek != drawDay)
            next = next.AddDays(1);

        return next.Add(drawTime);
    }


    public static DateTime ResolveDrawDate(DateTime? drawDate)
    {
        if (drawDate.HasValue)
            return DateTime.SpecifyKind(drawDate.Value, DateTimeKind.Utc);

        var now = DateTime.UtcNow;

        // Sunday = 0
        int daysUntilSunday = ((int)DayOfWeek.Sunday - (int)now.DayOfWeek + 7) % 7;

        // If today is Sunday, we want NEXT Sunday
        if (daysUntilSunday == 0)
            daysUntilSunday = 7;

        var nextSunday = now.Date.AddDays(daysUntilSunday);

        return DateTime.SpecifyKind(nextSunday, DateTimeKind.Utc);
    }
}