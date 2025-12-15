using Api.Dto.Game;
using Api.Services.Management;
using DataAccess;
using DataAccess.Enums;
using Test.Util;
using Utils.Exceptions;

namespace Test;

public class GameManagementServiceTest(MyDbContext ctx, ISeeder seeder, IGameManagementService gameManagementService)
{
    [Fact]
    public async Task GetGameTemplatesReturnsEmptyListWhenNoTemplatesExist()
    {
        ctx.GameTemplates.RemoveRange(ctx.GameTemplates);
        
        var exception = await Record.ExceptionAsync(() => ctx.SaveChangesAsync());
        Assert.Null(exception);
        
        var gameTemplates = await gameManagementService.GetGameTemplatesAsync();
        Assert.Empty(gameTemplates);
        // TODO: Seed the default data back to the gameTemplates
    }

    [Fact]
    public async Task CreateGameTemplateSuccess()
    {
        var createGameTemplateDto = new CreateGameTemplateRequestDto
        {
            Name = "HelloWolrd",
            Description = "No description has been filled out yet",
            PoolOfNumbers = 16,
            GameType = nameof(GameType.Lotto),
            MaxWinningNumbers = 3,
            BasePrice = 20,
            MinNumbersPerTicket = 5,
            MaxNumbersPerTicket = 8
        };

        var exception =
            await Record.ExceptionAsync(() => gameManagementService.CreateGameTemplate(createGameTemplateDto));
        
        Assert.Null(exception);

        var gameTemplates = await gameManagementService.GetGameTemplatesAsync();
        var createdGameTemplate = gameTemplates.FirstOrDefault(t => t.Name == createGameTemplateDto.Name);
        Assert.NotNull(createdGameTemplate);
        Assert.Equal(createGameTemplateDto.Name, createdGameTemplate?.Name);
        Assert.Equal(createGameTemplateDto.Description, createdGameTemplate?.Description);
        Assert.Equal(createGameTemplateDto.PoolOfNumbers, createdGameTemplate?.PoolOfNumbers);
        Assert.Equal(createGameTemplateDto.GameType, createdGameTemplate?.GameType);
        Assert.Equal(createGameTemplateDto.MaxWinningNumbers, createdGameTemplate?.MaxWinningNumbers);
        Assert.Equal(createGameTemplateDto.BasePrice, createdGameTemplate?.BasePrice);
        Assert.Equal(createGameTemplateDto.MinNumbersPerTicket, createdGameTemplate?.MinNumbersPerTicket);
        Assert.Equal(createGameTemplateDto.MaxNumbersPerTicket, createdGameTemplate?.MaxNumbersPerTicket);
        Assert.NotNull(createdGameTemplate?.CreatedAt);
        Assert.NotNull(createdGameTemplate?.UpdatedAt);
    }

    [Fact]
    public async Task CreateGameTemplateThrowsExceptionWhenAttributesAreMissing()
    {
        var createGameTemplateDto = new CreateGameTemplateRequestDto
        {
            Description = "No description has been filled out yet",
            PoolOfNumbers = 16,
            GameType = nameof(GameType.Lotto),
            MaxWinningNumbers = 3,
            BasePrice = 20,
            MinNumbersPerTicket = 5,
            MaxNumbersPerTicket = 8
        };
        await Assert.ThrowsAsync<ServiceException>(() =>
            gameManagementService.CreateGameTemplate(createGameTemplateDto));

        createGameTemplateDto.Name = "HelloWolrd";
        createGameTemplateDto.Description = null;

        await Assert.ThrowsAsync<ServiceException>(() =>
            gameManagementService.CreateGameTemplate(createGameTemplateDto));

        createGameTemplateDto.Description = "No description has been filled out yet";
        createGameTemplateDto.GameType = null;

        await Assert.ThrowsAsync<ServiceException>(() =>
            gameManagementService.CreateGameTemplate(createGameTemplateDto));
        createGameTemplateDto.GameType = nameof(GameType.Lotto);
        createGameTemplateDto.BasePrice = -500;

        await Assert.ThrowsAsync<ServiceException>(() =>
            gameManagementService.CreateGameTemplate(createGameTemplateDto));

        createGameTemplateDto.BasePrice = 0;

        await Assert.ThrowsAsync<ServiceException>(() =>
            gameManagementService.CreateGameTemplate(createGameTemplateDto));
    }

    [Fact]
    public async Task GetGameTemplatesSuccess()
    {
        var gameTemplates = await gameManagementService.GetGameTemplatesAsync();
        Assert.NotEmpty(gameTemplates);
    }

    [Fact]
    public async Task GetGameTemplatesAsyncTest()
    {
        var result = await gameManagementService.GetGameTemplatesAsync();
        Assert.NotEmpty(result);
        Assert.True(result.Count > 1);
    }

    [Fact]
    public async Task GetAllActiveGamesAsyncSuccess()
    {
        var result = await gameManagementService.GetAllActiveGamesAsync();
        Assert.NotEmpty(result);
        Assert.True(result.TrueForAll(g => g.Status == GameStatus.Active));
    }

    [Fact]
    public async Task GetGameTemplateByIdSuccess()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task UpdateGameTemplateByIdSuccess()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task StartGameInstanceSuccess()
    {
        var newGameInstance = new CreateGameTemplateRequestDto
        {
            Name = "HelloWolrdNewVersion",
            Description = "An amazing description",
            PoolOfNumbers = 16,
            GameType = nameof(GameType.Lotto),
            MaxWinningNumbers = 3,
            BasePrice = 60,
            MinNumbersPerTicket = 5,
            MaxNumbersPerTicket = 8
        };

        await gameManagementService.CreateGameTemplate(newGameInstance);
        var GameTemplateResponseDto = GetGameTemplateFromName(newGameInstance.Name);
        var gameInstance = await gameManagementService.GetAllActiveGamesAsync();
        var lastGameInstance = gameInstance.LastOrDefault();

        var exception = await Record.ExceptionAsync(() =>
            gameManagementService.StartGameInstance(lastGameInstance)
        );

        Assert.Null(exception);

        Assert.NotNull(GameTemplateResponseDto);
        Assert.Equal(GameTemplateResponseDto?.Id, lastGameInstance.TemplateId);
        Assert.Equal(GameStatus.Active, lastGameInstance.Status);
    }

    private GameTemplateResponseDto GetGameTemplateFromName(string name)
    {
        var getAllTemplates = gameManagementService.GetGameTemplatesAsync().Result;
        return getAllTemplates.FirstOrDefault(t => t.Name == name);
    }
}