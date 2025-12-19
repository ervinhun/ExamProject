using Api.Configuration;
using Api.Dto.Auth.Request;
using Api.Dto.User;
using Api.Services.Auth;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Test.Util;
using Utils.Exceptions;

namespace Test;

[Collection("Database collection")]
public class AuthTest
{
    private readonly MyDbContext _ctx;
    private readonly MyAuthenticationService _authService;
    private readonly DatabaseFixture _fixture;
    private readonly Seeder _seeder;

    public AuthTest(DatabaseFixture fixture)
    {
        _fixture = fixture;

        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseNpgsql(fixture.ConnectionString)
            .Options;

        _ctx = new MyDbContext(options);

        //var seeder = new Seeder(_ctx);
        _seeder = new Seeder(_ctx);
        _seeder.Seed().GetAwaiter().GetResult();

        // Construct dependencies MANUALLY
        var jwtSettings = new JwtSettings
        {
            Secret = "SuperSecretJwtSigningKey_32Chars!",
            Issuer = "localhost",
            Audience = "localhost",
            ExpirationMinutes = 30,
            RefreshTokenDays = 1
        };
        _authService = new MyAuthenticationService(_ctx, new Jwt(jwtSettings, _ctx));
    }

    [Fact]
    public async Task LoginReturnsJwt()
    {
        var result = await _authService.Login(new LoginRequestDto
        {
            Email = "admin@admin.com",
            Password = "admin"
        });

        Assert.NotNull(result);
        Assert.NotNull(result.AccessToken);
        Assert.NotNull(result.RefreshToken);
        Assert.NotNull(result.User);
        Assert.Equal("Admin", result.User.FirstName);
        Assert.Equal("Adminsson", result.User.LastName);
        Assert.Equal("admin@admin.com", result.User.Email);
    }

    [Fact]
    public async Task LoginThrowsExceptionWhenPasswordIsWrongOrEmpty()
    {
        await Assert.ThrowsAsync<AuthenticationException>(() =>
            _authService.Login(new LoginRequestDto { Email = "admin@admin.com", Password = "" }));
        await Assert.ThrowsAsync<AuthenticationException>(() => _authService.Login(new LoginRequestDto
            { Email = "admin@admin.com", Password = "DefinitelyNotTheRightPassword" }));
        await Assert.ThrowsAsync<AuthenticationException>(() =>
            _authService.Login(new LoginRequestDto { Email = "", Password = "admin" }));
        await Assert.ThrowsAsync<AuthenticationException>(() => _authService.Login(new LoginRequestDto
            { Email = "not.existing.email@email.com", Password = "SuperStrongPassword1234" }));
    }


    [Fact]
    public async Task RegisterReturnsJwtWhichCanVerifyAgain()
    {
        var userDob = new DateTime(1990, 01, 01, 0, 0, 0, DateTimeKind.Utc);
        
        var resultBool = await _authService.RequestMembership(new RequestRegistrationDto()
        {
            FirstName = "Test",
            LastName = "Testsson",
            Dob = userDob,
            Email = "test99@email.dk",
            PhoneNo = "+4512345678",
            Password = "asædkjlsadjsadjlksad"
        });
        
        Assert.True(resultBool);

        var result = await _ctx.Players
            .FirstOrDefaultAsync(p => p.Email == "test99@email.dk");


        Assert.NotNull(result);
        Assert.Equal("Test", result.FirstName);
        Assert.Equal("Testsson", result.LastName);
        Assert.Equal(userDob, result.DateOfBirth);
        Assert.False(result.Activated);
        Assert.False(result.IsDeleted);
        Assert.Equal("test99@email.dk", result.Email);
        Assert.NotNull(result.PasswordHash);
        Assert.NotNull(result.PasswordSalt);
        Assert.Null(result.ResetPasswordToken);
        Assert.Null(result.ResetPasswordTokenExpiry);
    }

    [Fact]
    public async Task RegisterWhenUserAlreadyExistsThrowsException()
    {
        await Assert.ThrowsAsync<AuthenticationException>(() => _authService.Register(
            new RegisterRequestDto
            {
                Email = "admin@admin.com",
                Password = "admin",
                FirstName = "Admin",
                LastName = "Adminsson",
                Dob = new DateTime(1990, 01, 01, 0, 0, 0, DateTimeKind.Utc)
            }));
    }
}