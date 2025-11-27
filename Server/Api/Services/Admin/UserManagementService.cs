using System.Security.Cryptography;
using Api.Dto.test;
using Api.Dto.User;
using Api.Services.Email;
using DataAccess;
using DataAccess.Entities.Auth;
using DataAccess.Entities.Finance;
using DataAccess.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utils;
using Utils.Exceptions;

namespace Api.Services.Admin;

public class UserManagementService(MyDbContext ctx, IEmailService emailService) : IUserManagementService
{
    public async Task<UserDto> RegisterUser(CreateUserDto createUserDto)
    {
        if (await ctx.Users.AnyAsync(u => u.Email == createUserDto.Email))
        {
            throw new ServiceException("Email already exists", new InvalidOperationException());
        }

        HashUtils.CreatePasswordHash("user", out var hash, out var salt);
        var user = new User
        {
            FirstName = createUserDto.FirstName,
            LastName = createUserDto.LastName,
            Email = createUserDto.Email,
            PhoneNumber = createUserDto.PhoneNumber,
            PasswordHash = hash,
            PasswordSalt = salt,
        };

        try
        {
            await ctx.Users.AddAsync(user);
            await ctx.SaveChangesAsync();
            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Roles = user.Roles.Select(r => r.Name).ToList(),
                PhoneNumber = user.PhoneNumber,
                CreatedAt = DateTimeHelper.ToCopenhagen(user.CreatedAt),
            };
        }
        catch (DbUpdateException e)
        {
            throw new ServiceException(e.Message, e);
        }
    }

    public async Task RegisterPlayer(CreatePlayerDto createPlayerDto)
    {
        if (await ctx.Users.AnyAsync(u => u.Email == createPlayerDto.Email))
        {
            throw new ServiceException("Email already exists", new InvalidOperationException());
        }

        HashUtils.CreatePasswordHash("user", out var hash, out var salt);
        
        // Get the Player role from database
        var playerRole = await ctx.Roles.FirstOrDefaultAsync(r => r.Name == UserRole.Player);
        if (playerRole == null)
        {
            throw new ServiceException("Player role not found in database", new InvalidOperationException());
        }
        
        var player = new Player
        {
            FirstName = createPlayerDto.FirstName,
            LastName = createPlayerDto.LastName,
            Email = createPlayerDto.Email,
            PhoneNumber = createPlayerDto.PhoneNumber,
            PasswordHash = hash,
            PasswordSalt = salt,
            Activated = false
        };
        
        // Assign player role
        player.Roles.Add(playerRole);

        var wallet = new Wallet
        {
            Player = player,
            Balance = 0
        };
        
        try
        {
            await ctx.Players.AddAsync(player);
            await ctx.Wallets.AddAsync(wallet);
            await ctx.SaveChangesAsync();
        }
        catch (DbUpdateException e)
        {
            throw new ServiceException(e.Message, e);
        }
    }

    public async Task<ICollection<UserDto>> GetAllUsersAsync()
    {
        var users = await ctx.Users.ToListAsync();
        var userDtos = new List<UserDto>();
        foreach (var user in users)
        {
            userDtos.Add(new PlayerDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                CreatedAt = DateTimeHelper.ToCopenhagen(user.CreatedAt)
            });
        }

        return userDtos;
    }

    public async Task<ICollection<PlayerDto>> GetAllPlayersAsync()
    {
        var players = await ctx.Players.ToListAsync();
        if(players == null) throw new ServiceException("No players found");
        var playerDtos = new List<PlayerDto>();
        foreach (var player in players)
        {
            playerDtos.Add(new PlayerDto
            {
                Id = player.Id,
                FirstName = player.FirstName,
                LastName = player.LastName,
                Email = player.Email,
                PhoneNumber = player.PhoneNumber,
                CreatedAt = DateTimeHelper.ToCopenhagen(player.CreatedAt)
            });
        }

        return playerDtos;
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
        user.UpdatedAt = DateTime.UtcNow;
        await ctx.SaveChangesAsync();

        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            CreatedAt = DateTimeHelper.ToCopenhagen(user.CreatedAt),
            UpdatedAt = DateTimeHelper.ToCopenhagen(user.UpdatedAt)
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