using System.Globalization;
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

namespace Api.Services.Admin;

public class UserManagementService(MyDbContext ctx, IEmailService emailService) : IUserManagementService
{
    public async Task<UserDto> RegisterUser(CreateUserDto createUserDto)
    {
        if (await ctx.Users.AnyAsync(u => u.Email == createUserDto.Email))
            throw new ServiceException("Email already exists", new InvalidOperationException());

        HashUtils.CreatePasswordHash("user", out var hash, out var salt);

        var user = new User
        {
            FirstName = createUserDto.FirstName,
            LastName = createUserDto.LastName,
            Email = createUserDto.Email,
            DateOfBirth = ConvertStringToDateTime(createUserDto.BirthDate),
            PhoneNumber = createUserDto.PhoneNumber,
            PasswordHash = hash,
            PasswordSalt = salt,
            Roles = new List<Role>()
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
                Dob = user.DateOfBirth,
                PhoneNumber = user.PhoneNumber,
                CreatedAt = DateTimeHelper.ToCopenhagen(user.CreatedAt),
                Roles = user.Roles.Select(r => r.Name).ToList()
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
            throw new ServiceException("Email already exists", new InvalidOperationException());

        HashUtils.CreatePasswordHash("user", out var hash, out var salt);

        if (hash == null || hash.Length == 0 || salt == null || salt.Length == 0)
            throw new ServiceException("PasswordHash cannot be empty");

        var playerRole = await ctx.Roles.SingleOrDefaultAsync(r => r.Name == UserRole.Player)
                         ?? throw new ServiceException("Player role not found in database");

        var player = new Player
        {
            FirstName = createPlayerDto.FirstName,
            LastName = createPlayerDto.LastName,
            Email = createPlayerDto.Email,
            DateOfBirth = ConvertStringToDateTime(createPlayerDto.BirthDate),
            PhoneNumber = createPlayerDto.PhoneNumber,
            PasswordHash = hash,
            PasswordSalt = salt,
            Activated = false,
            Roles = new List<Role> { playerRole }
        };

        var wallet = new Wallet
        {
            Player = player,
            Balance = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        player.Wallet = wallet;

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
                Dob = player.DateOfBirth,
                PhoneNumber = player.PhoneNumber,
                CreatedAt = player.CreatedAt,
                UpdatedAt = player.UpdatedAt,
                ExpireDate = player.ExpireDate,
                IsDeleted = player.IsDeleted,
                IsActive = player.Activated,
                Roles = player.Roles.Select(r => r.Name).ToList()
            };
        }
        catch (DbUpdateException e)
        {
            throw new ServiceException(e.Message, e);
        }
    }

    public async Task<ICollection<UserDto>> GetAllUsersAsync()
    {
        var users = await ctx.Users.Include(u => u.Roles).ToListAsync();

        return users.Select(u => new UserDto
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email,
            Dob = u.DateOfBirth,
            PhoneNumber = u.PhoneNumber,
            CreatedAt = DateTimeHelper.ToCopenhagen(u.CreatedAt),
            Roles = u.Roles.Select(r => r.Name).ToList()
        }).ToList();
    }

    public async Task<ICollection<PlayerDto>> GetAllPlayersAsync()
    {
        var players = await ctx.Players.Include(p => p.Roles).ToListAsync();

        return players.Select(p => new PlayerDto
        {
            Id = p.Id,
            FirstName = p.FirstName,
            LastName = p.LastName,
            Email = p.Email,
            Dob = p.DateOfBirth,
            PhoneNumber = p.PhoneNumber,
            IsActive = p.Activated,
            CreatedAt = DateTimeHelper.ToCopenhagen(p.CreatedAt),
            Roles = p.Roles.Select(r => r.Name).ToList()
        }).ToList();
    }

    public async Task ToggleStatus(Guid userId)
    {
        var user = await ctx.Users.SingleOrDefaultAsync(u => u.Id == userId)
                   ?? throw new ServiceException("User not found");

        user.Activated = !user.Activated;
        user.UpdatedAt = DateTime.UtcNow;

        await ctx.SaveChangesAsync();
    }

    public async Task<PlayerDto> GetPlayerByIdAsync(Guid id)
    {
        var player = await ctx.Players
                         .Include(p => p.Roles)
                         .SingleOrDefaultAsync(p => p.Id == id)
                     ?? throw new ServiceException("Player not found");

        return new PlayerDto
        {
            Id = player.Id,
            FirstName = player.FirstName,
            LastName = player.LastName,
            Email = player.Email,
            Dob = player.DateOfBirth,
            PhoneNumber = player.PhoneNumber,
            CreatedAt = player.CreatedAt,
            UpdatedAt = player.UpdatedAt,
            ExpireDate = player.ExpireDate,
            IsDeleted = player.IsDeleted,
            IsActive = player.Activated,
            Roles = player.Roles.Select(r => r.Name).ToList()
        };
    }

    public Task<ActionResult<User>> GetUserById(Guid userId)
        => throw new NotImplementedException();

    public async Task<bool> ConfirmMembership(Guid userId, bool isConfirmed, bool isActive, Guid adminId)
    {
        var player = await ctx.Players
                         .Include(p => p.Roles)
                         .FirstOrDefaultAsync(p => p.Id == userId)
                     ?? throw new ServiceException("Player not found");

        player.Roles ??= new List<Role>();

        if (player.DateOfBirth > DateTime.UtcNow.AddYears(-18))
        {
            player.Activated = false;
            player.UpdatedAt = DateTime.UtcNow;
            await ctx.SaveChangesAsync();
            isConfirmed = false;
            isActive = false;
        }

        var role = await ctx.Roles.SingleOrDefaultAsync(r => r.Name == UserRole.Player)
                   ?? throw new ServiceException("Player role not found");

        if (!player.Roles.Any(r => r.Id == role.Id))
            player.Roles.Add(role);

        player.Activated = isActive;
        player.UpdatedAt = DateTime.UtcNow;

        if (isConfirmed)
        {
            ctx.Wallets.Add(new Wallet
            {
                PlayerId = player.Id,
                Balance = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Player = player
            });
        }

        var entry = await ctx.WhoApplied.FirstOrDefaultAsync(x => x.playerId == player.Id);
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
            .ThenInclude(p => p.Roles)
            .Where(u => u.status == "Pending" || u.createdAt == u.updatedAt)
            .ToListAsync();

        return applied.Select(a => new PlayerWhoAppliedDto
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
                CreatedAt = DateTimeHelper.ToCopenhagen(a.Player.CreatedAt),
                Roles = a.Player.Roles.Select(r => r.Name).ToList()
            }
        }).ToList();
    }

    public Task<AdminDto> RegisterAdmin(CreateAdminDto createAdminDto)
        => throw new NotImplementedException();

    public Task<UserDto> UpdateUser(UpdateUserDetailsDto updateUserDto)
        => throw new NotImplementedException();

    public Task DeleteUser(Guid userId)
        => throw new NotImplementedException();

    public async Task<UserDto> AssignRoleToUserByIdAsync(UserRole userRole, Guid userId)
    {
        if (userRole == UserRole.SuperAdmin)
            throw new UnauthorizedAccessException("SuperAdmin role cannot be assigned");

        var user = await ctx.Users.Include(u => u.Roles)
                       .SingleOrDefaultAsync(u => u.Id == userId)
                   ?? throw new ServiceException("User not found");

        var role = await ctx.Roles.SingleOrDefaultAsync(r => r.Name == userRole)
                   ?? throw new ServiceException("Role not found");

        if (user.Roles.Any(r => r.Id == role.Id))
            throw new ServiceException("Role already assigned");

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
            UpdatedAt = DateTimeHelper.ToCopenhagen(user.UpdatedAt),
            Roles = user.Roles.Select(r => r.Name).ToList()
        };
    }

    private static string GeneratePassword(int length = 6)
    {
        const string chars =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()-_=+[]{}<>?";

        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[length];
        rng.GetBytes(bytes);

        return new string(bytes.Select(b => chars[b % chars.Length]).ToArray());
    }
    
    private static DateTime ConvertStringToDateTime(string date) => DateTime.Parse(date, CultureInfo.InvariantCulture,
        DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
}