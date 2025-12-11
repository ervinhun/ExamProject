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
        var result = await ticketService.GetAllTicketsForPlayerId(new Guid(GetActiveUserId()), false);
        return Ok(result);
    }

    [Authorize(Roles = "superadmin,admin")]
    [HttpGet("tickets-for-game/{gameId:guid}")]
    public async Task<IActionResult> GetAllTicketsForGame(Guid gameId, [FromQuery] bool? winning)
    {
        var result = await ticketService.GetAllTicketsForGameInstance(gameId, winning ?? false);
        return Ok(result);
    }

    [HttpPost("create-ticket")]
    public async Task<IActionResult> CreateTicket([FromBody] TicketDto.CreateTicketRequestDto requestDto)
    {
        //"019b033c-50f2-7699-8e4e-ff738a4f849c"
        Console.WriteLine("Request: " + requestDto);
        var result = await ticketService.CreateTicket(new Guid(GetActiveUserId()), requestDto);
        return Ok(result);
    }

    private string GetActiveUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                                        throw new UnauthorizedAccessException("User Id not found");
}