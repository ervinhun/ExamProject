using Api.Dto.Transaction;

using Api.Dto.Game;

namespace Api.Services.Game;

public interface ITicketService
{
    // Task PurchaseTicket()
    // Task<TicketDto.TicketResponseDto> CreateTicket(Guid playerId, TicketDto.CreateTicketRequestDto ticketDto);
    Task<List<TicketDto>> GetAllTicketsForPlayerId(Guid playerId);
    // Task<List<TicketDto.TicketResponseDto>> GetAllTicketsForGameInstance(Guid gameIsntanceId, bool winningOnly = false);
    Task PurchaseTicket(PurchaseTicketDto purchaseTicketDto);
}