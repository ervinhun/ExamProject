using DataAccess.Entities.Auth;
using DataAccess.Entities.Finance;
using DataAccess.Entities.Game;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;



namespace DataAccess;

public class MyDbContextFactory : IDesignTimeDbContextFactory<MyDbContext>
{
    public MyDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MyDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=LotteryApp;Username=postgres;Password=postgres");
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
                .HasOne(p => p.Wallet)
                .WithOne(w => w.Player)
                .HasForeignKey<Wallet>(w => w.PlayerId);
        });

        
        modelBuilder.Entity<Transaction>(transactionEntity =>
        {
            // Transaction -> Player : Many-to-One
            transactionEntity
                .HasOne(t => t.Player)
                .WithMany(p => p.Transactions)
                .HasForeignKey(t => t.PlayerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Transaction -> Wallet : Many-to-One
            transactionEntity
                .HasOne(t => t.Wallet)
                .WithMany(w => w.Transactions)
                .HasForeignKey(t => t.WalletId)
                .OnDelete(DeleteBehavior.Restrict);
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
