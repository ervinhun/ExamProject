using Api.Dto.Transaction;

using Api.Dto.Game;

namespace Api.Services.Game;

public interface ITicketService
{
    Task<TicketDto.TicketResponseDto> CreateTicket(Guid playerId, TicketDto.CreateTicketRequestDto ticketDto);
    Task<List<TicketDto.TicketResponseDto>> GetAllTicketsForPlayerId(Guid playerId, bool activeOnly = true);
    Task<List<TicketDto.TicketResponseDto>> GetAllTicketsForGameInstance(Guid gameIsntanceId, bool winningOnly = false);
    Task PurchaseTicket(PurchaseTicketDto ticketDto);
}