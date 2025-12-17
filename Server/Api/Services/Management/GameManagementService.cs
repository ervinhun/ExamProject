using System.Globalization;
using Api.Dto.Game;
using DataAccess;
using DataAccess.Entities.Game;
using DataAccess.Enums;
using Microsoft.EntityFrameworkCore;
using Utils;
using Utils.Exceptions;

namespace Api.Services.Management;

public class GameManagementService(MyDbContext ctx) : IGameManagementService
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
                .Where(g => g.Status == GameStatus.Active)
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
                    Week = game.Week,
                    Participants = participants,
                    TicketsSold = ticketsSold,
                    PrizePool = prizePool,
                    IsAutoRepeatable = game.IsAutoRepeatable,
                    DrawDate = game.DrawDate,
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
        
        // TODO: Implement test when method is done
    }


    public Task<GameTemplateResponseDto> UpdateGameTemplateById(Guid templateId,
        CreateGameTemplateRequestDto gameTemplateDto)
    {
        throw new NotImplementedException();
        // TODO: Implement test when method is done
    }

    public Task DeleteGameTemplateById(Guid templateId)
    {
        throw new NotImplementedException();
        // TODO: Implement test when method is done
    }


    public async Task StartGameInstance(GameInstanceDto gameInstanceDto)
    {
        try
        {
            if (gameInstanceDto.DrawDate != null && gameInstanceDto.DrawDate < DateTime.Now) throw new ServiceException("Draw date cannot be in the past");

            var currentWeek = ISOWeek.GetWeekOfYear(DateTime.Now);

            var gameInstance = new GameInstance
            {
                GameTemplateId = gameInstanceDto.TemplateId,
                IsAutoRepeatable = gameInstanceDto.IsAutoRepeatable,
                Status = GameStatus.Active,
                CreatedById = gameInstanceDto.CreatedById,
                Week = currentWeek,
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
                gameInstance.DrawDate = gameInstance.DrawDate;
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
            
            // Validate draw date/day
            var today = DateTime.UtcNow.Date;
            var todayDayOfWeek = (int)DateTime.UtcNow.DayOfWeek;
            
            if (game.IsAutoRepeatable)
            {
                // For auto-repeatable games, check if draw day of week matches today
                if (game.DrawDayOfWeek.HasValue && game.DrawDayOfWeek.Value != todayDayOfWeek)
                {
                    var scheduledDay = (DayOfWeek)game.DrawDayOfWeek.Value;
                    var currentDay = DateTime.UtcNow.DayOfWeek;
                    throw new ServiceException($"Cannot draw winning numbers. Game draw is scheduled for {scheduledDay} but today is {currentDay}");
                }
            }
            else
            {
                // For one-time games, check if draw date is today
                if (game.DrawDate.HasValue && game.DrawDate.Value.Date != today)
                {
                    throw new ServiceException($"Cannot draw winning numbers. Game is scheduled for {game.DrawDate.Value.Date:yyyy-MM-dd} but today is {today:yyyy-MM-dd}");
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
                var nextWeek = game.Week + 1;
                var maxWeeksInYear = ISOWeek.GetWeeksInYear(DateTime.UtcNow.Year);
                
                if (nextWeek > maxWeeksInYear)
                {
                    nextWeek = 1;
                }
                
                await StartGameInstance(new GameInstanceDto
                {
                    TemplateId = game.GameTemplateId,
                    IsAutoRepeatable = true,
                    Status = GameStatus.Active,
                    Week = nextWeek,
                    DrawDayOfWeek = game.DrawDayOfWeek,
                    DrawTimeOfDay = game.DrawTimeOfDay,
                    DrawDate = game.DrawDate,
                    WinningNumbers = [],
                    IsExpired = false,
                    IsDrawn = false,
                    CreatedAt = DateTime.UtcNow,
                });
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<List<GameInstanceDto>> GetAllGames()
    {
        try
        {
            var games = await ctx.GameInstances.Include(gi => gi.GameTemplate).AsNoTracking().ToListAsync();
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
                
                activeGamesDtos.Add(new GameInstanceDto
                {
                    Id = game.Id,
                    CreatedById = game.CreatedById,
                    Template = gameTemplateDto,
                    Status = game.Status,
                    Week = game.Week,
                    IsAutoRepeatable = game.IsAutoRepeatable,
                    DrawDate = game.DrawDate,
                    DrawDayOfWeek = game.DrawDayOfWeek,
                    DrawTimeOfDay = game.DrawTimeOfDay,
                    IsDrawn = game.IsDrawn,
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