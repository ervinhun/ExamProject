using Api.Dto.test;
using Api.Dto.User;
using DataAccess.Entities.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Api.Services.Admin;

public interface IUserManagementService
{
    public Task<UserDto> RegisterUser(CreateUserDto createUserDto);
    
    public Task<PlayerDto> RegisterPlayer(CreatePlayerDto createPlayerDto);
    
    public Task<AdminDto> RegisterAdmin(CreateAdminDto createAdminDto);
    
    public Task<ICollection<UserDto>> GetAllUsersAsync();
    
    public Task<ActionResult<User>> UpdateUser(UpdateUserDetailsDto updateUserDto);
    public Task<ActionResult> DeleteUser(Guid userId);
}