using Api.Dto.test;
using Api.Dto.User;
using DataAccess.Entities.Auth;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Api.Services.Management;

public interface IUserManagementService
{
    Task<UserDto> RegisterUser(CreateUserDto createUserDto);
    
    Task<PlayerDto> RegisterPlayer(CreatePlayerDto createPlayerDto);
    
    Task<AdminDto> RegisterAdmin(CreateAdminDto createAdminDto);
    
    Task<ICollection<UserDto>> GetAllUsersAsync();
    
    Task<PlayerDto> GetPlayerByIdAsync(Guid id);
    
    Task<UserDto> UpdateUser(UpdateUserDetailsDto updateUserDto);
    Task DeleteUser(Guid userId);
    Task<ICollection<PlayerDto>> GetAllPlayersAsync();

    Task<ActionResult<User>> GetUserById(Guid userId);
    Task<bool> ConfirmMembership(Guid userId, bool isConfirmed, bool isActive, Guid adminId);

    Task<List<PlayerWhoAppliedDto>> GetAppliedUsers();
    
    Task ToggleStatus(Guid userId);
    
}