using DataAccess;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

public class DatabaseFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container =
        new PostgreSqlBuilder()
            .WithDatabase("testdb")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

    public string ConnectionString => _container.GetConnectionString();

    public async ValueTask InitializeAsync()
    {
        await _container.StartAsync();

        Environment.SetEnvironmentVariable(
            "DB_CONNECTION_STRING",
            ConnectionString);

        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        await using var context = new MyDbContext(options);
        await context.Database.MigrateAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _container.StopAsync();
    }
}