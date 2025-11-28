using System;
using DataAccess.Entities.Auth;
using DataAccess.Entities.Finance;
using DataAccess.Entities.Game;
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
    private const string ConnectionStringEnv = "CONNECTION_STRING";
    
    public MyDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MyDbContext>();

        var connectionString = EnvironmentHelper.GetRequired(ConnectionStringEnv);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            // Fallback for local design-time usage if env var not set.
            connectionString = "Host=localhost;Port=5432;Database=LotteryApp;Username=postgres;Password=postgres;SslMode=Disable";
            Console.WriteLine($"[MyDbContextFactory] WARNING: {ConnectionStringEnv} not set. Falling back to local connection string.");
        }

        optionsBuilder.UseNpgsql(connectionString);
        return new MyDbContext(optionsBuilder.Options);
    }
}

public class MyDbContext(DbContextOptions<MyDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Player> Players => Set<Player>();
    public DbSet<Wallet> Wallets => Set<Wallet>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Admin> Admins => Set<Admin>();
    public DbSet<GameInstance> GameInstances => Set<GameInstance>();
    public DbSet<GameTemplate> GameTemplates => Set<GameTemplate>();
    public DbSet<WinningNumber> WinningNumbers => Set<WinningNumber>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        /*
         * Define global schema
         */
        modelBuilder.HasDefaultSchema("LotteryApp");
        
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
                .UsingEntity(j=>j.ToTable("RoleUser"))
                .HasIndex(u=>u.Email).IsUnique();
        });
        
        modelBuilder.Entity<Player>(playerEntity =>
        {
            // Player -> Wallet : One-to-One
            playerEntity
                .HasOne(p => p.Wallet);
        });

        
        modelBuilder.Entity<Transaction>(transactionEntity =>
        {
            // Transaction -> Wallet : Many-to-One
            transactionEntity
                .HasOne(t => t.Wallet)
                .WithMany(w => w.Transactions)
                .HasForeignKey(t => t.WalletId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Transaction -> Player : Foreign key only (no navigation to avoid circular dependency)
            transactionEntity
                .HasIndex(t => t.PlayerId);
        });

        modelBuilder.Entity<GameInstance>(gameInstanceEntity =>
        {
            // GameInstance -> GameTemplate : One-to-One Unidirectional
            gameInstanceEntity
                .HasOne(g => g.GameTemplate)
                .WithOne()
                .HasForeignKey<GameInstance>(g => g.GameTemplateId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            // GameInstance -> WinningNumbers : One-To-Many
            gameInstanceEntity
                .HasMany(g=>g.WinningNumbers)
                .WithOne(w=>w.GameInstance)
                .HasForeignKey(w=>w.GameInstanceId)
                .OnDelete(DeleteBehavior.Restrict);
        });   
        
        
        
        base.OnModelCreating(modelBuilder);
    }
}
