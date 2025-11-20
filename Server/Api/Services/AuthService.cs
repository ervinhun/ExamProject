using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Api.Dto.Auth;
using Api.Dto.Auth.Request;
using Api.Dto.Auth.Response;
using Api.Options;
using DataAccess;
using DataAccess.Entities.Auth;
using JWT;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Serializers;
using Microsoft.Extensions.Options;
using Utils.Exceptions;
using Utils.Json;
using DataAnnotations_ValidationException = System.ComponentModel.DataAnnotations.ValidationException;
using ValidationException = Bogus.ValidationException;

namespace api.Services;

public class AuthService(
    MyDbContext ctx,
    ILogger<AuthService> logger,
    TimeProvider timeProvider,
    IOptions<JwtOptions> options) : IAuthService
{
    private readonly JwtOptions _jwtOptions = options.Value;
    
    public async Task<JwtClaims> VerifyAndDecodeToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new AuthenticationException("No token attached!");

        var builder = CreateJwtBuilder();

        string jsonString;
        try
        {
            jsonString = builder.Decode(token);
            if (jsonString == null)
            {
                throw new AuthenticationException("Authentication failed! Token not found!");
            }
        }
        catch (AuthenticationException e)
        {
            logger.LogError(e.Message, e);
            throw new AuthenticationException("Failed to verify JWT");
        }

        var jwtClaims = JsonSerializer.Deserialize<JwtClaims>(jsonString, JsonSettings.DefaultOptions);
        if (jwtClaims == null)
        {
            throw new AuthenticationException("Authentication failed! No token attached!");
        }

        _ = ctx.Libraryusers.FirstOrDefault(u => u.Id == jwtClaims.Id)
            ?? throw new ValidationException("Authentication is valid, but user is not found!");

        return jwtClaims;
    }

    public async Task<JwtResponse> Login(LoginRequestDto dto)
    {
        var user = ctx.Libraryusers.FirstOrDefault(u => u.Email == dto.Email)
                   ?? throw new ValidationException("User is not found!");
        var passwordsMatch = user.Passwordhash ==
                             SHA512.HashData(
                                     Encoding.UTF8.GetBytes(dto.Password + user.Salt))
                                 .Aggregate("", (current, b) => current + b.ToString("x2"));
        if (!passwordsMatch)
            throw new AuthenticationException("Password is incorrect!");

        var token = CreateJwt(user);
        return new JwtResponse(token);
    }

    public async Task<JwtResponse> Register(RegisterRequestDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        var isEmailTaken = ctx.Users.Any(u => u.Email.Equals(dto.Email));
        if (isEmailTaken)
            throw new AuthenticationException("Email is already taken");

        var salt = Guid.NewGuid().ToString();
        var hash = SHA512.HashData(
            Encoding.UTF8.GetBytes(dto.Password + salt));
        var user = new User()
        {
            Email = dto.Email,
            //Createdat = timeProvider.GetUtcNow().DateTime.ToUniversalTime(),
            Id = Guid.NewGuid(),
            PasswordSalt = salt,
            PasswordHash = hash.Aggregate("", (current, b) => current + b.ToString("x2")),
            //Role = "User"
        };
        ctx.Users.Add(user);
        
        await ctx.SaveChangesAsync();

        var token = CreateJwt(user);
        return new JwtResponse(token);
    }

    private JwtBuilder CreateJwtBuilder()
    {
        return JwtBuilder.Create()
            .WithAlgorithm(new HMACSHA512Algorithm())
            .WithSecret(_jwtOptions.Secret)
            .WithUrlEncoder(new JwtBase64UrlEncoder())
            .WithJsonSerializer(new JsonNetSerializer())
                .MustVerifySignature();
    }

    private string CreateJwt(User user)
    {
        return CreateJwtBuilder()
            .AddClaim(nameof(user.Id), user.Id)
            .AddClaim(nameof(user.Email), user.Email)
            .Encode();
    }
}