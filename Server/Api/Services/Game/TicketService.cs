using Api.Dto.Game;
using Api.Dto.Transaction;
using DataAccess;
using DataAccess.Entities.Game;
using DataAccess.Enums;
using Microsoft.EntityFrameworkCore;
using Utils.Exceptions;

namespace Api.Services.Game;

public class TicketService(MyDbContext ctx, IWalletTransactionsService walletTransactionsService) : ITicketService
{
    // public async Task<TicketDto.TicketResponseDto> CreateTicket(Guid playerId,
    //     TicketDto.CreateTicketRequestDto ticketDto)
    // {
    //     var data = await ctx.GameInstances
    //         .Where(g => g.Id == ticketDto.GameInstanceId && g.Status == GameStatus.Active)
    //         .Select(g => new
    //         {
    //             GameInstance = g,
    //             GameTemplate = ctx.GameTemplates.FirstOrDefault(t => t.Id == g.GameTemplate.Id),
    //             Wallet = ctx.Wallets.FirstOrDefault(w => w.PlayerId == playerId)
    //         })
    //         .FirstOrDefaultAsync();
    //
    //     if (data?.GameInstance == null)
    //         throw new InvalidOperationException("Active game instance not found or the game is not active.");
    //
    //     if (data.GameTemplate == null)
    //         throw new InvalidOperationException("Game template not found.");
    //
    //     if (data.Wallet == null)
    //         throw new InvalidOperationException("Wallet not found.");
    //
    //     if (ticketDto.SelectedNumbers.Length < data.GameTemplate.MinNumbersPerTicket ||
    //         ticketDto.SelectedNumbers.Length > data.GameTemplate.MaxNumbersPerTicket)
    //         throw new InvalidOperationException("Invalid number of tickets");
    //
    //     var numbers = ticketDto.SelectedNumbers.OrderBy(n => n).ToList();
    //     Dictionary<int, double> priceGrowthRule;
    //
    //     if (!string.IsNullOrWhiteSpace(data.GameTemplate.PriceGrowthRule))
    //     {
    //         // JSON exists → deserialize it
    //         priceGrowthRule = JsonSerializer.Deserialize<Dictionary<int, double>>(
    //             data.GameTemplate.PriceGrowthRule
    //         )!;
    //     }
    //     else
    //     {
    //         // JSON missing → generate default rule
    //         priceGrowthRule = new Dictionary<int, double>();
    //
    //         var j = 0;
    //         for (var i = data.GameTemplate.MinNumbersPerTicket;
    //              i <= data.GameTemplate.MaxNumbersPerTicket;
    //              i++)
    //         {
    //             // basePrice * (2^j)
    //             var price = (data.GameTemplate.BasePrice * Math.Pow(2, j));
    //
    //             priceGrowthRule.Add(i, price);
    //             j++;
    //         }
    //     }
    //
    //     var priceOfTheTicket = priceGrowthRule[numbers.Count];
    //     if (data.Wallet.Balance < priceOfTheTicket)
    //         throw new InvalidOperationException("Insufficient funds");
    //     data.Wallet.Balance -= priceOfTheTicket;
    //     var ticket = new LotteryTicket
    //     {
    //         GameInstanceId = ticketDto.GameInstanceId,
    //         GameTemplateId = data.GameTemplate.Id,
    //         PlayerId = playerId,
    //         FullPrice = priceOfTheTicket,
    //         IsWinning = false,
    //         IsPaid = false,
    //         Repeatings = ticketDto.Repeat,
    //         BoughtAt = DateTime.UtcNow,
    //     };
    //     ctx.LotteryTickets.Add(ticket);
    //     await ctx.SaveChangesAsync();
    //
    //     Console.WriteLine("New ticket Id: " + ticket.Id);
    //     foreach (var number in numbers)
    //     {
    //         ticket.PickedNumbers.Add(new PickedNumber
    //             {
    //                 TicketId = ticket.Id,
    //                 Number = number
    //             }
    //         );
    //     }
    //
    //     data.Wallet.Balance -= priceOfTheTicket;
    //     await ctx.SaveChangesAsync();
    //     await SaveTicketPurchaseHistory(ticket, data.Wallet.Id);
    //     return ConvertTicketToTicketResponseDto(ticket);
    // }
    //
    public async Task<List<TicketDto>> GetAllTicketsForPlayerId(
        Guid playerId)
    {
        var tickets = await ctx.LotteryTickets.Include(t => t.PickedNumbers).Where(t => t.PlayerId == playerId)
            .ToListAsync();

        if (tickets.Count == 0) return [];
        
        var ticketsDto = tickets.Select(ConvertTicketToTicketResponseDto).ToList();

        return ticketsDto;
    }
    
    
    // public Task<List<TicketDto.TicketResponseDto>> GetAllTicketsForGameInstance(Guid gameInstanceId,
    //     bool winningOnly = false)
    // {
    //     var gameInstance = ctx.GameInstances
    //         .Include(i => i.GameTemplate);
    //     if (gameInstance == null) throw new InvalidOperationException("Game instance not found");
    //     return Task.FromResult(ctx.LotteryTickets.Where(t => t.GameInstanceId == gameInstanceId
    //                                                          && t.IsWinning == winningOnly)
    //         .Select(ConvertTicketToTicketResponseDto).ToList());
    // }
    //
    //
    // private async Task SaveTicketPurchaseHistory(LotteryTicket ticket, Guid walletId)
    // {
    //     try
    //     {
    //         var now = DateTime.UtcNow;
    //         var transaction = new Transaction
    //         {
    //             UserId = ticket.PlayerId,
    //             WalletId = walletId,
    //             Name = "Ticket purchase",
    //             Status = TransactionStatus.Approved,
    //             Type = TransactionType.TicketPurchase,
    //             Amount = ticket.FullPrice,
    //             CreatedAt = now
    //         };
    //         ctx.Transactions.Add(transaction);
    //         await ctx.SaveChangesAsync();
    //         var history = new TransactionHistory
    //         {
    //             TransactionId = transaction.Id,
    //             ActionUser = ticket.PlayerId,
    //             Status = TransactionStatus.Approved,
    //             Type = TransactionType.TicketPurchase
    //         };
    //         ctx.TransactionHistories.Add(history);
    //         await ctx.SaveChangesAsync();
    //     }
    //     catch (Exception e)
    //     {
    //         Console.WriteLine("Transaction history failed: " + e.Message);
    //     }
    // }
    //
    private static TicketDto ConvertTicketToTicketResponseDto(LotteryTicket ticket)
    {
        return new TicketDto
        {
            Id = ticket.Id,
            GameInstanceId = ticket.GameInstanceId,
            PickedNumbers = ticket.PickedNumbers.Select(p => p.Number).ToArray(),
            BoughtAt = ticket.BoughtAt,
            IsPaid = ticket.IsPaid,
            IsWinning = ticket.IsWinning,
            FullPrice = ticket.FullPrice
        };
    }

    public async Task PurchaseTicket(PurchaseTicketDto purchaseTicketDto)
    {
        var game = await ctx.GameInstances.Include(g => g.GameTemplate)
            .FirstOrDefaultAsync(g => g.Id == purchaseTicketDto.GameInstanceId);
        if (game == null || game.Status != GameStatus.Active)
            throw new ServiceException("Game instance not found or is not active");
        
        // Validate number of selected numbers
        var numbersCount = purchaseTicketDto.PickedNumbers.Length;
        if (numbersCount < game.GameTemplate!.MinNumbersPerTicket ||
            numbersCount > game.GameTemplate.MaxNumbersPerTicket)
            throw new ServiceException(
                $"Invalid number of selected numbers. Must be between {game.GameTemplate.MinNumbersPerTicket} and {game.GameTemplate.MaxNumbersPerTicket}");

        // Calculate expected price: base price * 2^(numbersCount - minNumbers)
        var basePrice = game.GameTemplate.BasePrice;
        var minNumbers = game.GameTemplate.MinNumbersPerTicket;
        var expectedPrice = basePrice * Math.Pow(2, numbersCount - minNumbers);

        // Validate the price from DTO
        var priceDifference = Math.Abs(purchaseTicketDto.FullPrice - expectedPrice);
        var isValidPrice = priceDifference <= 0.01;

        if (!isValidPrice)
            throw new ServiceException(
                $"Invalid ticket price. Expected {expectedPrice} but received {purchaseTicketDto.FullPrice}");

        var ticket = new LotteryTicket
        {
            GameInstanceId = game.Id,
            PlayerId = purchaseTicketDto.PlayerId,
            FullPrice = purchaseTicketDto.FullPrice,
            IsWinning = false,
            BoughtAt = DateTime.UtcNow
        };
        
        foreach (var pickedNumber in purchaseTicketDto.PickedNumbers)
        {
            ticket.PickedNumbers.Add(new PickedNumber
            {
                Number = pickedNumber,
                Ticket = ticket
            });
        }

        await ctx.LotteryTickets.AddAsync(ticket);
        await ctx.SaveChangesAsync();

        var transactionDto = new TransactionDto
        {
            UserId = purchaseTicketDto.PlayerId,
            Name = "Ticket purchase",
            WalletId = purchaseTicketDto.WalletId,
            Amount = purchaseTicketDto.FullPrice,
            Status = TransactionStatus.Requested,
            Type = TransactionType.TicketPurchase,
            PurchaseTicketId = ticket.Id,
            CreatedAt = DateTime.UtcNow,
        };

        await walletTransactionsService.RegisterTransaction(purchaseTicketDto.PlayerId, transactionDto);
    }

    public async Task StartTicketSubscription(StartTicketSubscriptionDto startTicketSubscriptionDto)
    {
        // Validate game template exists
        var gameTemplate = await ctx.GameTemplates
            .FirstOrDefaultAsync(gt => gt.Id == startTicketSubscriptionDto.GameTemplateId);
        
        if (gameTemplate == null)
            throw new ServiceException("Game template not found");

        // Validate number of selected numbers
        var numbersCount = startTicketSubscriptionDto.PickedNumbers.Length;
        if (numbersCount < gameTemplate.MinNumbersPerTicket ||
            numbersCount > gameTemplate.MaxNumbersPerTicket)
            throw new ServiceException(
                $"Invalid number of selected numbers. Must be between {gameTemplate.MinNumbersPerTicket} and {gameTemplate.MaxNumbersPerTicket}");

        // Calculate expected price: base price * 2^(numbersCount - minNumbers)
        var basePrice = gameTemplate.BasePrice;
        var minNumbers = gameTemplate.MinNumbersPerTicket;
        var expectedPrice = basePrice * Math.Pow(2, numbersCount - minNumbers);

        // Validate the price from DTO
        var priceDifference = Math.Abs(startTicketSubscriptionDto.Price - expectedPrice);
        var isValidPrice = priceDifference <= 0.01;

        if (!isValidPrice)
            throw new ServiceException(
                $"Invalid subscription price. Expected {expectedPrice} but received {startTicketSubscriptionDto.Price}");

        // Check if player has an active subscription for this game template
        var existingSubscription = await ctx.TicketSubscriptions
            .FirstOrDefaultAsync(ts => ts.PlayerId == startTicketSubscriptionDto.PlayerId 
                                      && ts.GameTemplateId == startTicketSubscriptionDto.GameTemplateId 
                                      && !ts.IsExpired);
        
        if (existingSubscription != null)
            throw new ServiceException("Player already has an active subscription for this game template");

        // Create the subscription
        var subscription = new TicketSubscription
        {
            PlayerId = startTicketSubscriptionDto.PlayerId,
            GameTemplateId = startTicketSubscriptionDto.GameTemplateId,
            Price = startTicketSubscriptionDto.Price,
            IsExpired = false,
            BoughtAt = DateTime.UtcNow
        };

        // Add picked numbers to subscription
        foreach (var pickedNumber in startTicketSubscriptionDto.PickedNumbers)
        {
            subscription.PickedNumbers.Add(new PickedNumber
            {
                Number = pickedNumber,
                TicketSubscriptionId = subscription.Id
            });
        }

        await ctx.TicketSubscriptions.AddAsync(subscription);
        await ctx.SaveChangesAsync();

        // Create transaction for subscription purchase
        var transactionDto = new TransactionDto
        {
            UserId = startTicketSubscriptionDto.PlayerId,
            Name = "Subscription purchase",
            WalletId = startTicketSubscriptionDto.WalletId,
            Amount = startTicketSubscriptionDto.Price,
            Status = TransactionStatus.Requested,
            Type = TransactionType.TicketSubscriptionStart,
            CreatedAt = DateTime.UtcNow,
        };

        await walletTransactionsService.RegisterTransaction(startTicketSubscriptionDto.PlayerId, transactionDto);
    }

    public async Task PurchaseTicketsForActiveSubscriptions(Guid gameTemplateId, Guid newGameInstanceId)
    {
        try
        {
            // Get all active subscriptions for this game template
            var activeSubscriptions = await ctx.TicketSubscriptions
                .Include(ts => ts.PickedNumbers)
                .Where(ts => ts.GameTemplateId == gameTemplateId && !ts.IsExpired)
                .ToListAsync();

            if (activeSubscriptions.Count == 0)
            {
                return; // No subscriptions to process
            }

            // Get wallets for all players with subscriptions
            var playerIds = activeSubscriptions.Select(ts => ts.PlayerId).Distinct().ToList();
            var wallets = await ctx.Wallets
                .Where(w => playerIds.Contains(w.PlayerId))
                .ToDictionaryAsync(w => w.PlayerId, w => w);

            // Purchase a ticket for each active subscription
            foreach (var subscription in activeSubscriptions)
            {
                try
                {
                    // Check if player has a wallet
                    if (!wallets.TryGetValue(subscription.PlayerId, out var wallet))
                    {
                        Console.WriteLine($"Player {subscription.PlayerId} does not have a wallet. Skipping subscription {subscription.Id}");
                        continue;
                    }

                    // Check if player has sufficient balance
                    if (wallet.Balance < subscription.Price)
                    {
                        Console.WriteLine($"Player {subscription.PlayerId} has insufficient balance. Skipping subscription {subscription.Id}");
                        continue;
                    }

                    // Create purchase ticket DTO and reuse existing PurchaseTicket method
                    var purchaseTicketDto = new PurchaseTicketDto
                    {
                        GameInstanceId = newGameInstanceId,
                        PlayerId = subscription.PlayerId,
                        WalletId = wallet.Id,
                        PickedNumbers = subscription.PickedNumbers.Select(pn => pn.Number).ToArray(),
                        FullPrice = subscription.Price
                    };

                    // Reuse the existing PurchaseTicket method
                    await PurchaseTicket(purchaseTicketDto);
                    
                    Console.WriteLine($"Successfully purchased ticket for subscription {subscription.Id} for player {subscription.PlayerId}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to purchase ticket for subscription {subscription.Id}: {ex.Message}");
                    // Continue with other subscriptions even if one fails
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error purchasing tickets for active subscriptions: {e.Message}");
            // Don't throw - we don't want to fail the entire draw process if subscription purchases fail
        }
    }

    public async Task<List<SubscriptionDto>> GetSubscriptionsForPlayer(Guid playerId)
    {
        var subscriptions = await ctx.TicketSubscriptions
            .Include(ts => ts.PickedNumbers)
            .Where(ts => ts.PlayerId == playerId)
            .OrderByDescending(ts => ts.BoughtAt)
            .ToListAsync();

        var subscriptionDtos = new List<SubscriptionDto>();
        
        foreach (var subscription in subscriptions)
        {
            // Get game template details
            var gameTemplate = await ctx.GameTemplates
                .FirstOrDefaultAsync(gt => gt.Id == subscription.GameTemplateId);

            var gameTemplateDto = gameTemplate != null ? new GameTemplateResponseDto
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
            } : null;

            subscriptionDtos.Add(new SubscriptionDto
            {
                Id = subscription.Id,
                GameTemplateId = subscription.GameTemplateId,
                PlayerId = subscription.PlayerId,
                PickedNumbers = subscription.PickedNumbers.Select(pn => pn.Number).ToArray(),
                Price = subscription.Price,
                IsExpired = subscription.IsExpired,
                BoughtAt = subscription.BoughtAt,
                GameTemplate = gameTemplateDto
            });
        }

        return subscriptionDtos;
    }
}