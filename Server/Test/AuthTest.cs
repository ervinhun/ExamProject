using System.Data;
using Api.Dto.Auth.Request;
using api.Services;
using DataAccess;
using Test.Util;
using Utils.Exceptions;

namespace Test;

public class AuthTest(
    MyDbContext ctx,
    ISeeder seeder,
    IMyAuthenticationService authService)
{
    
    [Fact]
    public async Task LoginReturnsJwt()
    {
        var result = await authService.Login(new LoginRequestDto
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
            authService.Login(new LoginRequestDto { Email = "admin@admin.com", Password = "" }));
        await Assert.ThrowsAsync<AuthenticationException>(() => authService.Login(new LoginRequestDto
            { Email = "admin@admin.com", Password = "DefinitelyNotTheRightPassword" }));
        await Assert.ThrowsAsync<AuthenticationException>(() =>
            authService.Login(new LoginRequestDto { Email = "", Password = "admin" }));
        await Assert.ThrowsAsync<AuthenticationException>(() => authService.Login(new LoginRequestDto
            { Email = "not.existing.email@email.com", Password = "SuperStrongPassword1234" }));
    }


    [Fact]
    public async Task RegisterReturnsJwtWhichCanVerifyAgain()
    {
        var result = await authService.Register(new RegisterRequestDto
        {
            FirstName = "Test",
            LastName = "Testsson",
            Dob = new DateTime(1990, 01, 01),
            Email = "test@email.dk",
            Password = "asædkjlsadjsadjlksad"
        });

        Assert.NotNull(result);
        Assert.Equal("Test", result.FirstName);
        Assert.Equal("Testsson", result.LastName);
        Assert.Equal(DateTime.Parse("1990-01-01"), result.DateOfBirth);
        Assert.False(result.Activated);
        Assert.False(result.IsDeleted);
        Assert.Equal(result.CreatedAt, result.UpdatedAt);
        Assert.Equal("test@email.dk", result.Email);
        Assert.NotNull(result.PasswordHash);
        Assert.NotNull(result.PasswordSalt);
        Assert.Null(result.ResetPasswordToken);
        Assert.Null(result.ResetPasswordTokenExpiry);
    }

    [Fact]
    public async Task RegisterWhenUserAlreadyExistsThrowsException()
    {
        await Assert.ThrowsAsync<DuplicateNameException>(() => authService.Register(
            new RegisterRequestDto
            {
                Email = "admin@admin.com",
                Password = "admin",
                FirstName = "Admin",
                LastName = "Adminsson",
                Dob = new DateTime(1990, 01, 01)
            }));
    }
}