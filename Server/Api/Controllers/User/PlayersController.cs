using Api.Dto.test;
using Api.Dto.User;
using Api.Services.Management;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utils.Exceptions;

namespace Api.Controllers.User;

[ApiController]
[Route("api/players")]
public class PlayersController(IUserManagementService userManagementService) : ControllerBase
{
    [Authorize(Roles = "superadmin,admin")]
    [HttpPost("create")]
    public async Task<IActionResult> CreatePlayer([FromBody] CreatePlayerDto createPlayerDto)
    {
        try
        {
            await userManagementService.RegisterPlayer(createPlayerDto);
            return Ok(200);
        }
        catch (ServiceException e)
        {
            return Conflict(new {message = e.Message});
        } 
    }
    
    [Authorize(Roles = "superadmin,admin")]
    [HttpGet("all")]
    public async Task<ActionResult<ICollection<PlayerDto>>> GetAllPlayersAsync()
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
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetPlayerByIdAsync(Guid id)
    {
        return await Task.FromResult(Ok(id));
    }

    [Authorize(Roles = "superadmin,admin,player")]
    [HttpGet("{id:guid}/wallet")]
    public async Task<ActionResult<WalletDto>> GetWalletByPlayerIdAsync(Guid id)
    {
        return await Task.FromResult(Ok(id));
    }
    

    [Authorize(Roles = "superadmin,admin,player")]
    [HttpPut("update/{id:guid}")]
    public async Task<ActionResult<UserDto>> UpdatePlayerDetailsByIdAsync(Guid id, [FromBody] UpdateUserDetailsDto updateUserDetailsDto)
    {
        return await Task.FromResult(Ok(id));
    }

    [Authorize(Roles = "superadmin,admin")]
    [HttpPut("delete/{id:guid}")]
    public async Task<ActionResult> DeletePlayerByIdAsync(Guid id)
    {
        return await Task.FromResult(Ok(id));
    }

    [Authorize(Roles = "superadmin,admin")]
    [HttpPost("{id:guid}/top-up")]
    public async Task<ActionResult<TransactionDto>> TopUpAmountForPlayerIdAsync(Guid id, [FromBody] CreateTransactionRequestDto createTransactionDto)
    {
        return await Task.FromResult(Ok(createTransactionDto));
    }
    
    
    [AllowAnonymous]
    [HttpPost("apply")]
    public async Task<ActionResult> ApplyForMembership([FromBody] CreatePlayerDto createPlayerDto)
    {
        return await Task.FromResult(Ok(createPlayerDto));
    }
}