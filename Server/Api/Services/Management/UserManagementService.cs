using System.Security.Cryptography;
using Api.Dto.test;
using Api.Dto.User;
using Api.Services.Email;
using Api.Services.Management;
using DataAccess;
using DataAccess.Entities.Auth;
using DataAccess.Entities.Finance;
using DataAccess.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utils;
using Utils.Exceptions;
using static Api.Services.Management.UserConverter;

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

    public async Task<PlayerDto> RegisterPlayer(CreatePlayerDto createPlayerDto)
    {
        if (await ctx.Users.AnyAsync(u => u.Email == createPlayerDto.Email))
        {
            throw new ServiceException("Email already exists", new InvalidOperationException());
        }

        HashUtils.CreatePasswordHash("user", out var hash, out var salt);
        if (hash == null || hash.Length == 0 || salt == null || salt.Length == 0)
            throw new ServiceException("PasswordHash cannot be empty");


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

        // Assign player role, and a wallet
        var role = ctx.Roles.SingleOrDefaultAsync(r => r.Name == UserRole.Player).Result ?? throw new Exception("Player role not found");
        player.Roles.Add(role);

        try
        {
            await ctx.Players.AddAsync(player);
            await ctx.SaveChangesAsync();
            return new PlayerDto
            {
                Id = player.Id,
                FirstName = player.FirstName,
                LastName = player.LastName,
                Email = player.Email,
                PhoneNumber = player.PhoneNumber,
                CreatedAt = player.CreatedAt,
                ExpireDate = player.ExpireDate,
                UpdatedAt = player.UpdatedAt,
                IsDeleted = player.IsDeleted,
                IsActive = player.Activated
            };
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
        if (players == null) throw new ServiceException("No players found");
        var playerDtos = new List<PlayerDto>();
        foreach (var player in players)
        {
            playerDtos.Add(new PlayerDto
            {
                Id = player.Id,
                FirstName = player.FirstName,
                LastName = player.LastName,
                Email = player.Email,
                IsActive = player.Activated,
                PhoneNumber = player.PhoneNumber,
                CreatedAt = DateTimeHelper.ToCopenhagen(player.CreatedAt)
            });
        }

        return playerDtos;
    }

    public async Task ToggleStatus(Guid userId)
    {
        try
        {
            var user = await ctx.Users.SingleOrDefaultAsync(u => u.Id == userId);
            if (user == null) throw new ServiceException("User not found", new InvalidOperationException());

            user.Activated = !user.Activated;
            await ctx.SaveChangesAsync();
        }
        catch (Exception e)
        {
            throw new ServiceException(e.Message, e);
        }
    }

    public async Task<PlayerDto> GetPlayerByIdAsync(Guid id)
    {
        try
        {
            var player = await ctx.Players.SingleOrDefaultAsync(u => u.Id == id);
            if (player == null) throw new ServiceException("Player not found", new InvalidOperationException());

            return new PlayerDto
            {
                Id = player.Id,
                FirstName = player.FirstName,
                LastName = player.LastName,
                Email = player.Email,
                PhoneNumber = player.PhoneNumber,
                CreatedAt = player.CreatedAt,
                UpdatedAt = player.UpdatedAt,
                IsDeleted = player.IsDeleted,
                ExpireDate = player.ExpireDate,
                IsActive = player.Activated
            };
        }
        catch (Exception e)
        {
            throw new ServiceException(e.Message, e);
        }
    }


    public Task<ActionResult<User>> GetUserById(Guid userId)
    {
        throw new NotImplementedException();
    }


    public async Task<bool> ConfirmMembership(Guid userId, bool isConfirmed, bool isActive, Guid adminId)
    {
        
        var player = await ctx.Players
            .Include(p => p.Roles)
            .FirstOrDefaultAsync(p => p.Id == userId);

        if (player == null)
            throw new ServiceException("Player not found");

        //If the player is under 18, the system declines the membership
        if (player.DateOfBirth < DateTime.UtcNow.AddYears(-18))
        {
            player.Activated = false;
            player.UpdatedAt = DateTime.UtcNow;
            await ctx.SaveChangesAsync();
            isConfirmed = false;
            isActive = false;
        }

        // Assign role
        var role = ctx.Roles.SingleOrDefaultAsync(r => r.Name == UserRole.Player).Result ?? throw new Exception("Player role not found");
            player.Roles.Add(role);

        player.UpdatedAt = DateTime.UtcNow;
        player.Activated = isActive;
        
        if (isConfirmed)
        {
            var wallet = new Wallet
            {
                PlayerId = player.Id,
                Balance = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Player = player
            };

            ctx.Wallets.Add(wallet);
        }

        // WhoApplied entry
        var entry = await ctx.WhoApplied
            .FirstOrDefaultAsync(x => x.playerId == player.Id);

        if (entry != null)
        {
            entry.status = isConfirmed ? "Confirmed" : "Rejected";
            entry.updatedAt = DateTime.UtcNow;
            entry.reviewedBy = adminId;
        }

        return await ctx.SaveChangesAsync() > 0;
    }


    public async Task<List<PlayerWhoAppliedDto>> GetAppliedUsers()
    {
        var applied = await ctx.WhoApplied
            .Include(w => w.Player)
            .Where(u => u.status == "Pending" || u.createdAt == u.updatedAt)
            .ToListAsync();

        if (applied.Count == 0)
            return new List<PlayerWhoAppliedDto>();

        var result = new List<PlayerWhoAppliedDto>();

        foreach (var a in applied)
        {
            result.Add(new PlayerWhoAppliedDto
            {
                Id = a.id,
                Status = a.status,
                CreatedAt = a.createdAt,
                UpdatedAt = a.updatedAt,
                ReviewedBy = a.reviewedBy,

                Player = new PlayerDto
                {
                    Id = a.Player.Id,
                    FirstName = a.Player.FirstName,
                    LastName = a.Player.LastName,
                    Email = a.Player.Email,
                    Dob = a.Player.DateOfBirth,
                    IsActive = a.Player.Activated,
                    PhoneNumber = a.Player.PhoneNumber,
                    CreatedAt = DateTimeHelper.ToCopenhagen(a.Player.CreatedAt)
                }
            });
        }

        return result;
    }


    public Task<AdminDto> RegisterAdmin(CreateAdminDto createAdminDto)
    {
        throw new NotImplementedException();
    }

    public Task<UserDto> UpdateUser(UpdateUserDetailsDto updateUserDto)
    {
        throw new NotImplementedException();
    }

    public Task DeleteUser(Guid userId)
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

    private User CheckWhoIsLoggedInFromToken(string token) // TODO: Check on the controller instead like in transaction
    {
        return ctx.Users.Include(u => u.Roles).SingleOrDefault(u => u.RefreshTokenHash == token)
               ?? throw new UnauthorizedAccessException();
    }
}