using Api.Dto.Game;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Internal;

[ApiController]
[Route("internal/cron")]
public class CronGamesController : ControllerBase
{
    private readonly GameRolloverService _service;
    private readonly IConfiguration _config;

    public CronGamesController(
        GameRolloverService service,
        IConfiguration config)
    {
        _service = service;
        _config = config;
    }

    [HttpPost("games")]
    public async Task<IActionResult> Handle()
    {
        // Protect endpoint
        var secret = Request.Headers["X-Cron-Secret"].FirstOrDefault();
        if (secret != _config["Cron:Secret"])
            return Unauthorized();

        var result = await _service.ExecuteAsync();

        return Ok(new
        {
            closedGames = result.ClosedGames,
            createdGames = result.CreatedGames
        });
    }
}