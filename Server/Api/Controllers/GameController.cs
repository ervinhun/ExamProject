using Api.Dto.Game;
using Api.Services.Management;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utils.Exceptions;

namespace Api.Controllers;

    
[Authorize(Roles = "superadmin,admin,player")]
[ApiController]
[Route("api/games")]
public class GameController(IGameManagementService gameManagementService) : ControllerBase
{

    [HttpGet("all-games")]
    public async Task<ActionResult<List<GameInstanceDto>>> GetAllGamesAsync()
    {
        return null;
    }

    [HttpGet("active-games")]
    public async Task<IActionResult> GetAllActiveGamesAsync()
    {
        try
        {
            var activeGames = gameManagementService.GetAllActiveGamesAsync().Result;
            return Ok(activeGames);
        }
        catch (ServiceException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPost("start-game")]
    public async Task StartGameInstanceAsync([FromBody] GameInstanceDto gameInstanceDto)
    {
        try
        {
            await gameManagementService.StartGameInstance(gameInstanceDto);
        }catch(Exception ex)
        {
            Conflict(ex.Message);
        }
    }
    
    [Authorize(Roles = "admin,superadmin")]
    [HttpGet("templates/all-templates")]
    public async Task<IActionResult> GetAllTemplatesAsync()
    {
        try
        {
            var templates = await gameManagementService.GetGameTemplates();
            return Ok(templates);
        }
        catch (Exception ex)
        {
            return Conflict(ex.Message);
        }
    }

    [Authorize(Roles = "admin,superadmin")]
    [HttpGet("templates/get-template/{templateId:guid}")]
    public async Task<IActionResult> GetTemplateByIdAsync(Guid templateId)
    {
        return null;
    }
    
    [Authorize(Roles = "admin,superadmin")]
    [HttpPost("templates/create-template")]
    public async Task<IActionResult> CreateGameTemplate([FromBody] CreateGameTemplateRequestDto dto)
    {
        try
        {
            await gameManagementService.CreateGameTemplate(dto);
            return Ok(dto);
        }
        catch (ServiceException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [Authorize(Roles = "admin,superadmin")]
    [HttpPut("templates/update-template/{templateId:guid}")]
    public async Task<IActionResult> UpdateGameTemplateByIdAsync(Guid templateId,
        [FromBody] CreateGameTemplateRequestDto gameTemplateRequestDto)
    {
        return null;
    }

    [Authorize(Roles = "admin,superadmin")]
    [HttpDelete("templates/delete-template/{templateId:guid}")]
    public async Task<ActionResult> DeleteGameTemplateByIdAsync(Guid templateId)
    {
        return null;
        
    }

    
}