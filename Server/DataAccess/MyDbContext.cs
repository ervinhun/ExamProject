using DataAccess.Entities.Auth;
using DataAccess.Entities.Finance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

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
        
        
        /*
         * ONE-TO-ONE RELATIONSHIP
         */
        modelBuilder.Entity<Player>()
            .HasOne(p => p.Wallet)
            .WithOne(w => w.Player)
            .HasForeignKey<Wallet>(w => w.PlayerId);
        
        
        base.OnModelCreating(modelBuilder);
    }
}
