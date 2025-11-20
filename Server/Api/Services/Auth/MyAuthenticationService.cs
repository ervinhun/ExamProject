using Api.Dto.Auth.Request;
using Api.Dto.Auth.Response;
using Api.Services.Auth;
using DataAccess;
using DataAccess.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Utils;
using Utils.Exceptions;
namespace api.Services.Auth;

public class MyAuthenticationService(MyDbContext ctx, IJwt jwt) : IMyAuthenticationService
{

    public async Task<JwtResponseDto> Login(LoginRequestDto dto)
    {
        var user = await ctx.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        
        if (user == null || !HashUtils.VerifyPasswordHash(dto.Password, user.PasswordHash, user.PasswordSalt))
        {
            throw new AuthenticationException("Email or password is incorrect");
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

        HashUtils.CreatePasswordHash(dto.Password, out byte[] hash, out var salt); 
        
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
}
