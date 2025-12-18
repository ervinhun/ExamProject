// using Api.Dto.Game;
// using Api.Services.Game;
// using DataAccess;
// using Microsoft.EntityFrameworkCore;
// using Test.Util;
//
// namespace Test;
//
// [Collection("Database collection")]
// public class TicketServiceTest
// {
//     private readonly MyDbContext _ctx;
//     private readonly TicketService _ticketService;
//     private readonly DatabaseFixture _fixture;
//     private readonly Seeder _seeder;
//
//     public TicketServiceTest(DatabaseFixture fixture)
//     {
//         _fixture = fixture;
//
//         var options = new DbContextOptionsBuilder<MyDbContext>()
//             .UseNpgsql(fixture.ConnectionString)
//             .Options;
//
//         _ctx = new MyDbContext(options);
//
//         //var seeder = new Seeder(_ctx);
//         _seeder = new Seeder(_ctx);
//         _seeder.Seed().GetAwaiter().GetResult();
//         _ticketService = new TicketService(_ctx);
//     }
//
//
//     private Guid ExistingUserIdWhoCanBuyTickets => _seeder.Player1Id;
//     private Guid ExistingUserIdWhoCanNotBuyTickets => _seeder.UserWithoutWalletId;
//     private Guid ValidGameInstanceId => _seeder.GameInstanceId;
//     private Guid ValidGameTemplateId => _seeder.GameTemplateId;
//
//     [Fact]
//     public async Task CreateTicketShouldReturnTicketDto()
//     {
//         await using var tx = await _ctx.Database.BeginTransactionAsync();
//
//         PurchaseTicketDto ticketDto = new PurchaseTicketDto
//         {
//             GameInstanceId = ValidGameInstanceId,
//             PickedNumbers = new[] { 1, 2, 3, 4, 5 },
//         };
//         var result = await _ticketService.PurchaseTicket(purchaseTicketDto);
//         Assert.NotNull(result);
//         Assert.Equal(ticketDto.GameInstanceId, result.GameInstanceId);
//         Assert.Equal(ticketDto.GameTemplateId, result.GameTemplateId);
//         Assert.Equal(ticketDto.SelectedNumbers, result.SelectedNumbers);
//         Assert.Equal(ticketDto.Repeat, result.Repeat);
//         
//         await tx.RollbackAsync();
//     }
//
//     [Fact]
//     public async Task CreateTicketShouldThrowExceptionWhenGameInstanceIdIsIncorrect()
//     {
//         CreateTicketRequestDto ticketDto = new CreateTicketRequestDto
//         {
//             GameInstanceId = Guid.NewGuid(),
//             GameTemplateId = ValidGameTemplateId,
//             SelectedNumbers = new int[] { 1, 2, 3, 4, 5 },
//             Repeat = 1
//         };
//
//         await Assert.ThrowsAsync<InvalidOperationException>(() =>
//             _ticketService.CreateTicket(ExistingUserIdWhoCanBuyTickets, ticketDto));
//     }
//
//     [Fact]
//     public async Task CreateTicketShouldThrowExceptionWhenGameTemplateIdIsIncorrect()
//     {
//         CreateTicketRequestDto ticketDto = new CreateTicketRequestDto
//         {
//             GameInstanceId = ValidGameInstanceId,
//             GameTemplateId = Guid.NewGuid(),
//             SelectedNumbers = new int[] { 1, 2, 3, 4, 5 },
//             Repeat = 1
//         };
//
//         await Assert.ThrowsAsync<InvalidOperationException>(() =>
//             _ticketService.CreateTicket(ExistingUserIdWhoCanBuyTickets, ticketDto));
//     }
//
//     [Fact]
//     public async Task CreateTicketShouldThrowExceptionWhenPlayerDoesntHaveValidWallet()
//     {
//         CreateTicketRequestDto ticketDto = new CreateTicketRequestDto
//         {
//             GameInstanceId = ValidGameInstanceId,
//             GameTemplateId = ValidGameTemplateId,
//             SelectedNumbers = new int[] { 1, 2, 3, 4, 5 },
//             Repeat = 1
//         };
//
//         await Assert.ThrowsAsync<InvalidOperationException>(() =>
//             _ticketService.CreateTicket(_seeder.UserWithoutWalletId, ticketDto));
//     }
//
//     [Fact]
//     public async Task CreateTicketShouldThrowExceptionWhenThereAreMoreNumbersThanInTemplate()
//     {
//         CreateTicketRequestDto ticketDto = new CreateTicketRequestDto
//         {
//             GameInstanceId = ValidGameInstanceId,
//             GameTemplateId = ValidGameTemplateId,
//             SelectedNumbers = new int[] { 1, 2 },
//             Repeat = 1
//         };
//
//         await Assert.ThrowsAsync<InvalidOperationException>(() =>
//             _ticketService.CreateTicket(ExistingUserIdWhoCanBuyTickets, ticketDto));
//     }
//
//     [Fact]
//     public async Task CreateTicketShouldThrowExceptionWhenThereAreLessNumbersThanInTemplate()
//     {
//         CreateTicketRequestDto ticketDto = new CreateTicketRequestDto
//         {
//             GameInstanceId = ValidGameInstanceId,
//             GameTemplateId = ValidGameTemplateId,
//             SelectedNumbers = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 },
//             Repeat = 1
//         };
//
//         await Assert.ThrowsAsync<InvalidOperationException>(() =>
//             _ticketService.CreateTicket(ExistingUserIdWhoCanBuyTickets, ticketDto));
//     }
//
//     [Fact]
//     public async Task CreateTicketShouldThrowExceptionWhenThePlayerDoesNotHaveEnoughMoneyToBuyTicket()
//     {
//         CreateTicketRequestDto ticketDto = new CreateTicketRequestDto
//         {
//             GameInstanceId = ValidGameInstanceId,
//             GameTemplateId = ValidGameTemplateId,
//             SelectedNumbers = new int[] { 1, 2, 3, 4, 5 },
//             Repeat = 1
//         };
//
//         await Assert.ThrowsAsync<InvalidOperationException>(() =>
//             _ticketService.CreateTicket(ExistingUserIdWhoCanNotBuyTickets, ticketDto));
//     }
//
//     [Fact]
//     public async Task GetAllTicketsForPlayerIdShouldReturnListOfTickets()
//     {
//         var result = await _ticketService.GetAllTicketsForPlayerId(ExistingUserIdWhoCanBuyTickets);
//         Assert.NotEmpty(result);
//     }
//
//     [Fact]
//     public async Task GetAllTicketsForPlayerIdShouldReturnEmptyListWhenPlayerHasNoTickets()
//     {
//         var result = await _ticketService.GetAllTicketsForPlayerId(ExistingUserIdWhoCanNotBuyTickets);
//         Assert.Empty(result);
//     }
//
//     [Fact]
//     public async Task GetAllTicketsForGameInstanceSuccess()
//     {
//         var result = await _ticketService.GetAllTicketsForGameInstance(ValidGameInstanceId);
//         Assert.NotEmpty(result);
//     }
//
//     [Fact]
//     public async Task GetAllTicketsForGameInstanceFail()
//     {
//         var result = await _ticketService.GetAllTicketsForGameInstance(Guid.NewGuid());
//         Assert.Empty(result);
//     }
// }