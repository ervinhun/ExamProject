using System.Security.Cryptography;
using Api.Dto.test;
using Api.Dto.User;
using Api.Services.Email;
using DataAccess;
using DataAccess.Entities.Auth;
using DataAccess.Entities.Finance;
using Microsoft.AspNetCore.Mvc;
using Utils;

namespace Api.Services.Admin;

public class UserManagementService(MyDbContext ctx, IEmailService emailService) : IUserManagementService
{
    
    public async Task<ActionResult<Player>> RegisterPlayer(CreatePlayerDto createPlayerDto)
    {
        HashUtils.CreatePasswordHash("123321", out byte[] hash, out var salt);
        
        var player = new Player()
        {
            FullName = createPlayerDto.FullName,
            Email = createPlayerDto.Email,
            PhoneNumber = createPlayerDto.PhoneNo,
            PasswordHash = hash,
            PasswordSalt = salt,
        };
        await ctx.Players.AddAsync(player);

        return player;
    }

    public Task<ActionResult<DataAccess.Entities.Auth.Admin>> RegisterAdmin(CreateAdminDto createAdminDto)
    {
        throw new NotImplementedException();
    }

    public Task<ActionResult<User>> UpdateUser(UpdateUserDetailsDto updateUserDto)
    {
        throw new NotImplementedException();
    }
    
    public Task<ActionResult> DeleteUser(Guid userId)
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