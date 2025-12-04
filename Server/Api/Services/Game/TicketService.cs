using Api.Dto.Transaction;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Utils.Exceptions;

namespace Api.Services.Game;

public class TicketService(IWalletTransactionsService walletTransactionsService, MyDbContext ctx) : ITicketService
{
    public async Task PurchaseTicket(PurchaseTicketDto ticketDto)
    {
        try
        {
            var game = await ctx.GameInstances.Include(gameInstance => gameInstance.GameTemplate).FirstOrDefaultAsync(g => g.Id == ticketDto.GameInstanceId);
            if(game == null) throw new ServiceException("Game not found.");
            var player = await ctx.Players.FirstOrDefaultAsync(p => p.Id == ticketDto.PlayerId && p.Activated);
            if(player == null) throw new ServiceException("Player not found.");
            if (ticketDto.PickedNumbers.Count < game.GameTemplate!.MinNumbersPerTicket || ticketDto.PickedNumbers.Count > game.GameTemplate!.MaxNumbersPerTicket)
            {
                throw new ServiceException($"Pick numbers from:{game.GameTemplate.MinNumbersPerTicket}-{game.GameTemplate.MaxNumbersPerTicket}");
            }
            
        }
    }
}