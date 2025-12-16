using DataAccess;
using DataAccess.Entities.Auth;
using DataAccess.Entities.Finance;
using DataAccess.Entities.Game;
using DataAccess.Enums;
using Microsoft.EntityFrameworkCore;
using Utils;

namespace Test.Util;

public class Seeder(MyDbContext context) : ISeeder
{
    public Guid AdminId { get; } = Guid.Parse("00000000-0000-0000-0000-000000000001");
    public Guid AdminRoleId { get; } = Guid.Parse("00000000-0000-0000-0001-000000000001");

    public Guid PlayerRoleId { get; } = Guid.Parse("00000000-0000-0000-0002-000000000002");

    public Guid Player1Id { get; } = Guid.Parse("00000000-0000-0000-0000-000000000002");
    public Guid Player1WalletId { get; } = Guid.Parse("00000000-0000-0000-2000-000000000002");

    public Guid Player2Id { get; } = Guid.Parse("00000000-0000-0000-0000-000000000003");
    public Guid Player2WalletId { get; } = Guid.Parse("00000000-0000-0000-2000-000000000003");

    public Guid UnconfirmedPlayerId { get; } = Guid.Parse("00000000-0000-0000-0000-000000000004");
    public Guid UserWithoutWalletId { get; } = Guid.Parse("00000000-0000-0000-0000-000000000005");

    public Guid GameTemplateId { get; } = Guid.Parse("00000001-0000-0000-0000-000000000000");
    public Guid GameInstanceId { get; } = Guid.Parse("00000001-0000-0000-0000-000000000001");

    public Guid PendingDepositTransactionId { get; } = Guid.Parse("00000000-0000-a001-0000-000000000004");
    public Guid PendingDepositTransactionIdForError { get; } = Guid.Parse("00000000-0000-a001-0000-000000000008");
    public Guid ExistingTransactionId { get; } = Guid.Parse("00000000-0000-a001-0000-000000000005");
    
    public Guid TransactionIdForWithdraw { get; } = Guid.Parse("00000000-0000-aa01-0000-000000000005");

    public Guid TicketId { get; } = Guid.Parse("00000000-0000-a001-0000-000000000006");

    public string AdminEmail => "admin@admin.com";
    public string Player1Email => "player1@player.com";
    public string Player2Email => "player2@player.com";
    public string UnconfirmedPlayerEmail => "unconfirmed@player.com";
    public string UserWithoutWalletEmail => "no-wallet@user.com";

    public async Task Seed()
    {
        if (await context.Users.AnyAsync())
            return;

        // ROLES
        var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == UserRole.Admin)
                        ?? new Role { Id = AdminRoleId, Name = UserRole.Admin };

        var playerRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == UserRole.Player)
                         ?? new Role { Id = PlayerRoleId, Name = UserRole.Player };

        context.Roles.AddRange(adminRole, playerRole);

        // USERS
        HashUtils.CreatePasswordHash("admin", out var adminHash, out var adminSalt);
        HashUtils.CreatePasswordHash("player1", out var p1Hash, out var p1Salt);
        HashUtils.CreatePasswordHash("player2", out var p2Hash, out var p2Salt);
        HashUtils.CreatePasswordHash("unconfirmed", out var uHash, out var uSalt);
        HashUtils.CreatePasswordHash("no-wallet", out var nwHash, out var nwSalt);

        var admin = new User
        {
            Id = AdminId,
            Email = AdminEmail,
            FirstName = "Admin",
            LastName = "Adminsson",
            PasswordHash = adminHash,
            PasswordSalt = adminSalt,
            DateOfBirth = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Roles = new List<Role> { adminRole }
        };

        var player1 = new Player
        {
            Id = Player1Id,
            Email = Player1Email,
            FirstName = "Player",
            LastName = "One",
            PasswordHash = p1Hash,
            PasswordSalt = p1Salt,
            DateOfBirth = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Roles = new List<Role> { playerRole },
            Activated = true
        };

        var player2 = new Player
        {
            Id = Player2Id,
            Email = Player2Email,
            FirstName = "Player",
            LastName = "Two",
            PasswordHash = p2Hash,
            PasswordSalt = p2Salt,
            DateOfBirth = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Roles = new List<Role> { playerRole }
        };

        var unconfirmed = new Player
        {
            Id = UnconfirmedPlayerId,
            Email = UnconfirmedPlayerEmail,
            FirstName = "Unconfirmed",
            LastName = "Player",
            PasswordHash = uHash,
            PasswordSalt = uSalt,
            DateOfBirth = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Activated = false,
            Roles = new List<Role> { playerRole }
        };

        var noWalletUser = new User
        {
            Id = UserWithoutWalletId,
            Email = UserWithoutWalletEmail,
            FirstName = "No",
            LastName = "Wallet",
            PasswordHash = nwHash,
            PasswordSalt = nwSalt,
            DateOfBirth = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Roles = new List<Role>()
        };

        context.Users.AddRange(admin, player1, player2, unconfirmed, noWalletUser);

        // WALLETS
        context.Wallets.AddRange(
            new Wallet { Id = Player1WalletId, PlayerId = Player1Id, Player = player1, Balance = 100 },
            new Wallet { Id = Player2WalletId, PlayerId = Player2Id, Player = player2, Balance = 0 }
        );

        // GAME
        context.GameTemplates.Add(new GameTemplate
        {
            Id = GameTemplateId,
            Name = "Test Lotto",
            GameType = GameType.Lotto,
            PoolOfNumbers = 36,
            MaxWinningNumbers = 7,
            BasePrice = 10,
            MinNumbersPerTicket = 5,
            MaxNumbersPerTicket = 8
        });

        context.GameInstances.Add(new GameInstance
        {
            Id = GameInstanceId,
            GameTemplateId = GameTemplateId,
            Status = GameStatus.Active,
            DrawDate = DateTime.UtcNow.AddDays(7),
            CreatedById = AdminId
        });

        // TRANSACTIONS
        context.Transactions.AddRange(
            new Transaction
            {
                Id = PendingDepositTransactionId,
                WalletId = Player1WalletId,
                UserId = Player1Id,
                Amount = 200,
                Status = TransactionStatus.Requested,
                Type = TransactionType.Deposit,
                MobilePayTransactionNumber = "pending-txx"
            },
            new Transaction
            {
                Id = PendingDepositTransactionIdForError,
                WalletId = Player1WalletId,
                UserId = Player1Id,
                Amount = 50,
                Status = TransactionStatus.Requested,
                Type = TransactionType.Deposit,
                MobilePayTransactionNumber = "pending-tx"
            },
            new Transaction
            {
                Id = ExistingTransactionId,
                WalletId = Player1WalletId,
                UserId = Player1Id,
                Amount = 10,
                Status = TransactionStatus.Approved,
                Type = TransactionType.Deposit,
                MobilePayTransactionNumber = "1234"
            },
            new Transaction
            {
                Id = TransactionIdForWithdraw,
                WalletId = Player1WalletId,
                UserId = Player1Id,
                Amount = 3,
                Status = TransactionStatus.Requested,
                Type = TransactionType.Withdrawal,
                MobilePayTransactionNumber = "withdrawal-tx"
            }
        );

        // TICKET
        context.LotteryTickets.Add(new LotteryTicket
        {
            Id = TicketId,
            PlayerId = Player1Id,
            GameInstanceId = GameInstanceId,
            BoughtAt = DateTime.UtcNow,
            PickedNumbers =
            [
                new() { Number = 1 },
                new() { Number = 2 },
                new() { Number = 3 },
                new() { Number = 4 },
                new() { Number = 5 }
            ]
        });

        await context.SaveChangesAsync();
    }
}