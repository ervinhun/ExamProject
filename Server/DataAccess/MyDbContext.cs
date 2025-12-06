using System;
using DataAccess.Entities.Auth;
using DataAccess.Entities.Finance;
using DataAccess.Entities.Game;
using DataAccess.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;
using DotNetEnv;
using Utils;


namespace DataAccess;

/// <summary>
/// Design-time factory used only by EF Core tools (migrations add/update).
/// At runtime the application builds the DbContext via dependency injection in Program.cs.
/// This factory resolves the connection string from environment variable CONNECTION_STRING so
/// migrations can target Neon or local Postgres without hardcoding secrets.
/// </summary>
public class MyDbContextFactory : IDesignTimeDbContextFactory<MyDbContext>
{
    public MyDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MyDbContext>();

        // Use EnvironmentHelper to automatically find and load .env file
        var connectionString = EnvironmentHelper.LoadAndGetConnectionString();

        optionsBuilder.UseNpgsql(connectionString);
        return new MyDbContext(optionsBuilder.Options);
    }
}

public class MyDbContext(DbContextOptions<MyDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Player> Players => Set<Player>();
    public DbSet<PlayerWhoApplied> WhoApplied => Set<PlayerWhoApplied>();
    public DbSet<Wallet> Wallets => Set<Wallet>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<TransactionHistory> TransactionHistories => Set<TransactionHistory>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Admin> Admins => Set<Admin>();
    public DbSet<GameInstance> GameInstances => Set<GameInstance>();
    public DbSet<GameTemplate> GameTemplates => Set<GameTemplate>();
    public DbSet<WinningNumber> WinningNumbers => Set<WinningNumber>();
    public DbSet<LotteryTicket> LotteryTickets => Set<LotteryTicket>();
    public DbSet<PickedNumber> PickedNumbers => Set<PickedNumber>();
    public DbSet<UserConfirmationEntity> UserConfirmations => Set<UserConfirmationEntity>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        /*
         * TPT - Table Per Type Strategy -> Each type/class has its own table in DataBase
         */
        modelBuilder.Entity<User>().ToTable("Users");
        modelBuilder.Entity<Player>().ToTable("Players");
        modelBuilder.Entity<Admin>().ToTable("Admins");

        modelBuilder.Entity<User>(userEntity =>
        {
            // Users -> Roles : Many-to-Many (Automatically creates RoleUser join table)
            userEntity
                .HasMany(u => u.Roles)
                .WithMany(r => r.Users)
                .UsingEntity(j => j.ToTable("RoleUser"));

            userEntity.HasIndex(u => u.Email).IsUnique();

            // Reset password token column size
            userEntity.Property(u => u.ResetPasswordToken)
                .HasMaxLength(512);
        });

        modelBuilder.Entity<Player>(playerEntity =>
        {
            // Player -> Wallet : One-to-One
            playerEntity
                .HasOne(p => p.Wallet);

            playerEntity
                .HasMany(p => p.LotteryTickets)
                .WithOne(t => t.Player)
                .HasForeignKey(t => t.PlayerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PlayerWhoApplied>(playerWhoAppliedEntity =>
        {
            playerWhoAppliedEntity
                .HasOne(e => e.Player)
                .WithOne(p => p.PlayerWhoApplied)
                .HasForeignKey<PlayerWhoApplied>(e => e.playerId)
                .OnDelete(DeleteBehavior.Restrict);
        });


        modelBuilder.Entity<Wallet>(walletEntity =>
        {
            walletEntity
                .HasMany(w => w.Transactions)
                .WithOne(t => t.Wallet)
                .HasForeignKey(t => t.WalletId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Transaction>(transactionEntity =>
        {
            // Transaction -> User : Foreign key only (no navigation to avoid circular dependency)
            transactionEntity
                .HasIndex(t => t.UserId);

        });

        modelBuilder.Entity<TransactionHistory>(transactionHistoryEntity =>
        {
            transactionHistoryEntity
                .HasOne(t => t.Transaction)
                .WithMany(t => t.TransactionHistory)
                .HasForeignKey(t => t.TransactionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<GameInstance>(gameInstanceEntity =>
        {
            // GameInstance -> GameTemplate : One-to-One Unidirectional
            gameInstanceEntity
                .HasIndex(g => new { g.GameTemplateId, g.Status })
                .IsUnique()
                .HasFilter($"\"Status\" = {(int)GameStatus.Active}");

            // GameInstance -> WinningNumbers : One-To-Many
            gameInstanceEntity
                .HasMany(g => g.WinningNumbers)
                .WithOne(w => w.GameInstance)
                .HasForeignKey(w => w.GameInstanceId)
                .OnDelete(DeleteBehavior.Restrict);

        });

        modelBuilder.Entity<LotteryTicket>(lotteryTicketEntity =>
        {
            lotteryTicketEntity
                .HasOne(lt => lt.Player)
                .WithMany(p => p.LotteryTickets)
                .HasForeignKey(lt => lt.PlayerId)
                .OnDelete(DeleteBehavior.Restrict);
            
            lotteryTicketEntity
                .HasMany(lt => lt.PickedNumbers)
                .WithOne(pn => pn.Ticket)
                .HasForeignKey(pn=>pn.TicketId)
                .OnDelete(DeleteBehavior.Restrict);
        });


        // =============================
        // USER CONFIRMATION ENTITY
        // =============================
        modelBuilder.Entity<UserConfirmationEntity>(entity =>
        {
            entity.ToTable("UserConfirmations");

            entity.HasKey(e => e.PlayerId);

            entity.Property(e => e.ConfirmationToken)
                .HasMaxLength(512);

            entity.Property(e => e.Result)
                .IsRequired();

            // FK to Player
            entity.HasOne<Player>()
                .WithMany()
                .HasForeignKey(e => e.PlayerId)
                .OnDelete(DeleteBehavior.Restrict);
        });


        base.OnModelCreating(modelBuilder);
    }
}
