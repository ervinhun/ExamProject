using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Api.Configuration;
using Api.Dto.Auth.Request;
using Api.Dto.Auth.Response;
using DataAccess;
using DataAccess.Entities.Auth;
using DataAccess.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Utils.Exceptions;
using Utils;

namespace Api.Services.Auth;

public class Jwt(JwtSettings jwtSettings, MyDbContext ctx): IJwt
{
    public async Task<JwtResponseDto> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequest)
    {
        try
        {
            var user = await ValidateRefreshTokenForUserIdAsync(Guid.Parse(refreshTokenRequest.UserId), refreshTokenRequest.RefreshToken);
            if (user != null) return await CreateTokenResponse(user);
        }
        catch (AuthenticationException e)
        {
            throw new ServiceException(e.Message, e);
        }
        return null!;
    }
    

    public async Task<JwtResponseDto> CreateTokenResponse(User user)
    {
        var accessToken = GenerateAccessToken(user);
        return new JwtResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = await GenerateAndSaveRefreshTokenAsync(user) ?? user.RefreshTokenHash,
            User = new UserResponseDto()
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = user.Roles
                    .Select(r => r.Name)
                    .ToList()
            }
        };
        
    }

    private async Task<User?> ValidateRefreshTokenForUserIdAsync(Guid userId, string refreshToken)
    {
        // Must include Roles to generate new access token with roles claims
        var user = await ctx.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == userId);
            
        if(user is null) throw new AuthenticationException("User not found while refreshing token");
        
        // Hash the incoming token to compare with stored hash
        var hashedRefreshToken = HashUtils.HashRefreshToken(refreshToken);
        if(user.RefreshTokenHash != hashedRefreshToken) throw new AuthenticationException("Failed while validating refreshToken");
        
        if(user.RefreshTokenExpires < DateTime.UtcNow) throw new AuthenticationException("Refresh token expired");
        
        return user;
    }
    
    private async Task<string?> GenerateAndSaveRefreshTokenAsync(User user)
    {
        var refreshToken = HashUtils.GenerateRefreshToken(); 
        user.RefreshTokenHash = HashUtils.HashRefreshToken(refreshToken);
        user.RefreshTokenExpires = DateTime.UtcNow.AddDays(jwtSettings.RefreshTokenDays);
        await ctx.SaveChangesAsync();
        return refreshToken;
    }

    private string GenerateAccessToken(User user)
    {
        /* Set up the claims JWT Token will return */
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FirstName),
            new Claim(ClaimTypes.Surname, user.LastName),
            new Claim(ClaimTypes.DateOfBirth, DateTimeHelper.ToCopenhagen(user.DateOfBirth).ToString(CultureInfo.CurrentCulture)),
            new Claim(ClaimTypes.Expired, (!user.Activated).ToString(CultureInfo.CurrentCulture)),
        };

        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Name.ToString().ToLower()));
        }
        
        /*  Symmetric key -> same key used for signing and verifying */
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret));
        /*  Signing credentials = key + hashing algorithm */
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            audience: jwtSettings.Audience,
            issuer: jwtSettings.Issuer,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwtSettings.ExpirationMinutes),
            signingCredentials: credentials
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}