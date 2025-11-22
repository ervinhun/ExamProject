using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Api.Configuration;
using Api.Dto.Auth.Request;
using Api.Dto.Auth.Response;
using DataAccess;
using DataAccess.Entities.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Utils.Exceptions;
using Utils;

namespace Api.Services.Auth;

public class Jwt(IOptions<JwtOptions> options, MyDbContext ctx): IJwt
{
    private readonly JwtOptions _jwt = options.Value;
    
    public async Task<JwtResponseDto> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequest)
    {
        var user = await ValidateRefreshTokenForUserIdAsync(refreshTokenRequest.UserId, refreshTokenRequest.RefreshToken);

        if (user == null)
        {
            throw new AuthenticationException("User not found while refreshing token");
        }
        
        return await CreateTokenResponse(user);
    }
    

    public async Task<JwtResponseDto> CreateTokenResponse(User user)
    {
        return new JwtResponseDto
        {
            AccessToken = GenerateAccessToken(user),
            RefreshToken = await GenerateAndSaveRefreshTokenAsync(user),
            user = new UserRepsonseDto(user)
        };
    }

    private async Task<User?> ValidateRefreshTokenForUserIdAsync(Guid userId, string refreshToken)
    {
        var user = await ctx.Users.FindAsync(userId);
        if (user == null)
        {
            return null;
        }
        
        if (user.RefreshTokenHash != HashUtils.HashRefreshToken(refreshToken) || user.RefreshTokenExpires < DateTime.UtcNow)
        {
            throw new AuthenticationException("Invalid refresh token");
        }

        return user;
    }
    
    private async Task<string> GenerateAndSaveRefreshTokenAsync(User user)
    {
        var refreshToken = HashUtils.GenerateRefreshToken(); 
        user.RefreshTokenHash = HashUtils.HashRefreshToken(refreshToken);
        user.RefreshTokenExpires = DateTime.UtcNow.AddDays(7);
        await ctx.SaveChangesAsync();
        return refreshToken;
    }

    private string GenerateAccessToken(User user)
    {
        /* Set up the claims JWT Token will return */
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
        };

        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Name.ToString()));
        }
        
        /*  Symmetric key -> same key used for signing and verifying */
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Secret));
        /*  Signing credentials = key + hashing algorithm */
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            audience: _jwt.Audience,
            issuer: _jwt.Issuer,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwt.ExpiryMinutes),
            signingCredentials: credentials
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}