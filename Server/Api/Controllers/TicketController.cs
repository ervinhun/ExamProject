using System.Security.Claims;
using Api.Dto.Game;
using Api.Services.Game;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utils.Exceptions;

namespace Api.Controllers;

[ApiController]
[Route("api/tickets")]
public class TicketController(ITicketService ticketService) : ControllerBase
{
    [HttpGet("all-my-tickets")]
    public async Task<IActionResult> GetAllMyTickets()
    {
        try
        {
            var result = await ticketService.GetAllTicketsForPlayerId(new Guid(GetActiveUserId()));
            return Ok(result);
        }
        catch (ServiceException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
    //
    // [Authorize(Roles = "superadmin,admin")]
    // [HttpGet("tickets-for-game/{gameId:guid}")]
    // public async Task<IActionResult> GetAllTicketsForGame(Guid gameId, [FromQuery] bool? winning)
    // {
    //     var result = await ticketService.GetAllTicketsForGameInstance(gameId, winning ?? false);
    //     return Ok(result);
    // }
    //
    // [HttpPost("create-ticket")]
    // public async Task<IActionResult> CreateTicket([FromBody] TicketDto.CreateTicketRequestDto requestDto)
    // {
    //     Console.WriteLine("Request: " + requestDto);
    //     var result = await ticketService.CreateTicket(new Guid(GetActiveUserId()), requestDto);
    //     return Ok(result);
    // }
    
    [Authorize(Roles = "player")]
    [HttpPost("purchase-ticket")]
    public async Task<IActionResult> PurchaseTicket([FromBody] PurchaseTicketDto purchaseTicketDto)
    {
        try
        {
            if(Guid.Parse(GetActiveUserId()) !=  purchaseTicketDto.PlayerId) return BadRequest();
            await ticketService.PurchaseTicket(purchaseTicketDto);
            return Ok(200);
        }
        catch (ServiceException e)
        {
            return StatusCode(500, new { message = e.Message });
        }
    }
    
    private string GetActiveUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                                        throw new UnauthorizedAccessException("User Id not found");
}