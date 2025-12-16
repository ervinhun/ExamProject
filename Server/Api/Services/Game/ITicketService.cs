using Api.Dto.Transaction;
using Api.Dto.Game;

namespace Api.Services.Game;

public interface ITicketService
{
    // Task PurchaseTicket()
    Task<TicketResponseDto> CreateTicket(Guid playerId, CreateTicketRequestDto ticketDto);
    Task<List<TicketResponseDto>> GetAllTicketsForPlayerId(Guid playerId);
    Task<List<TicketResponseDto>> GetAllTicketsForGameInstance(Guid gameIsntanceId, bool winningOnly = false);
    Task PurchaseTicket(PurchaseTicketDto purchaseTicketDto);
}