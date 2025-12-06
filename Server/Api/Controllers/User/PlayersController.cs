using System.Security.Claims;
using Api.Dto.test;
using Api.Dto.Transaction;
using Api.Dto.User;
using Api.Services.Admin;
using Api.Services.Game;
using Api.Services.Management;
using DataAccess.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utils.Exceptions;

namespace Api.Controllers.User;

[ApiController]
[Route("api/players")]
public class PlayersController(IUserManagementService userManagementService, IWalletTransactionsService walletTransactionsService) : ControllerBase
{
    [Authorize(Roles = "superadmin,admin")]
    [HttpPost("register-player")]
    public async Task<IActionResult> RegisterPlayer([FromBody] CreatePlayerDto createPlayerDto)
    {
        try
        {
            var player = await userManagementService.RegisterPlayer(createPlayerDto);
            return Ok(player);
        }
        catch (ServiceException e)
        {
            return Conflict(new {message = e.Message});
        } 
    }
    
    [Authorize(Roles = "superadmin,admin")]
    [HttpGet("all-players")]
    public async Task<IActionResult> GetAllPlayersAsync()
    {
        try
        {
            var players = await userManagementService.GetAllPlayersAsync();
            return Ok(players);
        }
        catch (ServiceException e)
        {
            return Conflict(new {message = e.Message});
        }
    }

    [Authorize(Roles = "superadmin,admin,player")]
    [HttpGet("get-player/{id:guid}")]
    public async Task<IActionResult> GetPlayerByIdAsync(Guid id)
    {
        try
        {
            var player = await userManagementService.GetPlayerByIdAsync(id);
            return Ok(player);
        }
        catch (Exception e)
        {
            return Conflict(new { message = e.Message });
        }
    }

    [Authorize(Roles = "superadmin,admin,player")]
    [HttpGet("{id:guid}/wallet")]
    public async Task<IActionResult> GetWalletByPlayerIdAsync(Guid id)
    {
        try
        {
            var wallet = await walletTransactionsService.GetWalletForPlayerId(id);
            return Ok(wallet);
        }
        catch (ServiceException e)
        {
            return NotFound(new { message = e.Message });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { message = "Internal server error", detail = e.Message });
        }
    }

    [Authorize(Roles = "superadmin,admin,player")]
    [HttpPost("{id:guid}/wallet/deposit")]
    public async Task<IActionResult> RequestForDepositByPlayerIdAsync(Guid id, [FromBody] DepositRequestDto depositRequestDto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                         ?? User.FindFirst("sub")?.Value;
            var firstName = User.FindFirst(ClaimTypes.Name)?.Value;
            var lastName = User.FindFirst(ClaimTypes.Surname)?.Value;
            if (userId == null) return Unauthorized("Invalid user id");
            
            if(id != Guid.Parse(userId)) return BadRequest("Ids won't match");
            
            var transaction = new TransactionDto
            {
                UserId = id,
                WalletId = depositRequestDto.WalletId,
                Name = $"{firstName} {lastName}",
                TransactionNumber = null,
                Amount = depositRequestDto.Amount,
                Status = TransactionStatus.Requested,
                Type = TransactionType.Deposit,
                CreatedAt = DateTime.UtcNow,
            };
            await walletTransactionsService.RegisterTransaction(id, transaction);
            return Ok(200);
        }
        catch (ServiceException e)
        {
            return NotFound(new { message = e.Message });
        }
        catch ( Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
    

    [Authorize(Roles = "superadmin,admin,player")]
    [HttpPut("update-player/{id:guid}")]
    public async Task<ActionResult<UserDto>> UpdatePlayerDetailsByIdAsync(Guid id, [FromBody] UpdateUserDetailsDto updateUserDetailsDto)
    {
        return await Task.FromResult(Ok(id));
    }

    [Authorize(Roles = "superadmin,admin")]
    [HttpPut("delete-player/{id:guid}")]
    public async Task<ActionResult> DeletePlayerByIdAsync(Guid id)
    {
        return await Task.FromResult(Ok(id));
    }
    
    
    [AllowAnonymous]
    [HttpPost("apply")]
    public async Task<ActionResult> ApplyForMembership([FromBody] CreatePlayerDto createPlayerDto)
    {
        return await Task.FromResult(Ok(createPlayerDto));
    }
}