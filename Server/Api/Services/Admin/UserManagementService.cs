using System.Security.Cryptography;
using Api.Dto.test;
using Api.Services.Email;
using DataAccess;
using DataAccess.Entities.Auth;
using DataAccess.Entities.Finance;
using Utils;

namespace Api.Services.Admin;

public class UserManagementService(MyDbContext ctx, IEmailService emailService) : IUserManagementService
{
    public Task<Player> RegisterPlayer(CreateUserDto createUserDto)
    {
        HashUtils.CreatePasswordHash(GeneratePassword(), out byte[] hash, out var salt);
        
        var player = new Player()
        {
            FullName = createUserDto.FullName,
            Email = createUserDto.Email,
            PhoneNumber = createUserDto.PhoneNo,
            PasswordHash = hash,
            PasswordSalt = salt,
        };
        
        return Task.FromResult(player);
    }

    public Task<DataAccess.Entities.Auth.Admin> RegisterAdmin(CreateAdminDto createAdminDto)
    {
        throw new NotImplementedException();
    }

    public Task<Player> UpdateUser(UpdateUserDto updateUserDto)
    {
        throw new NotImplementedException();
    }

    public Task DeleteUser(Guid userId)
    {
        throw new NotImplementedException();
    }
    
    private static string GeneratePassword(int length = 6)
    {
        const string chars =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
            "abcdefghijklmnopqrstuvwxyz" +
            "0123456789" +
            "!@#$%^&*()-_=+[]{}<>?";

        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[length];
        rng.GetBytes(bytes);

        var result = new char[length];
        for (int i = 0; i < length; i++)
        {
            result[i] = chars[bytes[i] % chars.Length];
        }

        return new string(result);
    }
}