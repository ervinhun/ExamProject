using System.Text.Json;
using Api.Dto.Game;
using Api.Dto.Transaction;
using DataAccess;
using DataAccess.Entities.Finance;
using DataAccess.Entities.Game;
using DataAccess.Enums;
using Microsoft.EntityFrameworkCore;

namespace Api.Services.Game;

public class TicketService(MyDbContext ctx) : ITicketService
{
    public async Task<TicketDto.TicketResponseDto> CreateTicket(Guid playerId,
        TicketDto.CreateTicketRequestDto ticketDto)
    {
        var data = await ctx.GameInstances
            .Where(g => g.Id == ticketDto.GameInstanceId && g.Status == GameStatus.Active)
            .Select(g => new
            {
                GameInstance = g,
                GameTemplate = ctx.GameTemplates.FirstOrDefault(t => t.Id == g.GameTemplate.Id),
                Wallet = ctx.Wallets.FirstOrDefault(w => w.PlayerId == playerId)
            })
            .FirstOrDefaultAsync();

        if (data?.GameInstance == null)
            throw new InvalidOperationException("Active game instance not found or the game is not active.");

        if (data.GameTemplate == null)
            throw new InvalidOperationException("Game template not found.");

        if (data.Wallet == null)
            throw new InvalidOperationException("Wallet not found.");

        if (ticketDto.SelectedNumbers.Length < data.GameTemplate.MinNumbersPerTicket ||
            ticketDto.SelectedNumbers.Length > data.GameTemplate.MaxNumbersPerTicket)
            throw new InvalidOperationException("Invalid number of tickets");

        var numbers = ticketDto.SelectedNumbers.OrderBy(n => n).ToList();
        Dictionary<int, double> priceGrowthRule;

        if (!string.IsNullOrWhiteSpace(data.GameTemplate.PriceGrowthRule))
        {
            // JSON exists → deserialize it
            priceGrowthRule = JsonSerializer.Deserialize<Dictionary<int, double>>(
                data.GameTemplate.PriceGrowthRule
            )!;
        }
        else
        {
            // JSON missing → generate default rule
            priceGrowthRule = new Dictionary<int, double>();

            var j = 0;
            for (var i = data.GameTemplate.MinNumbersPerTicket;
                 i <= data.GameTemplate.MaxNumbersPerTicket;
                 i++)
            {
                // basePrice * (2^j)
                var price = (data.GameTemplate.BasePrice * Math.Pow(2, j));

                priceGrowthRule.Add(i, price);
                j++;
            }
        }

        var priceOfTheTicket = priceGrowthRule[numbers.Count];
        if (data.Wallet.Balance < priceOfTheTicket)
            throw new InvalidOperationException("Insufficient funds");
        data.Wallet.Balance -= priceOfTheTicket;
        var ticket = new LotteryTicket
        {
            GameInstanceId = ticketDto.GameInstanceId,
            GameTemplateId = data.GameTemplate.Id,
            PlayerId = playerId,
            FullPrice = priceOfTheTicket,
            IsWinning = false,
            IsPaid = false,
            Repeatings = ticketDto.Repeat,
            BoughtAt = DateTime.UtcNow,
        };
        ctx.LotteryTickets.Add(ticket);
        await ctx.SaveChangesAsync();

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

        data.Wallet.Balance -= priceOfTheTicket;
        await ctx.SaveChangesAsync();
        await SaveTicketPurchaseHistory(ticket, data.Wallet.Id);
        return ConvertTicketToTicketResponseDto(ticket);
    }

    public async Task<List<TicketDto.TicketResponseDto>> GetAllTicketsForPlayerId(
        Guid playerId, bool activeOnly = true)
    {
        var tickets = ctx.LotteryTickets.Where(t => t.PlayerId == playerId).Include(t => t.PickedNumbers);

        var gameInstance = ctx.GameInstances
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


    public Task<List<TicketDto.TicketResponseDto>> GetAllTicketsForGameInstance(Guid gameInstanceId,
        bool winningOnly = false)
    {
        var gameInstance = ctx.GameInstances
            .Include(i => i.GameTemplate);
        if (gameInstance == null) throw new InvalidOperationException("Game instance not found");
        return Task.FromResult(ctx.LotteryTickets.Where(t => t.GameInstanceId == gameInstanceId
                                                             && t.IsWinning == winningOnly)
            .Select(ConvertTicketToTicketResponseDto).ToList());
    }

    public Task PurchaseTicket(PurchaseTicketDto ticketDto)
    {
        throw new NotImplementedException();
    }

    private async Task SaveTicketPurchaseHistory(LotteryTicket ticket, Guid walletId)
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
            ctx.Transactions.Add(transaction);
            await ctx.SaveChangesAsync();
            var history = new TransactionHistory
            {
                TransactionId = transaction.Id,
                ActionUser = ticket.PlayerId,
                Status = TransactionStatus.Approved,
                Type = TransactionType.TicketPurchase
            };
            ctx.TransactionHistories.Add(history);
            await ctx.SaveChangesAsync();
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
            TicketPrice = ticket.FullPrice
        };
    }
}