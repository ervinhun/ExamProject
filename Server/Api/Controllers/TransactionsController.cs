using System.Security.Claims;
using Api.Services.Game;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utils.Exceptions;

namespace Api.Controllers;

[ApiController]
[Route("api/transactions")]
public class TransactionsController(IWalletTransactionsService walletTransactionsService) : ControllerBase
{

    [Authorize(Roles = "admin,superadmin")]
    [HttpGet("get-pending-transactions")]
    public async Task<IActionResult> GetPendingTransactions()
    {
        try
        {
            var transactions = await walletTransactionsService.GetPendingTransactions();
            return Ok(transactions);
        }catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = "admin,superadmin")]
    [HttpGet("get-transaction/{transactionId:guid}")]
    public async Task<IActionResult> GetTransactionByIdAsync(Guid transactionId)
    {
        throw  new NotImplementedException();
    }

    [Authorize(Roles = "admin,superadmin")]
    [HttpPost("approve-transaction/{transactionId:guid}")]
    public async Task<IActionResult> ApproveTransaction(Guid transactionId)
    {
        if (User.Identity is not { IsAuthenticated: true })
        {
            return Unauthorized(new {message="User is  not authenticated"});
        }

        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? User.FindFirst("sub")?.Value;
            await walletTransactionsService.ApproveTransaction(Guid.Parse(userId!), transactionId);
            return Ok(200);
        }
        catch (ServiceException serviceException)
        {
            return NotFound(new {message = serviceException.Message});
        }
        catch (Exception ex)
        {
            return BadRequest(new {message = ex.Message});
        }
    }
    
}