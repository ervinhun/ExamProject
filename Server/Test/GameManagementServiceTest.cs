using Api.Dto.Game;
using Api.Services.Management;
using DataAccess;
using DataAccess.Enums;
using Microsoft.EntityFrameworkCore;
using Test.Util;
using Utils.Exceptions;

namespace Test;

[Collection("Database collection")]
public class GameManagementServiceTest
{
    private readonly MyDbContext _ctx;
    private readonly GameManagementService _gameService;
    private readonly DatabaseFixture _fixture;
    private readonly Seeder _seeder;

    public GameManagementServiceTest(DatabaseFixture fixture)
    {
        _fixture = fixture;

        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseNpgsql(fixture.ConnectionString)
            .Options;

        _ctx = new MyDbContext(options);

        //var seeder = new Seeder(_ctx);
        _seeder = new Seeder(_ctx);
        _seeder.Seed().GetAwaiter().GetResult();
        _gameService = new GameManagementService(_ctx);
    }

    [Fact]
    public async Task CreateGameTemplateSuccess()
    {
        var createGameTemplateDto = new CreateGameTemplateRequestDto
        {
            Name = "HelloWorld",
            Description = "No description has been filled out yet",
            PoolOfNumbers = 16,
            GameType = nameof(GameType.Lotto),
            MaxWinningNumbers = 3,
            BasePrice = 20,
            MinNumbersPerTicket = 5,
            MaxNumbersPerTicket = 8
        };

        var exception =
            await Record.ExceptionAsync(() => _gameService.CreateGameTemplate(createGameTemplateDto));
        
        Assert.Null(exception);

        var gameTemplates = await _gameService.GetGameTemplatesAsync();
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
            _gameService.CreateGameTemplate(createGameTemplateDto));

        createGameTemplateDto.Name = "HelloWorld";
        createGameTemplateDto.Description = null;

        await Assert.ThrowsAsync<ServiceException>(() =>
            _gameService.CreateGameTemplate(createGameTemplateDto));

        createGameTemplateDto.Description = "No description has been filled out yet";
        createGameTemplateDto.GameType = null;

        await Assert.ThrowsAsync<ServiceException>(() =>
            _gameService.CreateGameTemplate(createGameTemplateDto));
        createGameTemplateDto.GameType = nameof(GameType.Lotto);
        createGameTemplateDto.BasePrice = -500;

        await Assert.ThrowsAsync<ServiceException>(() =>
            _gameService.CreateGameTemplate(createGameTemplateDto));

        createGameTemplateDto.BasePrice = 0;

        await Assert.ThrowsAsync<ServiceException>(() =>
            _gameService.CreateGameTemplate(createGameTemplateDto));
    }

    [Fact]
    public async Task GetGameTemplatesSuccess()
    {
        var gameTemplates = await _gameService.GetGameTemplatesAsync();
        Assert.NotEmpty(gameTemplates);
    }

    [Fact]
    public async Task GetGameTemplatesAsyncTest()
    {
        var result = await _gameService.GetGameTemplatesAsync();
        Assert.NotEmpty(result);
        Assert.True(result.Count > 1);
    }

    [Fact]
    public async Task GetAllActiveGamesAsyncSuccess()
    {
        var result = await _gameService.GetAllActiveGamesAsync();
        Assert.NotEmpty(result);
        Assert.True(result.TrueForAll(g => g.Status == GameStatus.Active));
    }

    [Fact(Skip = "Not implemented yet")]
    public async Task GetGameTemplateByIdSuccess()
    {
        throw new NotImplementedException();
    }

    [Fact(Skip = "Not implemented yet")]
    public async Task UpdateGameTemplateByIdSuccess()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task StartGameInstanceSuccess()
    {
        var gameTemplate = new CreateGameTemplateRequestDto
        {
            Name = "HelloWorldNewVersion",
            Description = "An amazing description",
            PoolOfNumbers = 16,
            GameType = nameof(GameType.Lotto),
            MaxWinningNumbers = 3,
            BasePrice = 60,
            MinNumbersPerTicket = 5,
            MaxNumbersPerTicket = 8
        };

        await _gameService.CreateGameTemplate(gameTemplate);

        var gameTemplateResponse = await _ctx.GameTemplates
            .FirstOrDefaultAsync(t => t.Name == gameTemplate.Name);

        Assert.NotNull(gameTemplateResponse);

        var gameInstanceDto = new GameInstanceDto
        {
            TemplateId = gameTemplateResponse.Id,
            CreatedById = _seeder.AdminId,
            IsAutoRepeatable = true,
            DrawDayOfWeek = 6,
            DrawTimeOfDay = new TimeOnly(12, 0, 0),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // 👉 ACT: create the game instance
        var exception = await Record.ExceptionAsync(() =>
            _gameService.StartGameInstance(gameInstanceDto)
        );

        Assert.Null(exception);

        // 👉 ASSERT: query AFTER creation
        var lastGameInstance = await _ctx.GameInstances
            .OrderByDescending(g => g.CreatedAt)
            .FirstOrDefaultAsync(g => g.GameTemplateId == gameTemplateResponse.Id);

        Assert.NotNull(lastGameInstance);
        Assert.Equal(gameTemplateResponse.Id, lastGameInstance.GameTemplateId);
        Assert.Equal(GameStatus.Active, lastGameInstance.Status);
    }


    private GameTemplateResponseDto GetGameTemplateFromName(string name)
    {
        var getAllTemplates = _gameService.GetGameTemplatesAsync().Result;
        return getAllTemplates.FirstOrDefault(t => t.Name == name);
    }
}