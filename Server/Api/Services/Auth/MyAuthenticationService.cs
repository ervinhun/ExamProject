using System.Security.Cryptography;
using Api.Dto.Auth.Request;
using Api.Dto.Auth.Response;
using Api.Dto.User;
using Api.Services.Auth;
using DataAccess;
using DataAccess.Entities.Auth;
using DataAccess.Enums;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utils;
using Utils.Exceptions;

namespace api.Services.Auth;

public class MyAuthenticationService(MyDbContext ctx, IJwt jwt) : IMyAuthenticationService
{
    public async Task<JwtResponseDto> Login(LoginRequestDto dto)
    {
        var user = await ctx.Users.Include(user => user.Roles).FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null || !HashUtils.VerifyPasswordHash(dto.Password, user.PasswordHash, user.PasswordSalt))
        {
            Console.Out.WriteLine("Invalid credentials");
            throw new AuthenticationException("Email or password is incorrect");
        }

        return await jwt.CreateTokenResponse(user);
    }

    public async Task<User> Register(RegisterRequestDto dto)
    {
        var user = await ctx.Users.AnyAsync(u => u.Email == dto.Email);
        if (user)
        {
            throw new AuthenticationException("User already exists");
        }

        HashUtils.CreatePasswordHash(dto.Password, out var hash, out var salt);

        var newUser = new User
        {
            Email = dto.Email,
            PasswordHash = hash,
            PasswordSalt = salt,
        };

        ctx.Users.Add(newUser);

        await ctx.SaveChangesAsync();

        return newUser;
    }

    public async Task<string> RequestPasswordReset(string email)
    {
        var user = await ctx.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
            throw new ServiceException("User not found", new InvalidOperationException());
        var passwordResetToken = GeneratePasswordToken(128);
        HashUtils.CreatePasswordHash(passwordResetToken, out var hash, out var salt);

        user.ResetPasswordToken = Convert.ToBase64String(hash);
        user.ResetPasswordTokenExpiry = DateTime.UtcNow.AddHours(12);
        await ctx.SaveChangesAsync();
        // Returning the ResetPasswordToken to the controller to send email
        // TODO: sending e-mail part in Controller
        return passwordResetToken;
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

    private static string GeneratePasswordToken(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var data = new byte[length];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(data);

        var result = new char[length];
        for (int i = 0; i < length; i++)
        {
            result[i] = chars[data[i] % chars.Length];
        }

        return new string(result);
    }

    public async Task<bool> RequestMembership(RequestRegistrationDto requestRegistrationDto)
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

        var user = new Player
        {
            FirstName = requestRegistrationDto.FirstName,
            LastName = requestRegistrationDto.LastName,
            Email = requestRegistrationDto.Email,
            PhoneNumber = requestRegistrationDto.PhoneNo,
            PasswordHash = hash,
            PasswordSalt = salt,
            Activated = false
        };

        // Assign player role
        var role = await AssignUserAsPlayer();
        user.Roles.Add(role);

        try
        {
            await ctx.Players.AddAsync(user);
            await ctx.SaveChangesAsync();
        }
        catch (DbUpdateException e)
        {
            throw new ServiceException(e.Message, e);
        }

        var userId = ctx.Users.FirstOrDefaultAsync(u => u.Email == requestRegistrationDto.Email).Result.Id;
        if (user == null)
            throw new ServiceException("Player not found", new InvalidOperationException());
        var utcNow = DateTime.UtcNow;
        var request = new PlayerWhoApplied()
        {
            Player = user,
            playerId = user.Id,
            createdAt = utcNow,
            updatedAt = utcNow,
            status = "Pending"
        };

        ctx.WhoApplied.Add(request);
        await ctx.SaveChangesAsync();
        return true;
    }

    private async Task<Role> AssignUserAsPlayer()
    {
        var playerRole = await ctx.Roles
            .FirstOrDefaultAsync(r => r.Name == UserRole.Player);

        if (playerRole == null)
            throw new ServiceException("Player role not found in database");

        return playerRole;
    }
}