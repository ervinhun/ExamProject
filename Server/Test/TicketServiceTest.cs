using Api.Dto.Game;
using Api.Services.Game;
using DataAccess;
using DataAccess.Entities.Game;
using Test.Util;

namespace Test;

public class TicketServiceTest(
    MyDbContext ctx,
    ISeeder seeder,
    ITicketService ticketService)
{
    private Guid ExistingUserIdWhoCanBuyTickets = new Guid("1");
    private Guid ExistingUserIdWhoCanNotBuyTickets = new Guid("2");
    private Guid ValidGameInstanceId = new Guid("GameInstance");
    private Guid ValidGameTemplateId = new Guid("Gametemplate");

    [Fact]
    public async Task CreateTicketShouldReturnTicketDto()
    {
        TicketDto.CreateTicketRequestDto ticketDto = new TicketDto.CreateTicketRequestDto
        {
            GameInstanceId = ValidGameInstanceId,
            GameTemplateId = ValidGameTemplateId,
            SelectedNumbers = new [] { 1, 2, 3, 4, 5 },
            Repeat = 1
        };
        var result = await ticketService.CreateTicket(ExistingUserIdWhoCanBuyTickets, ticketDto);
        Assert.NotNull(result);
        Assert.Equal(ticketDto.GameInstanceId, result.GameInstanceId);
        Assert.Equal(ticketDto.GameTemplateId, result.GameTemplateId);
        Assert.Equal(ticketDto.SelectedNumbers, result.SelectedNumbers);
        Assert.Equal(ticketDto.Repeat, result.Repeat);
    }

    [Fact]
    public async Task CreateTicketShouldThrowExceptionWhenGameInstanceIdIsIncorrect()
    {
        TicketDto.CreateTicketRequestDto ticketDto = new TicketDto.CreateTicketRequestDto
        {
            GameInstanceId = new Guid("GameInstanceIdNotValid"),
            GameTemplateId =ValidGameTemplateId,
            SelectedNumbers = new int[] { 1, 2, 3, 4, 5 },
            Repeat = 1
        };
        
        await Assert.ThrowsAsync<InvalidOperationException>(() => ticketService.CreateTicket(ExistingUserIdWhoCanBuyTickets, ticketDto));
    }
    
    [Fact]
    public async Task CreateTicketShouldThrowExceptionWhenGameTamplateIdIsIncorrect()
    {
        TicketDto.CreateTicketRequestDto ticketDto = new TicketDto.CreateTicketRequestDto
        {
            GameInstanceId = ValidGameInstanceId,
            GameTemplateId = new Guid("NotAValidGameTemplateId"),
            SelectedNumbers = new int[] { 1, 2, 3, 4, 5 },
            Repeat = 1
        };
        
        await Assert.ThrowsAsync<InvalidOperationException>(() => ticketService.CreateTicket(ExistingUserIdWhoCanBuyTickets, ticketDto));
    }
    
    [Fact]
    public async Task CreateTicketShouldThrowExceptionWhenPlayerDoesntHaveValidWallet()
    {
        TicketDto.CreateTicketRequestDto ticketDto = new TicketDto.CreateTicketRequestDto
        {
            GameInstanceId = ValidGameInstanceId,
            GameTemplateId =ValidGameTemplateId,
            SelectedNumbers = new int[] { 1, 2, 3, 4, 5 },
            Repeat = 1
        };
        
        await Assert.ThrowsAsync<InvalidOperationException>(() => ticketService.CreateTicket(new Guid("InvalidPlayerSoItHasNoWallet"), ticketDto));
    }
    
    [Fact]
    public async Task CreateTicketShouldThrowExceptionWhenThereAreMoreOrLessNumbersThanInTemplate()
    {
        TicketDto.CreateTicketRequestDto ticketDto = new TicketDto.CreateTicketRequestDto
        {
            GameInstanceId = ValidGameInstanceId,
            GameTemplateId =ValidGameTemplateId,
            SelectedNumbers = new int[] { 1, 2},
            Repeat = 1
        };
        
        await Assert.ThrowsAsync<InvalidOperationException>(() => ticketService.CreateTicket(ExistingUserIdWhoCanBuyTickets, ticketDto));
        
        ticketDto.SelectedNumbers = new []{1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20};
        await Assert.ThrowsAsync<InvalidOperationException>(() => ticketService.CreateTicket(ExistingUserIdWhoCanBuyTickets, ticketDto));
    }
    
    [Fact]
    public async Task CreateTicketShouldThrowExceptionWhenThePlayerDoesNotHaveEnoughMoneyToBuyTicket()
    {
        TicketDto.CreateTicketRequestDto ticketDto = new TicketDto.CreateTicketRequestDto
        {
            GameInstanceId = ValidGameInstanceId,
            GameTemplateId =ValidGameTemplateId,
            SelectedNumbers = new int[] { 1, 2},
            Repeat = 1
        };
        
        await Assert.ThrowsAsync<InvalidOperationException>(() => ticketService.CreateTicket(ExistingUserIdWhoCanBuyTickets, ticketDto));
        
        ticketDto.SelectedNumbers = new []{1,2,3,4,5};
        await Assert.ThrowsAsync<InvalidOperationException>(() => ticketService.CreateTicket(ExistingUserIdWhoCanNotBuyTickets, ticketDto));
    }
    
    [Fact]
    public async Task GetAllTicketsForPlayerIdShouldReturnListOfTickets()
    {
        var result = await ticketService.GetAllTicketsForPlayerId(ExistingUserIdWhoCanBuyTickets);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task GetAllTicketsForGameInstanceSuccess()
    {
        var result = await ticketService.GetAllTicketsForGameInstance(ValidGameInstanceId);
        Assert.NotEmpty(result);
    }
    
    [Fact]
    public async Task GetAllTicketsForGameInstanceFail()
    {
        var result = await ticketService.GetAllTicketsForGameInstance(new Guid("NotAValidGameInstanceId"));
        Assert.Empty(result);
    }
}