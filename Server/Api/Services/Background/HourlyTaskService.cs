using Api.Services.Management;
using Microsoft.EntityFrameworkCore;
using Utils;

namespace Api.Services.Background;

public class HourlyTaskService(
    ILogger<HourlyTaskService> logger,
    IServiceScopeFactory serviceScopeFactory)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Hourly Task Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Calculate delay until next hour
                var now = DateTime.UtcNow;
                var nextHour = now.Date.AddHours(now.Hour + 1);
                var delay = nextHour - now;

                var nextHourLocal = DateTimeHelper.ToCopenhagen(nextHour);
                
                logger.LogInformation("Next task execution scheduled at {nextHourLocal} (in {Minutes} minutes)", 
                    nextHourLocal, delay.TotalMinutes);

                // Wait until next hour
                await Task.Delay(delay, stoppingToken);

                if (!stoppingToken.IsCancellationRequested)
                {
                    await ExecuteHourlyTask();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in hourly task service");
                // Wait 5 minutes before retrying on error
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }

    private async Task ExecuteHourlyTask()
    {
        logger.LogInformation("Starting hourly task execution at {Time}", DateTime.UtcNow);

        try
        {
            using var scope = serviceScopeFactory.CreateScope();
            
            // Check for games that need to be marked as PendingDraw
            await CheckAndUpdateGameStatusToPendingDraw(scope);
            
            logger.LogInformation("Hourly task completed successfully at {Time}", DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error executing hourly task");
        }
    }

    private async Task CheckAndUpdateGameStatusToPendingDraw(IServiceScope scope)
    {
        try
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<DataAccess.MyDbContext>();
            var now = DateTime.UtcNow;
            var currentDayOfWeek = (int)now.DayOfWeek;
            var currentTimeOfDay = TimeOnly.FromDateTime(now);
            var currentWeek = System.Globalization.ISOWeek.GetWeekOfYear(now);
            var currentYear = System.Globalization.ISOWeek.GetYear(now);
            
            logger.LogInformation("Checking games for pending draw status. Current time: {Time}, Day: {Day}, Week: {Week}, Year: {Year}", 
                now, now.DayOfWeek, currentWeek, currentYear);

            // Get all active games
            var activeGames = await dbContext.GameInstances
                .Include(g => g.GameTemplate)
                .Where(g => g.Status == DataAccess.Enums.GameStatus.Active && !g.IsDrawn)
                .ToListAsync();

            logger.LogInformation("Found {Count} active games to check", activeGames.Count);

            var gamesMarkedPending = 0;

            foreach (var game in activeGames)
            {
                var shouldMarkPending = false;
                var reason = "";

                if (game.IsAutoRepeatable)
                {
                    // For auto-repeatable games, check week number, day of week and time of day
                    if (game.Week.HasValue && game.Year.HasValue && game.DrawDayOfWeek.HasValue)
                    {
                        // Check if we're past the game's scheduled week/year
                        var isPastScheduledWeek = currentYear > game.Year.Value || 
                                                   (currentYear == game.Year.Value && currentWeek > game.Week.Value);
                        
                        // Check if we're in the same week/year
                        var isSameWeek = currentYear == game.Year.Value && currentWeek == game.Week.Value;
                        
                        if (isPastScheduledWeek)
                        {
                            shouldMarkPending = true;
                            reason = $"Current week {currentYear}-W{currentWeek:D2} is past game week {game.Year.Value}-W{game.Week.Value:D2}";
                        }
                        else if (isSameWeek)
                        {
                            // We're in the correct week, now check day and time
                            if (currentDayOfWeek > game.DrawDayOfWeek.Value)
                            {
                                shouldMarkPending = true;
                                reason = $"Current day ({(DayOfWeek)currentDayOfWeek}) is past draw day ({(DayOfWeek)game.DrawDayOfWeek.Value}) in week {currentWeek}";
                            }
                            else if (currentDayOfWeek == game.DrawDayOfWeek.Value && game.DrawTimeOfDay.HasValue)
                            {
                                if (currentTimeOfDay >= game.DrawTimeOfDay.Value)
                                {
                                    shouldMarkPending = true;
                                    reason = $@"Current time ({currentTimeOfDay:hh\:mm}) is past draw time ({game.DrawTimeOfDay.Value:hh\:mm}) in week {currentWeek}";
                                }
                            }
                        }
                    }
                }
                else
                {
                    // For one-time games, check draw date (DateTime includes both date and time)
                    if (game.DrawDate.HasValue)
                    {
                        if (game.DrawDate.Value <= now)
                        {
                            shouldMarkPending = true;
                            reason = $"Draw date and time ({game.DrawDate.Value:yyyy-MM-dd HH:mm}) has passed";
                        }
                    }
                }

                if (shouldMarkPending)
                {
                    game.Status = DataAccess.Enums.GameStatus.PendingDraw;
                    gamesMarkedPending++;
                    
                    logger.LogInformation(
                        "Game {GameId} (Week {Week}, Template: {TemplateId}) marked as PendingDraw. Reason: {Reason}",
                        game.Id, game.Week, game.GameTemplateId, reason);
                }
            }

            if (gamesMarkedPending > 0)
            {
                await dbContext.SaveChangesAsync();
                logger.LogInformation("Successfully marked {Count} games as PendingDraw", gamesMarkedPending);
            }
            else
            {
                logger.LogInformation("No games needed to be marked as PendingDraw");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking games for pending draw status");
            throw;
        }
    }
}
