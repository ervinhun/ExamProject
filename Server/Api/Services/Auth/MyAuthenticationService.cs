using System.Security.Cryptography;
using Api.Dto.Auth.Request;
using Api.Dto.Auth.Response;
using Api.Dto.User;
using Api.Services.Auth;
using DataAccess;
using DataAccess.Entities.Auth;
using Microsoft.AspNetCore.Identity.Data;
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
            throw new AuthenticationException("Email or password is incorrect.");
        }
        return await jwt.CreateTokenResponse(user); 
    }
    
    public async Task<User> Register(RegisterRequestDto dto)
    {
        var user = await ctx.Users.AnyAsync(u=> u.Email == dto.Email);
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
}
