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

    [HttpGet("all")]
    public async Task<ActionResult<List<GameInstanceDto>>> GetAllGamesAsync()
    {
        return null;
    }

    [HttpGet("active")]
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

    [HttpPost("start")]
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
    [HttpGet("templates/all")]
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
    [HttpGet("templates/{templateId:guid}")]
    public async Task<ActionResult<GameTemplateResponseDto>> GetTemplateByIdAsync(Guid templateId)
    {
        return null;
    }
    
    [Authorize(Roles = "admin,superadmin")]
    [HttpPost("templates/create")]
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
    [HttpPut("templates/update/{templateId:guid}")]
    public async Task<ActionResult<GameTemplateResponseDto>> UpdateGameTemplateByIdAsync(Guid templateId,
        [FromBody] CreateGameTemplateRequestDto gameTemplateRequestDto)
    {
        return null;
    }

    [Authorize(Roles = "admin,superadmin")]
    [HttpDelete("templates/delete/{templateId:guid}")]
    public async Task<ActionResult> DeleteGameTemplateByIdAsync(Guid templateId)
    {
        return null;
        
    }

    
}