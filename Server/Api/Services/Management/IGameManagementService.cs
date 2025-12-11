using Api.Dto.Game;
using Api.Dto.test;
using DataAccess.Entities.Game;

namespace Api.Services.Management;

public interface IGameManagementService
{
    Task CreateGameTemplate(CreateGameTemplateRequestDto gameTemplateDto);
    Task<ICollection<GameTemplateResponseDto>> GetGameTemplatesAsync();
    Task<List<GameInstanceDto>> GetAllActiveGamesAsync();
    Task<GameTemplateResponseDto> GetGameTemplateById(Guid gameTemplateId);
    Task<GameTemplateResponseDto> UpdateGameTemplateById(Guid templateId, CreateGameTemplateRequestDto gameTemplateDto);
    Task DeleteGameTemplateById(Guid templateId);
    
    Task StartGameInstance(GameInstanceDto gameInstanceDto);
    Task StopGameInstanceById(Guid gameInstanceId);
}