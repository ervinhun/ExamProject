using DataAccess;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace tests;

public class Startup
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder().Build();

    public string ConnectionString => _postgreSqlContainer.GetConnectionString();

    public async ValueTask InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
        Environment.SetEnvironmentVariable("DB_CONNECTION_STRING", ConnectionString);
        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        await using var context = new MyDbContext(options);
        await context.Database.MigrateAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _postgreSqlContainer.StopAsync();
    }
}