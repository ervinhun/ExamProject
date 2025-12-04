using Api.Dto.test;
using Api.Dto.User;
using DataAccess.Entities.Auth;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Api.Services.Management;

public interface IUserManagementService
{
    public Task<UserDto> RegisterUser(CreateUserDto createUserDto);

    public Task RegisterPlayer(CreatePlayerDto createPlayerDto);

    public Task<AdminDto> RegisterAdmin(CreateAdminDto createAdminDto);

    public Task<ICollection<UserDto>> GetAllUsersAsync();

    public Task<ActionResult<User>> UpdateUser(UpdateUserDetailsDto updateUserDto);
    public Task<ActionResult> DeleteUser(Guid userId);
    Task<ICollection<PlayerDto>> GetAllPlayersAsync();

    public Task<ActionResult<User>> GetUserById(Guid userId);
    public Task<string> RequestPasswordReset(string email);
    public Task<ActionResult<User>> RequestMembership(RequestRegistrationDto requestRegistrationDto);
    public Task<PlayerDto> ConfirmMembership(Guid userId, UserConfirmationEntity userConfirmationEntity);
    Task<bool> ResetPassword(string resetToken, ResetPasswordRequest request);
}