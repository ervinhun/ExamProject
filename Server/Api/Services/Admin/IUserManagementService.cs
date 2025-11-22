using Api.Dto.test;
using DataAccess.Entities.Auth;
namespace Api.Services.Admin;

public interface IUserManagementService
{
    public Task<Player> RegisterPlayer(CreateUserDto createUserDto);
    public Task<DataAccess.Entities.Auth.Admin> RegisterAdmin(CreateAdminDto createAdminDto);
    
    public Task<Player> UpdateUser(UpdateUserDto updateUserDto);
    public Task DeleteUser(Guid userId);
}