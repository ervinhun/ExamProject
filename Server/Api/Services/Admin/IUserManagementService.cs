using Api.Dto.test;
using Api.Dto.User;
using DataAccess.Entities.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Api.Services.Admin;

public interface IUserManagementService
{
    public Task<ActionResult<Player>> RegisterPlayer(CreatePlayerDto createPlayerDto);
    public Task<ActionResult<DataAccess.Entities.Auth.Admin>> RegisterAdmin(CreateAdminDto createAdminDto);
    
    public Task<ActionResult<User>> UpdateUser(UpdateUserDetailsDto updateUserDto);
    public Task<ActionResult> DeleteUser(Guid userId);
}