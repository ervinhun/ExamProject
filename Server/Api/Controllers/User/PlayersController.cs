using Api.Dto.test;
using Api.Dto.User;
using Api.Services.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.User;

[Authorize(Roles = "superadmin, admin, player")]
[ApiController]
[Route("api/players")]
public class PlayersController(IUserManagementService userManagementService) : ControllerBase
{
    
    [HttpGet("all")]
    public async Task GetAllPlayersAsync()
    {
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetPlayerByIdAsync(Guid id)
    {
        return await Task.FromResult(Ok(id));
    }

    [HttpGet("{id:guid}/wallet")]
    public async Task<ActionResult<WalletDto>> GetWalletByPlayerIdAsync(Guid id)
    {
        return await Task.FromResult(Ok(id));
    }
    

    [HttpPut("update/{id:guid}")]
    public async Task<ActionResult<UserDto>> UpdatePlayerDetailsByIdAsync(Guid id, [FromBody] UpdateUserDetailsDto updateUserDetailsDto)
    {
        return await Task.FromResult(Ok(id));
    }

    [HttpPut("delete/{id:guid}")]
    public async Task<ActionResult> DeletePlayerByIdAsync(Guid id)
    {
        return await Task.FromResult(Ok(id));
    }

    [HttpPost("{id:guid}/top-up")]
    public async Task<ActionResult<TransactionDto>> TopUpAmountForPlayerIdAsync(Guid id, [FromBody] CreateTransactionRequestDto createTransactionDto)
    {
        return await Task.FromResult(Ok(createTransactionDto));
    }
    
    [Authorize(Roles = "superadmin,admin")]
    [HttpPost("create")]
    public async Task RegisterPlayerAsync([FromBody] CreatePlayerDto createPlayerDto)
    {
        try
        {
            await userManagementService.RegisterPlayer(createPlayerDto);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    [AllowAnonymous]
    [HttpPost("apply")]
    public async Task<ActionResult> ApplyForMembership([FromBody] CreatePlayerDto createPlayerDto)
    {
        return await Task.FromResult(Ok(createPlayerDto));
    }
}