using Api.Dto.Transaction;

namespace Api.Services.Game;

public interface ITicketService
{
    Task PurchaseTicket(PurchaseTicketDto ticketDto);
}