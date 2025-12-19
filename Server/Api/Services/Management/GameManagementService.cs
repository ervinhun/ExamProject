using System.Globalization;
using Api.Dto.Game;
using Api.Services.Game;
using DataAccess;
using DataAccess.Entities.Game;
using DataAccess.Enums;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Utils;
using Utils.Exceptions;

namespace Api.Services.Management;

public class GameManagementService(MyDbContext ctx, ITicketService ticketService) : IGameManagementService
{
    public async Task CreateGameTemplate(CreateGameTemplateRequestDto gameTemplateDto)
    {
        try
        {
            if (gameTemplateDto.Name == null) throw new ServiceException("Name cannot be null");
            if (gameTemplateDto.Description == null) throw new ServiceException("Description cannot be null");
            if (gameTemplateDto.GameType == null) throw new ServiceException("Game type cannot be null");
            if (gameTemplateDto.BasePrice <= 0)
                throw new ServiceException("Base price cannot be less or equals than 0");

            var newGameTemplate = new GameTemplate
            {
                Name = gameTemplateDto.Name,
                Description = gameTemplateDto.Description,
                GameType = Enum.Parse<GameType>(gameTemplateDto.GameType),
                PoolOfNumbers = gameTemplateDto.PoolOfNumbers,
                MaxWinningNumbers = gameTemplateDto.MaxWinningNumbers,
                MinNumbersPerTicket = gameTemplateDto.MinNumbersPerTicket,
                MaxNumbersPerTicket = gameTemplateDto.MaxNumbersPerTicket,
                BasePrice = gameTemplateDto.BasePrice,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = default,
            };

            await ctx.GameTemplates.AddAsync(newGameTemplate);
            await ctx.SaveChangesAsync();
        }
        catch (Exception e)
        {
            throw new ServiceException(e.Message, e);
        }
    }

    public Task<ICollection<GameTemplateResponseDto>> GetGameTemplatesAsync()
    {
        try
        {
            var gameTemplates = ctx.GameTemplates.ToListAsync().Result;
            List<GameTemplateResponseDto> gameTemplatesDtos = [];
            foreach (var gameTemplate in gameTemplates)
            {
                gameTemplatesDtos.Add(new GameTemplateResponseDto
                {
                    Id = gameTemplate.Id,
                    Name = gameTemplate.Name,
                    Description = gameTemplate.Description,
                    PoolOfNumbers = gameTemplate.PoolOfNumbers,
                    GameType = gameTemplate.GameType.ToString(),
                    MaxWinningNumbers = gameTemplate.MaxWinningNumbers,
                    BasePrice = gameTemplate.BasePrice,
                    MinNumbersPerTicket = gameTemplate.MinNumbersPerTicket,
                    MaxNumbersPerTicket = gameTemplate.MaxNumbersPerTicket,
                    CreatedAt = gameTemplate.CreatedAt,
                    UpdatedAt = gameTemplate.UpdatedAt,
                });
            }

            return Task.FromResult<ICollection<GameTemplateResponseDto>>(gameTemplatesDtos);
        }
        catch (Exception e)
        {
            throw new ServiceException(e.Message, e);
        }
    }

    public async Task<List<GameInstanceDto>> GetAllActiveGamesAsync()
    {
        try
        {
            var activeGames = ctx.GameInstances
                .Include(g => g.GameTemplate)
                .Where(g => g.Status == GameStatus.Active || g.Status == GameStatus.PendingDraw)
                .ToList();
            var activeGamesDtos = new List<GameInstanceDto>();
            foreach (var game in activeGames)
            {
                var gameTemplateDto = new GameTemplateResponseDto
                {
                    Name = game.GameTemplate!.Name,
                    Description = game.GameTemplate!.Description,
                    PoolOfNumbers = game.GameTemplate!.PoolOfNumbers,
                    GameType = game.GameTemplate.GameType.ToString(),
                    MaxWinningNumbers = game.GameTemplate.MaxWinningNumbers,
                    BasePrice = game.GameTemplate.BasePrice,
                    MinNumbersPerTicket = game.GameTemplate.MinNumbersPerTicket,
                    MaxNumbersPerTicket = game.GameTemplate.MaxNumbersPerTicket,
                    Id = game.GameTemplate.Id,
                    CreatedAt = game.GameTemplate.CreatedAt,
                    UpdatedAt = game.GameTemplate.CreatedAt,
                };

                var participants = await ctx.Players
                    .Where(p => p.LotteryTickets.Any(lt => lt.GameInstanceId == game.Id))
                    .CountAsync();

                var ticketsSold = await ctx.LotteryTickets
                    .Where(lt => lt.GameInstanceId == game.Id)
                    .CountAsync();

                // Calculate prize pool by summing all ticket prices for this game
                var prizePool = await ctx.LotteryTickets
                    .Where(lt => lt.GameInstanceId == game.Id)
                    .SumAsync(lt => lt.FullPrice);
            
                activeGamesDtos.Add(new GameInstanceDto
                {
                    Id = game.Id,
                    CreatedById = game.CreatedById,
                    Template = gameTemplateDto,
                    Status = game.Status,
                    Week = game.Week ?? 0,
                    Year = game.Year ?? 0,
                    Participants = participants,
                    TicketsSold = ticketsSold,
                    PrizePool = prizePool,
                    IsAutoRepeatable = game.IsAutoRepeatable,
                    DrawDate = DateTimeHelper.ToCopenhagen(game.DrawDate),
                    DrawDayOfWeek = game.DrawDayOfWeek,
                    DrawTimeOfDay = game.DrawTimeOfDay,
                    IsDrawn = game.IsDrawn,
                    CreatedAt = game.CreatedAt,
                    UpdatedAt = game.UpdatedAt,
                });
            }

            return await Task.FromResult(activeGamesDtos);
        }
        catch (Exception e)
        {
            throw new ServiceException(e.Message, e);
        }
    }

    public Task<GameTemplateResponseDto> GetGameTemplateById(Guid gameTemplateId)
    {
        throw new NotImplementedException();
    }


    public Task<GameTemplateResponseDto> UpdateGameTemplateById(Guid templateId,
        CreateGameTemplateRequestDto gameTemplateDto)
    {
        throw new NotImplementedException();
    }

    public Task DeleteGameTemplateById(Guid templateId)
    {
        throw new NotImplementedException();
    }


    public async Task StartGameInstance(GameInstanceDto gameInstanceDto)
    {
        try
        {
            // Only validate draw date for non-auto-repeatable games
            if (!gameInstanceDto.IsAutoRepeatable && gameInstanceDto.DrawDate.HasValue && gameInstanceDto.DrawDate < DateTime.Now)
            {
                throw new ServiceException("Draw date cannot be in the past");
            }

            // For auto-repeatable games, use the week from DTO (for next week calculation)
            // For manual games, calculate current week if not provided
            var weekNumber = gameInstanceDto.IsAutoRepeatable && gameInstanceDto.Week > 0 
                ? gameInstanceDto.Week 
                : ISOWeek.GetWeekOfYear(DateTime.Now);
            
            var year = gameInstanceDto.Year > 0 ? gameInstanceDto.Year : ISOWeek.GetYear(DateTime.Now);

            // Convert DrawDate to UTC if it has Unspecified kind
            DateTime? drawDateUtc = null;
            if (!gameInstanceDto.IsAutoRepeatable && gameInstanceDto.DrawDate.HasValue)
            {
                var drawDate = gameInstanceDto.DrawDate.Value;
                drawDateUtc = drawDate.Kind == DateTimeKind.Unspecified 
                    ? DateTime.SpecifyKind(drawDate, DateTimeKind.Utc) 
                    : drawDate.ToUniversalTime();
            }

            var gameInstance = new GameInstance
            {
                GameTemplateId = gameInstanceDto.TemplateId,
                IsAutoRepeatable = gameInstanceDto.IsAutoRepeatable,
                Status = GameStatus.Active,
                CreatedById = gameInstanceDto.CreatedById,
                Week = weekNumber,
                Year = year,
                IsExpired = false,
                IsDrawn = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = default
            };
            if (gameInstanceDto.IsAutoRepeatable)
            {
                gameInstance.DrawDayOfWeek = gameInstanceDto.DrawDayOfWeek;
                gameInstance.DrawTimeOfDay = gameInstanceDto.DrawTimeOfDay;
                gameInstance.DrawDate = null;
            }
            else
            {
                gameInstance.DrawDayOfWeek = null;
                gameInstance.DrawTimeOfDay = null;
                gameInstance.DrawDate = drawDateUtc;
            }

            await ctx.GameInstances.AddAsync(gameInstance);
            await ctx.SaveChangesAsync();
        }
        catch (Exception e)
        {
            throw new ServiceException(e.Message, e);
        }
    }

    public async Task DrawWinningNumbersForGameInstance(DrawWinningNumbersDto drawWinningNumbersDto)
    {
        try
        {
            var game = await ctx.GameInstances.Include(gi => gi.GameTemplate).FirstOrDefaultAsync(gi => gi.Id == drawWinningNumbersDto.GameInstanceId);
            if (game == null) throw new ServiceException("Game instance not found");
            if (game.GameTemplate!.MaxWinningNumbers != drawWinningNumbersDto.WinningNumbers.Length) throw new ServiceException("Number of winning numbers do not match");
            if (game.IsExpired) throw new ServiceException("Game expired");
            if (game.IsDrawn) throw new ServiceException("Game drawn");
            
            // Validate draw timing
            var now = DateTime.UtcNow;
            
            if (game.IsAutoRepeatable)
            {
                // For auto-repeatable games, validate week, year, day of week and time
                if (game.Week.HasValue && game.Year.HasValue && game.DrawDayOfWeek.HasValue)
                {
                    var currentWeek = ISOWeek.GetWeekOfYear(now);
                    var currentYear = ISOWeek.GetYear(now);
                    var currentDayOfWeek = (int)now.DayOfWeek;
                    var currentTimeOfDay = TimeOnly.FromDateTime(now);
                    
                    // Check if current week/year is before scheduled week/year
                    var isBeforeScheduledWeek = currentYear < game.Year.Value || 
                                                  (currentYear == game.Year.Value && currentWeek < game.Week.Value);
                    
                    if (isBeforeScheduledWeek)
                    {
                        throw new ServiceException($"Cannot draw winning numbers yet. Game is scheduled for {game.Year.Value}-W{game.Week.Value:D2} but current week is {currentYear}-W{currentWeek:D2}.");
                    }
                    
                    // If we're in the correct week, check day and time
                    if (currentYear == game.Year.Value && currentWeek == game.Week.Value)
                    {
                        if (currentDayOfWeek < game.DrawDayOfWeek.Value)
                        {
                            throw new ServiceException($"Cannot draw winning numbers yet. Game is scheduled for {(DayOfWeek)game.DrawDayOfWeek.Value} but today is {(DayOfWeek)currentDayOfWeek}.");
                        }
                        
                        if (currentDayOfWeek == game.DrawDayOfWeek.Value && game.DrawTimeOfDay.HasValue)
                        {
                            if (currentTimeOfDay < game.DrawTimeOfDay.Value)
                            {
                                throw new ServiceException($"Cannot draw winning numbers yet. Game is scheduled for {game.DrawTimeOfDay.Value:HH:mm} but current time is {currentTimeOfDay:HH:mm}.");
                            }
                        }
                    }
                }
            }
            else
            {
                // For one-time games, check if draw date/time is in the future
                if (game.DrawDate.HasValue && game.DrawDate.Value > DateTimeHelper.ToCopenhagen(now))
                {
                    throw new ServiceException($"Cannot draw winning numbers yet. Game is scheduled for {game.DrawDate.Value:yyyy-MM-dd HH:mm} UTC but current time is {now:yyyy-MM-dd HH:mm} UTC.");
                }
            }
            
            var winningNumbersList = drawWinningNumbersDto.WinningNumbers.ToList();
            
            foreach (var winningNumber in winningNumbersList)
            {
                game.WinningNumbers.Add(new  WinningNumber
                {
                    GameInstanceId = game.Id,
                    Number = winningNumber
                });
            }
            
            game.IsDrawn = true;
            game.IsExpired = true;
            game.Status = GameStatus.Completed;
            await ctx.SaveChangesAsync();
            
            var ticketsForGame = await ctx.LotteryTickets
                .Include(lt => lt.PickedNumbers)
                .Where(lt => lt.GameInstanceId == game.Id)
                .ToListAsync();
            
            foreach (var ticket in ticketsForGame)
            {
                var pickedNumbers = ticket.PickedNumbers.Select(pn=>pn.Number).ToList();
                var matchingNumbers = pickedNumbers.Intersect(winningNumbersList).Count();
                if (matchingNumbers == game.GameTemplate.MaxWinningNumbers)
                {
                    ticket.IsWinning = true;
                }

                ticket.IsExpired = true;
            }
            await ctx.SaveChangesAsync();
            
            if (game.IsAutoRepeatable)
            {
                var nextWeek = game.Week!.Value + 1;
                var maxWeeksInYear = ISOWeek.GetWeeksInYear(DateTime.UtcNow.Year);
                var nextYear = game.Year!.Value;
                
                if (nextWeek > maxWeeksInYear)
                {
                    nextWeek = 1;
                    nextYear = game.Year!.Value + 1;
                }
                
                var newGameInstanceDto = new GameInstanceDto
                {
                    TemplateId = game.GameTemplateId,
                    IsAutoRepeatable = true,
                    Status = GameStatus.Active,
                    Week = nextWeek,
                    Year = nextYear,
                    DrawDayOfWeek = game.DrawDayOfWeek,
                    DrawTimeOfDay = game.DrawTimeOfDay,
                    DrawDate = game.DrawDate,
                    WinningNumbers = [],
                    IsExpired = false,
                    IsDrawn = false,
                    CreatedAt = DateTime.UtcNow,
                };
                
                await StartGameInstance(newGameInstanceDto);
                
                // Get the newly created game instance
                var newGameInstance = await ctx.GameInstances
                    .FirstOrDefaultAsync(gi => gi.GameTemplateId == game.GameTemplateId && gi.Status == GameStatus.Active);
                
                if (newGameInstance != null)
                {
                    // Purchase tickets for all active subscriptions using ticket service
                    await ticketService.PurchaseTicketsForActiveSubscriptions(game.GameTemplateId, newGameInstance.Id);
                }
            }
        }
        catch (Exception e)
        {
            throw new ServiceException(e.Message, e);
        }
    }

    public async Task<List<GameInstanceDto>> GetAllGames()
    {
        try
        {
            var games = await ctx.GameInstances
                .Include(gi => gi.GameTemplate)
                .Include(gi => gi.WinningNumbers)
                .AsNoTracking()
                .ToListAsync();
            var activeGamesDtos = new List<GameInstanceDto>();
            foreach (var game in games)
            {
                var gameTemplateDto = new GameTemplateResponseDto
                {
                    Name = game.GameTemplate!.Name,
                    Description = game.GameTemplate!.Description,
                    PoolOfNumbers = game.GameTemplate!.PoolOfNumbers,
                    GameType = game.GameTemplate.GameType.ToString(),
                    MaxWinningNumbers = game.GameTemplate.MaxWinningNumbers,
                    BasePrice = game.GameTemplate.BasePrice,
                    MinNumbersPerTicket = game.GameTemplate.MinNumbersPerTicket,
                    MaxNumbersPerTicket = game.GameTemplate.MaxNumbersPerTicket,
                    Id = game.GameTemplate.Id,
                    CreatedAt = game.GameTemplate.CreatedAt,
                    UpdatedAt = game.GameTemplate.CreatedAt,
                };

                // Calculate winning tickets count
                var ticketsWon = await ctx.LotteryTickets
                    .Where(lt => lt.GameInstanceId == game.Id && lt.IsWinning)
                    .CountAsync();
                
                activeGamesDtos.Add(new GameInstanceDto
                {
                    Id = game.Id,
                    CreatedById = game.CreatedById,
                    Template = gameTemplateDto,
                    Status = game.Status,
                    Week = (int)game.Week!,
                    Year = (int)game.Year!,
                    IsAutoRepeatable = game.IsAutoRepeatable,
                    DrawDate = DateTimeHelper.ToCopenhagen(game.DrawDate),
                    DrawDayOfWeek = game.DrawDayOfWeek,
                    DrawTimeOfDay = game.DrawTimeOfDay,
                    IsDrawn = game.IsDrawn,
                    TicketsWon = ticketsWon,
                    WinningNumbers = game.WinningNumbers.Select(wn => wn.Number).ToList(),
                    CreatedAt = game.CreatedAt,
                    UpdatedAt = game.UpdatedAt,
                });
            }
            
            return activeGamesDtos;
        }
        catch (Exception e)
        {
            throw new ServiceException(e.Message, e);
        }
    }

    public Task StopGameInstanceById(Guid gameInstanceId)
    {
        throw new NotImplementedException();
    }
}