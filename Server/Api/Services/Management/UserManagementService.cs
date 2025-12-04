using System.Security.Cryptography;
using Api.Dto.test;
using Api.Dto.User;
using Api.Services.Email;
using DataAccess;
using DataAccess.Entities.Auth;
using DataAccess.Entities.Finance;
using DataAccess.Enums;
using Microsoft.AspNetCore.Identity.Data;
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
        var role = await AssignUserAsPlayer(player);
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
            if(user == null) throw new ServiceException("User not found", new InvalidOperationException());
            
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
            if(player == null)throw new ServiceException("Player not found", new InvalidOperationException());
            
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

    public async Task<string> RequestPasswordReset(string email)
    {
        var user = await ctx.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
            throw new ServiceException("User not found", new InvalidOperationException());
        var passwordResetToken = GeneratePassword(128);
        HashUtils.CreatePasswordHash(passwordResetToken, out var hash, out var salt);

        user.ResetPasswordToken = hash.ToString();
        user.ResetPasswordTokenExpiry = DateTime.UtcNow.AddHours(12);
        await ctx.SaveChangesAsync();
        // Returning the ResetPasswordToken to the controller to send email
        // TODO: sending e-mail part in Controller
        return passwordResetToken;
    }

    public async Task<ActionResult<User>> RequestMembership(RequestRegistrationDto requestRegistrationDto)
    {
        if (await ctx.Users.AnyAsync(u => u.Email == requestRegistrationDto.Email))
        {
            throw new ServiceException("Email already exists", new InvalidOperationException());
        }

        HashUtils.CreatePasswordHash(requestRegistrationDto.Password, out var hash, out var salt);

        // Get the Player role from database
        var playerRole = await ctx.Roles.FirstOrDefaultAsync(r => r.Name == UserRole.Player);
        if (playerRole == null)
        {
            throw new ServiceException("Player role not found in database", new InvalidOperationException());
        }

        var request = new Player
        {
            FirstName = requestRegistrationDto.FirstName,
            LastName = requestRegistrationDto.LastName,
            Email = requestRegistrationDto.Email,
            PhoneNumber = requestRegistrationDto.PhoneNo,
            PasswordHash = hash,
            PasswordSalt = salt,
            Activated = false
        };

        // TODO: finish with the requestedmembership table

        try
        {
            await ctx.Players.AddAsync(request);
            await ctx.SaveChangesAsync();
        }
        catch (DbUpdateException e)
        {
            throw new ServiceException(e.Message, e);
        }

        return request;
    }

    public async Task<PlayerDto> ConfirmMembership(Guid userId, UserConfirmationEntity userConfirmationEntity)
    {
        Player player;
        var admin = CheckWhoIsLoggedInFromToken(userConfirmationEntity.ConfirmationToken ?? string.Empty);
        if (admin.Roles.Any(r => r.Name != UserRole.SuperAdmin || r.Name != UserRole.Admin))
            throw new UnauthorizedAccessException("Only admins can confirm membership");

        // Admin can assign only Player role to a request. User can not request for other roles.
        if (Enum.TryParse<UserRole>(userConfirmationEntity.Role, out var parsedRole) &&
            parsedRole == UserRole.Player)
        {
            var user = await ctx.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                throw new ServiceException("User not found");

            // Assign role and wallet
            player = GetPlayerFromUser(user);
            var role = await AssignUserAsPlayer(player);
            player.Roles.Add(role);
            player.UpdatedAt = DateTime.UtcNow;
            player.Activated = userConfirmationEntity.isActive;

            // Update WhoApplied table
            var entry = await ctx.WhoApplied
                .FirstOrDefaultAsync(x => x.playerId == userConfirmationEntity.PlayerId);

            if (entry != null)
            {
                entry.status = userConfirmationEntity.Result;
                entry.updatedAt = DateTime.UtcNow;
                entry.reviewedBy = admin.Id;
            }

            await ctx.SaveChangesAsync();
        }
        else
        {
            throw new ServiceException("Invalid role in request");
        }

        return GetPlayerDtoFromPlayer(player);
    }

    public async Task<bool> ResetPassword(string resetToken, ResetPasswordRequest request)
    {
        var user = await ctx.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null)
            return false;

        var tokenValid =
            user.ResetPasswordToken == resetToken &&
            user.ResetPasswordTokenExpiry > DateTime.UtcNow;

        if (!tokenValid)
            return false;

        // Hash new password
        HashUtils.CreatePasswordHash(request.NewPassword, out var hash, out var salt);
        user.PasswordHash = hash;
        user.PasswordSalt = salt;

        // Clear the reset token
        user.ResetPasswordToken = null;
        user.ResetPasswordTokenExpiry = null;

        return await ctx.SaveChangesAsync() > 0;
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

    private User CheckWhoIsLoggedInFromToken(string token)
    {
        return ctx.Users.Include(u => u.Roles).SingleOrDefault(u => u.RefreshTokenHash == token)
               ?? throw new UnauthorizedAccessException();
    }

    private async Task<Role> AssignUserAsPlayer(User user)
    {
        var playerRole = await ctx.Roles
            .FirstOrDefaultAsync(r => r.Name == UserRole.Player);

        if (playerRole == null)
            throw new ServiceException("Player role not found in database");

        var player = new Player
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Activated = true,
            CreatedAt = user.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };
        var wallet = new Wallet
        {
            Player = player,
            Balance = 0
        };
        await ctx.Wallets.AddAsync(wallet);
        return playerRole;
    }
}