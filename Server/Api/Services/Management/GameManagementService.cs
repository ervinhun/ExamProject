using System.Globalization;
using Api.Dto.Game;
using Api.Dto.test;
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
            
            var newGameTemplate = new GameTemplate
            {
                Name = gameTemplateDto.Name!,
                Description = gameTemplateDto.Description!,
                GameType = Enum.Parse<GameType>(gameTemplateDto.GameType!),
                PoolOfNumbers = gameTemplateDto.PoolOfNumbers,
                MaxWinningNumbers =  gameTemplateDto.MaxWinningNumbers,
                MinNumbersPerTicket = gameTemplateDto.MinNumbersPerTicket,
                MaxNumbersPerTicket =  gameTemplateDto.MaxNumbersPerTicket,
                BasePrice = gameTemplateDto.BasePrice,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = default
            };
            
            await ctx.GameTemplates.AddAsync(newGameTemplate);
            await ctx.SaveChangesAsync();
        }
        catch (Exception e)
        {
            throw new ServiceException(e.Message, e);
        }
    }

    public Task<ICollection<GameTemplateResponseDto>> GetGameTemplates()
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
                    UpdatedAt = gameTemplate.UpdatedAt
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
            var activeGames = ctx.GameInstances.Include(g=> g.GameTemplate).Where(g=>g.Status == GameStatus.Active).ToList();
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
                    UpdatedAt = game.GameTemplate.CreatedAt
                };

                var participants = await ctx.Players
                    .Where(p => p.LotteryTickets.Any(lt => lt.GameInstanceId == game.Id && lt.IsPaid))
                    .CountAsync();
                
                activeGamesDtos.Add(new GameInstanceDto
                {
                    Id = game.Id,
                    CreatedById = game.CreatedById,
                    Template = gameTemplateDto,
                    Status = game.Status,
                    Week = game.Week,
                    Participants = participants,
                    IsAutoRepeatable = game.IsAutoRepeatable,
                    DrawDate = game.DrawDate,
                    ExpirationDate = game.ExpirationDate,
                    ExpirationDayOfWeek = game.ExpirationDayOfWeek,
                    ExpirationTimeOfDay = game.ExpirationTimeOfDay,
                    
                    IsDrawn = game.IsDrawn,
                    CreatedAt = game.CreatedAt,
                    UpdatedAt = game.UpdatedAt
                });
            }
            return await Task.FromResult(activeGamesDtos);
        }catch(Exception e)
        {
            throw new ServiceException(e.Message, e);
        }
    }

    public Task<GameTemplateResponseDto> GetGameTemplateById(Guid gameTemplateId)
    {
        throw new NotImplementedException();
    }
    

    public Task<GameTemplateResponseDto> UpdateGameTemplateById(Guid templateId, CreateGameTemplateRequestDto gameTemplateDto)
    {
        throw new NotImplementedException();
    }

    public Task DeleteGameTemplateById(Guid templateId)
    {
        throw new NotImplementedException();
    }
    

    public async Task StartGameInstance(GameInstanceDto  gameInstanceDto)
    {
        try
        {
            var currentWeek = ISOWeek.GetWeekOfYear(DateTime.Now);

            var gameInstance = new GameInstance
            {
                GameTemplateId = gameInstanceDto.TemplateId,
                IsAutoRepeatable = gameInstanceDto.IsAutoRepeatable,
                DrawDate = DateTimeHelper.ToUtc(gameInstanceDto.DrawDate),
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
                gameInstance.ExpirationDayOfWeek = gameInstanceDto.ExpirationDayOfWeek;
                gameInstance.ExpirationTimeOfDay = gameInstanceDto.ExpirationTimeOfDay;
                gameInstance.ExpirationDate = null;
            }
            else
            {
                gameInstance.ExpirationDayOfWeek = null;
                gameInstance.ExpirationTimeOfDay = null;
                gameInstance.ExpirationDate = gameInstance.ExpirationDate;
            }

            await ctx.GameInstances.AddAsync(gameInstance);
            await ctx.SaveChangesAsync();
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