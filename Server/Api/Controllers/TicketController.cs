using System.Security.Claims;
using Api.Dto.Game;
using Api.Services.Game;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/tickets")]
public class TicketController(ITicketService ticketService) : ControllerBase
{
    [HttpGet("all-my-tickets")]
    public async Task<IActionResult> GetAllTickets()
    {
        var activeUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (activeUserId == null)
            throw new UnauthorizedAccessException($"User Id '`{activeUserId}`' not found");
        var result = await ticketService.GetAllTicketsForPlayerId(new Guid(activeUserId), false);
        return Ok(result);
    }

    [HttpPost("create-ticket")]
    public async Task<IActionResult> CreateTicket([FromBody] TicketDto.CreateTicketRequestDto requestDto)
    {
        var activeUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (activeUserId == null)
            throw new UnauthorizedAccessException($"User Id '`{activeUserId}`' not found");
        var result = await ticketService.CreateTicket(new Guid(activeUserId), requestDto);
        return Ok(result);
    }
}