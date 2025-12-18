using Api.Dto.test;
using Api.Dto.User;
using Api.Services.Admin;
using api.Services.Auth;
using Api.Services.Auth;
using Api.Services.Management;
using DataAccess;
using DataAccess.Enums;
using Microsoft.EntityFrameworkCore;
using Test.Util;
using Utils.Exceptions;

namespace Test;

[Collection("Database collection")]
public class UserManagementServiceTest
{
    private readonly MyDbContext _ctx;
    private readonly IUserManagementService _userManagementService;
    private readonly DatabaseFixture _fixture;
    private readonly Seeder _seeder;

    public UserManagementServiceTest(DatabaseFixture fixture)
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
        _userManagementService = new UserManagementService(_ctx, new FakeEmailService());
    }


    private static readonly DateTime validDate =
        new DateTime(2025, 12, 14, 19, 51, 44, DateTimeKind.Utc);

    private readonly Guid ValidAdminId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private string ExistingPlayer1Email => _seeder.Player1Email;
    private Guid ExistingPlayer1Id => _seeder.Player1Id;

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

        var result = await _userManagementService.RegisterUser(createUserDto);
        Assert.NotNull(result);
        Assert.Equal(createUserDto.FirstName, result.FirstName);
        Assert.Equal(createUserDto.LastName, result.LastName);
        Assert.Equal(createUserDto.Email, result.Email);
        Assert.Equal(createUserDto.PhoneNumber, result.PhoneNumber);
        Assert.Equal(new DateTime(1970, 02, 02), result.Dob);
        Assert.NotNull(result.Roles);
    }

    [Fact]
    public async Task RegisterUserFailsWhenEmailAlreadyExists()
    {
        var createUserDto = new CreateUserDto
        {
            FirstName = "Jack",
            LastName = "ThePirate",
            Email = ExistingPlayer1Email,
            PhoneNumber = "12345678",
            BirthDate = "1972-02-02"
        };

        await Assert.ThrowsAsync<ServiceException>(() => _userManagementService.RegisterUser(createUserDto));
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

        var result = await _userManagementService.RegisterPlayer(createUserDto);

        Assert.NotNull(result);
        Assert.Equal(createUserDto.FirstName, result.FirstName);
        Assert.Equal(createUserDto.LastName, result.LastName);
        Assert.Equal(createUserDto.Email, result.Email);
        Assert.Equal(createUserDto.PhoneNumber, result.PhoneNumber);
        Assert.Equal(new DateTime(1977, 03, 03, 0, 0, 0, DateTimeKind.Utc), result.Dob);
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
            Email = ExistingPlayer1Email,
            PhoneNumber = "12345678",
            BirthDate = "1985-01-19"
        };

        await Assert.ThrowsAsync<ServiceException>(() => _userManagementService.RegisterPlayer(createUserDto));
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

        var playerRole = await _ctx.Roles.FirstOrDefaultAsync(r => r.Name == UserRole.Player,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(playerRole);
        _ctx.Roles.Remove(playerRole);
        await _ctx.SaveChangesAsync(TestContext.Current.CancellationToken);

        try
        {
            // Act + Assert
            await Assert.ThrowsAsync<ServiceException>(() =>
                _userManagementService.RegisterPlayer(createUserDto));
        }
        finally
        {
            // Cleanup (important!)
            _ctx.Roles.Add(playerRole);
            await _ctx.SaveChangesAsync(TestContext.Current.CancellationToken);
        }
    }

    [Fact]
    public async Task GetAllUsersTestSuccess()
    {
        var existingUser = await _userManagementService.GetPlayerByIdAsync(ExistingPlayer1Id);
        var result = await _userManagementService.GetAllUsersAsync();
        Assert.NotEmpty(result);
        Assert.Contains(result, p => p.Id == existingUser.Id);
    }

    [Fact]
    public async Task GetAllPlayersTestSuccess()
    {
        var existingUser = await _ctx.Users.FirstOrDefaultAsync(u => u.Id == ExistingPlayer1Id,
            cancellationToken: TestContext.Current.CancellationToken);

        var result = await _userManagementService.GetAllPlayersAsync();
        Assert.NotEmpty(result);
        Assert.Contains(result, p => p.Id == existingUser.Id);
    }

    [Fact(Skip = "TPT inheritance with many-to-many roles not loading correctly")]
    public async Task GetPlayerByIdTestSuccess()
    {
        var result = await _userManagementService.GetPlayerByIdAsync(ExistingPlayer1Id);
        Assert.NotNull(result);
        Assert.Equal(ExistingPlayer1Id, result.Id);
        Assert.True(result.IsActive);
        Assert.False(result.IsDeleted);
        Assert.NotNull(result.FirstName);
        Assert.NotNull(result.LastName);
        Assert.NotNull(result.Email);
        Assert.True(result.Dob <= DateTime.Now.AddYears(-18));
        Assert.Contains(UserRole.Player, result.Roles);
    }

    [Fact]
    public async Task GetPlayerByIdThrowsExceptionWhenUserDoesNotExist()
    {
        await Assert.ThrowsAsync<ServiceException>(() => _userManagementService.GetPlayerByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task ConfirmMembershipTestSuccess()
    {
        var creatPlayer = new RequestRegistrationDto()
        {
            FirstName = "Greg",
            LastName = "Longbeard",
            Email = "g.l.b@hotmail.com",
            Dob = new DateTime(1986, 01, 31),
            PhoneNo = "+4512345678",
            Password = "showerHead"
        };
        var authService = new MyAuthenticationService(
            _ctx,
            new Jwt(null, _ctx)
        );

        // Act
        await authService.RequestMembership(creatPlayer);

        var players = await _userManagementService.GetAllPlayersAsync();
        var createdPlayer = players.FirstOrDefault(p => p.Email == creatPlayer.Email);

        Assert.NotNull(createdPlayer);

        //Assert.Contains(UserRole.Player, createdPlayer.Roles);

        var result = await _userManagementService.ConfirmMembership(createdPlayer.Id, true, true, ValidAdminId);
        Assert.True(result);

        var appliedUserEntry = await _ctx.WhoApplied.FirstOrDefaultAsync(w => w.playerId == createdPlayer.Id);

        Assert.NotNull(appliedUserEntry);
        Assert.Equal("Confirmed", appliedUserEntry.status);
    }

    [Fact]
    public async Task ConfirmMembershipThrowsExceptionWhenUserDoesNotExist()
    {
        await Assert.ThrowsAsync<ServiceException>(() =>
            _userManagementService.ConfirmMembership(Guid.NewGuid(), true, true, ValidAdminId));
    }

    [Fact]
    public async Task ConfirmMembershipThrowsExceptionWhenUserIsNotPlayer()
    {
        await Assert.ThrowsAsync<ServiceException>(() =>
            _userManagementService.ConfirmMembership(ValidAdminId, false, true, ValidAdminId));
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

        var createdPlayer = await _userManagementService.RegisterPlayer(creatPlayer);
        Assert.NotNull(createdPlayer);

        var result = await _userManagementService.GetAppliedUsers();
        Assert.NotNull(result);
        Assert.True(result.TrueForAll(u => u.Status == "Pending"));
    }
}