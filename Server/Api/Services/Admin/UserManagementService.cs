using System.Security.Cryptography;
using Api.Dto.test;
using Api.Dto.User;
using Api.Services.Email;
using DataAccess;
using DataAccess.Entities.Auth;
using DataAccess.Entities.Finance;
using DataAccess.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utils;

namespace Api.Services.Admin;

public class UserManagementService(MyDbContext ctx, IEmailService emailService) : IUserManagementService
{
    
    public async Task<UserDto> RegisterUser(CreateUserDto createUserDto)
    {
        HashUtils.CreatePasswordHash("player", out var hash, out var salt);
        
        var user = new User()
        {
            FirstName = createUserDto.FirstName,
            LastName =  createUserDto.LastName,
            Email = createUserDto.Email,
            PhoneNumber = createUserDto.PhoneNumber,
            PasswordHash = hash,
            PasswordSalt = salt,
        };
        await ctx.Users.AddAsync(user);
    
        await ctx.SaveChangesAsync();
        var userDto =  new UserDto
        {
            UserId =  user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber =  user.PhoneNumber,
            Roles = user.Roles.Select(r => r.Name).ToList(),
            CreatedAt = DateTimeHelper.ToCopenhagen(user.CreatedAt)
        };

        return userDto;
    }

    public async Task<PlayerDto> RegisterPlayer(CreatePlayerDto createPlayerDto)
    {
        var createdUserDto = await RegisterUser(createPlayerDto);
        await AssignRoleToUserByIdAsync(UserRole.Player, createdUserDto.UserId);
        await ctx.SaveChangesAsync();
    }

    public Task<AdminDto> RegisterAdmin(CreateAdminDto createAdminDto)
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

    public async Task<UserDto> AssignRoleToUserByIdAsync(UserRole userRole, Guid userId)
    {
        if (userRole == UserRole.SuperAdmin)
        {
            throw new UnauthorizedAccessException("SuperAdmin role cannot be assigned to anyone");
        }

        var user = ctx.Users.Include(user => user.Roles).SingleOrDefaultAsync(u => u.Id == userId).Result;
        var role = ctx.Roles.SingleOrDefaultAsync(r => r.Name == userRole).Result;

        if (user == null || role == null)
        {
            throw new Exception($"User with id {userId}, {role} not found");
        }

        if (user.Roles.Contains(role))
        {
            throw new Exception($"User with id {userId}, {role} is already assigned");
        }

        user.Roles.Add(role);
        await ctx.SaveChangesAsync();

        return new UserDto
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Roles = user.Roles.Select(r => r.Name).ToList(),
            CreatedAt = DateTimeHelper.ToCopenhagen(user.CreatedAt)
        };
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