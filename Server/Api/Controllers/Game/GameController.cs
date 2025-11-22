using Api.Dto.test;
using Api.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Game;

    
[Authorize(Roles = "superadmin,admin,player")]
[ApiController]
[Route("api/games")]
public class GameController
{

    [HttpGet("all")]
    public async Task<ActionResult<List<GameInstanceDto>>> GetAllGamesAsync()
    {
        return null;
    }

    [HttpGet("active")]
    public async Task<ActionResult<List<GameInstanceDto>>> GetAllActiveGamesAsync()
    {
        return null;
    }
    
    [Authorize(Roles = "admin,superadmin")]
    [HttpGet("templates")]
    public async Task<ActionResult<List<GameTemplateResponseDto>>> GetAllTemplatesAsync()
    {
        return null;
    }

    [Authorize(Roles = "admin,superadmin")]
    [HttpGet("templates/{templateId:guid}")]
    public async Task<ActionResult<GameTemplateResponseDto>> GetTemplateByIdAsync(Guid templateId)
    {
        return null;
    }
    
    [Authorize(Roles = "admin,superadmin")]
    [HttpPost("templates/create")]
    public async Task<ActionResult> CreateGameTemplate([FromBody] CreateGameTemplateRequestDto dto)
    {
        return null;
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