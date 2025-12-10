using System.Text.Json;
using Api.Dto.Game;
using DataAccess;
using DataAccess.Entities.Finance;
using DataAccess.Entities.Game;
using DataAccess.Enums;
using Microsoft.EntityFrameworkCore;

namespace Api.Services.Game;

public class TicketService : ITicketService
{
    private readonly MyDbContext _ctx;
    private readonly ILogger<TicketService> _logger;

    public TicketService(MyDbContext ctx, ILogger<TicketService> logger)
    {
        _ctx = ctx;
        _logger = logger;
    }

    public async Task<TicketDto.TicketResponseDto> CreateTicket(Guid playerId,
        TicketDto.CreateTicketRequestDto ticketDto)
    {
        var gameInstance =
            await _ctx.GameInstances.FirstOrDefaultAsync(instance => instance.Id == ticketDto.GameInstanceId);
        if (gameInstance == null)
            throw new InvalidOperationException("Game instance not found");
        var gameTemplate =
            await _ctx.GameTemplates.FirstOrDefaultAsync(template => template.Id == gameInstance.GameTemplateId);
        if (gameTemplate == null)
            throw new InvalidOperationException("Game template not found");
        var wallet = await _ctx.Wallets.FirstOrDefaultAsync(w => w.PlayerId == playerId);
        if (wallet == null)
            throw new InvalidOperationException("Wallet not found");
        var numbers = ticketDto.SelectedNumbers.OrderBy(n => n).ToList();
        Dictionary<int, double> priceGrowthRule;

        if (!string.IsNullOrWhiteSpace(gameTemplate.PriceGrowthRule))
        {
            // JSON exists → deserialize it
            priceGrowthRule = JsonSerializer.Deserialize<Dictionary<int, double>>(
                gameTemplate.PriceGrowthRule
            )!;
        }
        else
        {
            // JSON missing → generate default rule
            priceGrowthRule = new Dictionary<int, double>();

            var j = 0;
            for (var i = gameTemplate.MinNumbersPerTicket;
                 i <= gameTemplate.MaxNumbersPerTicket;
                 i++)
            {
                // basePrice * (2^j)
                var price = (gameTemplate.BasePrice * Math.Pow(2, j));

                priceGrowthRule.Add(i, price);
                j++;
            }
        }

        var priceOfTheTicket = priceGrowthRule[numbers.Count];
        if (wallet.Balance < priceOfTheTicket)
            throw new InvalidOperationException("Insufficient funds");
        wallet.Balance -= priceOfTheTicket;
        var ticket = new LotteryTicket
        {
            GameInstanceId = ticketDto.GameInstanceId,
            GameTemplateId = gameTemplate.Id,
            PlayerId = playerId,
            FullPrice = priceOfTheTicket,
            IsWinning = false,
            IsPaid = false,
            Repeatings = ticketDto.Repeat,
            BoughtAt = DateTime.UtcNow,
        };
        _ctx.LotteryTickets.Add(ticket);
        await _ctx.SaveChangesAsync();

        Console.WriteLine("New ticket Id: " + ticket.Id);
        foreach (var number in numbers)
        {
            ticket.PickedNumbers.Add(new PickedNumber
                {
                    TicketId = ticket.Id,
                    Number = number
                }
            );
        }

        wallet.Balance -= priceOfTheTicket;
        await _ctx.SaveChangesAsync();
        await SaveTicketPurchaseHistory(ticket, wallet.Balance, wallet.Id);
        return ConvertTicketToTicketResponseDto(ticket);
    }

    public async Task<List<TicketDto.TicketResponseDto>> GetAllTicketsForPlayerId(
        Guid playerId, bool activeOnly = true)
    {
        var tickets = _ctx.LotteryTickets.Where(t => t.PlayerId == playerId).Include(t => t.PickedNumbers);

        var gameInstance = _ctx.GameInstances
            .Include(i => i.GameTemplate)
            .Where(i => i.Status == GameStatus.Active);

        if (activeOnly)
        {
            var activeInstanceId = await gameInstance
                .Select(i => i.Id)
                .FirstOrDefaultAsync();

            tickets = tickets.Where(t => t.GameInstanceId == activeInstanceId).Include(t => t.PickedNumbers);
        }

        var ticketList = tickets
            .Select(ConvertTicketToTicketResponseDto)
            .ToList();

        return ticketList;
    }


    public Task<List<TicketDto.TicketResponseDto>> GetAllTicketsForGameTemplateId(Guid gameTemplateId,
        bool activeOnly = true)
    {
        throw new NotImplementedException();
    }

    private async Task SaveTicketPurchaseHistory(LotteryTicket ticket, double walletBalance, Guid walletId)
    {
        try
        {
            var now = DateTime.UtcNow;
            var transaction = new Transaction
            {
                UserId = ticket.PlayerId,
                WalletId = walletId,
                Name = "Ticket purchase",
                Status = TransactionStatus.Approved,
                Type = TransactionType.TicketPurchase,
                Amount = ticket.FullPrice,
                CreatedAt = now
            };
            _ctx.Transactions.Add(transaction);
            await _ctx.SaveChangesAsync();
            var history = new TransactionHistory
            {
                TransactionId = transaction.Id,
                ActionUser = ticket.PlayerId,
                Status = TransactionStatus.Approved,
                Type = TransactionType.TicketPurchase
            };
            _ctx.TransactionHistories.Add(history);
            await _ctx.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine("Transaction history failed: " + e.Message);
        }
    }

    private TicketDto.TicketResponseDto ConvertTicketToTicketResponseDto(LotteryTicket ticket)
    {
        return new TicketDto.TicketResponseDto
        {
            Id = ticket.Id,
            GameInstanceId = ticket.GameInstanceId,
            GameTemplateId = ticket.GameTemplateId,
            SelectedNumbers = ticket.PickedNumbers.Select(p => p.Number).ToArray(),
            Repeat = ticket.Repeatings.GetValueOrDefault(),
            CreatedAt = ticket.BoughtAt,
            UpdatedAt = ticket.BoughtAt,
            IsPaid = ticket.IsPaid,
            IsWinning = ticket.IsWinning,
            FullPrice = ticket.FullPrice
        };
    }
}