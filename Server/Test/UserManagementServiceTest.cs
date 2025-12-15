using Api.Dto.test;
using Api.Dto.User;
using Api.Services.Management;
using DataAccess;
using DataAccess.Enums;
using Microsoft.EntityFrameworkCore;
using Test.Util;
using Utils;
using Utils.Exceptions;

namespace Test;

public class UserManagementServiceTest(MyDbContext ctx, ISeeder seeder, IUserManagementService userManagementService)
{
    private static readonly DateTime validDate = new DateTime(2025, 12, 14, 19, 51, 44);
    private readonly Guid ValidAdminId = new Guid("1");

    private readonly UserDto ExistingUser = new UserDto
    {
        Id = Guid.NewGuid(),
        FirstName = "John",
        LastName = "Doe",
        Email = "j.d@hotmail.com",
        Dob = new DateTime(1985, 01, 19),
        Roles = new List<UserRole> { UserRole.Player },
        IsDeleted = false,
        CreatedAt = validDate,
        UpdatedAt = validDate
    };
    
    [Fact]
    public async Task RegisterUserTestSuccess()
    {
        var createUserDto = new CreateUserDto
        {
            FirstName = "Alfred",
            LastName = "MoneyBag",
            Email = "alfred@hotmail.com",
            PhoneNumber = "12345678",
            BirthDate = "1970-02-02"
        };

        var result = await userManagementService.RegisterUser(createUserDto);
        Assert.NotNull(result);
        Assert.Equal(createUserDto.FirstName, result.FirstName);
        Assert.Equal(createUserDto.LastName, result.LastName);
        Assert.Equal(createUserDto.Email, result.Email);
        Assert.Equal(createUserDto.PhoneNumber, result.PhoneNumber);
        Assert.Equal(new DateTime(1970, 02, 02), result.Dob);
        Assert.NotNull(result.Roles);
        Assert.False(result.IsDeleted);
    }

    [Fact]
    public async Task RegisterUserFailsWhenEmailAlreadyExists()
    {
        var createUserDto = new CreateUserDto
        {
            FirstName = "Jack",
            LastName = "ThePirate",
            Email = ExistingUser.Email,
            PhoneNumber = "12345678",
            BirthDate = "1970-02-02"
        };

        await Assert.ThrowsAsync<ServiceException>(() => userManagementService.RegisterUser(createUserDto));
    }

    [Fact]
    public async Task RegisterPlayerTestSucces()
    {
        var createUserDto = new CreatePlayerDto
        {
            FirstName = "Jack",
            LastName = "ThePirate",
            Email = "jack@hotmail.com",
            PhoneNumber = "12345678",
            BirthDate = "1977-03-03"
        };
        
        var result = await userManagementService.RegisterPlayer(createUserDto);
        
        Assert.NotNull(result);
        Assert.Equal(createUserDto.FirstName, result.FirstName);
        Assert.Equal(createUserDto.LastName, result.LastName);
        Assert.Equal(createUserDto.Email, result.Email);
        Assert.Equal(createUserDto.PhoneNumber, result.PhoneNumber);
        Assert.Equal(new DateTime(1977, 03, 03), result.Dob);
        Assert.NotNull(result.Roles);
        Assert.False(result.IsDeleted);
        Assert.False(result.IsActive);
    }
    
    [Fact]
    public async Task RegisterPlayerFailsWhenEmailAlreadyExists()
    {
        var createUserDto = new CreatePlayerDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = ExistingUser.Email,
            PhoneNumber = "12345678",
            BirthDate = "1985-01-19"
        };
        
        await Assert.ThrowsAsync<ServiceException>(() => userManagementService.RegisterPlayer(createUserDto));
    }

    [Fact]
    public async Task RegisterPlayerThrowsErrorWhenPlayerRoleIsNotInTheDatabase()
    {
        var createUserDto = new CreatePlayerDto
        {
            FirstName = "Fred",
            LastName = "Sailor",
            Email = "fred.s@hotmail.com",
            PhoneNumber = "0987534",
            BirthDate = "1984-06-29"
        };
        
        var playerRole = await ctx.Roles.FirstOrDefaultAsync(r => r.Name == UserRole.Player, cancellationToken: TestContext.Current.CancellationToken);
        
        Assert.NotNull(playerRole);
        ctx.Roles.Remove(playerRole);
        await ctx.SaveChangesAsync(TestContext.Current.CancellationToken);
        
        try
        {
            // Act + Assert
            await Assert.ThrowsAsync<ServiceException>(() =>
                userManagementService.RegisterPlayer(createUserDto));
        }
        finally
        {
            // Cleanup (important!)
            ctx.Roles.Add(playerRole);
            await ctx.SaveChangesAsync(TestContext.Current.CancellationToken);
        }
    }
    
    [Fact]
    public async Task GetAllUsersTestSuccess()
    {
        var result = await userManagementService.GetAllUsersAsync();
        Assert.NotEmpty(result);
        Assert.Contains(ExistingUser, result);
    }
    
    [Fact]
    public async Task GetAllPlayersTestSuccess()
    {
        var result = await userManagementService.GetAllPlayersAsync();
        Assert.NotEmpty(result);
        Assert.Contains(ExistingUser, result);
    }

    [Fact]
    public async Task GetPlayerByIdTestSuccess()
    {
        var result = await userManagementService.GetPlayerByIdAsync(ExistingUser.Id);
        Assert.NotNull(result);
        Assert.Equal(ExistingUser.Id, result.Id);
        Assert.True(result.IsActive);
        Assert.False(result.IsDeleted);
        Assert.NotNull(result.FirstName);
        Assert.NotNull(result.LastName);
        Assert.NotNull(result.Email);
        Assert.True(result.Dob >= DateTime.Now.AddYears(-18));
        Assert.Contains(UserRole.Player, result.Roles);
    }
    
    [Fact]
    public async Task GetPlayerByIdThrowsExceptionWhenUserDoesNotExist()
    {
        await Assert.ThrowsAsync<ServiceException>(() => userManagementService.GetPlayerByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task ConfirmMembershipTestSuccess()
    {
        var creatPlayer = new CreatePlayerDto
        {
            FirstName = "Greg",
            LastName = "Longbeard",
            Email = "g.l.b@hotmail.com",
            PhoneNumber = "12345678",
            BirthDate = "1986-01-31"
        };
        
        var createdPlayer = await userManagementService.RegisterPlayer(creatPlayer);
        Assert.NotNull(createdPlayer);
        Assert.False(createdPlayer.IsActive);
        Assert.False(createdPlayer.IsDeleted);
        Assert.Contains(UserRole.Player, createdPlayer.Roles);
        
        var result = await userManagementService.ConfirmMembership(createdPlayer.Id, true, true, ValidAdminId);
        Assert.True(result);
        
        var appliedUserEntry = await ctx.WhoApplied.FirstOrDefaultAsync(w => w.playerId == createdPlayer.Id);
        
        Assert.NotNull(appliedUserEntry);
        Assert.Equal("Confirmed", appliedUserEntry.status);
    }
    
    [Fact]
    public async Task ConfirmMembershipThrowsExceptionWhenUserDoesNotExist()
    {
        await Assert.ThrowsAsync<ServiceException>(() => userManagementService.ConfirmMembership(Guid.NewGuid(), true, true, ValidAdminId));
    }
    
    [Fact]
    public async Task ConfirmMembershipThrowsExceptionWhenAdminIsNotAdmin()
    {
        await Assert.ThrowsAsync<ServiceException>(() => userManagementService.ConfirmMembership(ExistingUser.Id, true, true, Guid.NewGuid()));
    }
    
    [Fact]
    public async Task ConfirmMembershipThrowsExceptionWhenUserIsNotPlayer()
    {
        await Assert.ThrowsAsync<ServiceException>(() => userManagementService.ConfirmMembership(ValidAdminId, false, true, ValidAdminId));
    }

    [Fact]
    public async Task GetAppliedUserTestsSuccess()
    {
        var creatPlayer = new CreatePlayerDto
        {
            FirstName = "Tim",
            LastName = "Lille",
            Email = "t.l@hotmail.com",
            PhoneNumber = "12345678",
            BirthDate = "1996-11-11"
        };

        var createdPlayer = await userManagementService.RegisterPlayer(creatPlayer);
        Assert.NotNull(createdPlayer);

        var result = await userManagementService.GetAppliedUsers();
        Assert.NotNull(result);
        Assert.Contains(result, u => u.Player.Id == createdPlayer.Id);
        Assert.True(result.TrueForAll(u => u.Status == "Pending"));
    }
    
}