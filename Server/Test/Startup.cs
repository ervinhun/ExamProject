using Api;
using Api.Configuration;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;

namespace Test;

public class Startup
{
    public static void ConfigureServices(IServiceCollection services, AppSettings appSettings)
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
        Program.ConfigureServices(services, appSettings);
        services.RemoveAll(typeof(MyDbContext));
        services.AddScoped<MyDbContext>(factory =>
        {
            var postgreSqlContainer = new PostgreSqlBuilder().Build();
            postgreSqlContainer.StartAsync().GetAwaiter().GetResult();
            var connectionString = postgreSqlContainer.GetConnectionString();
            var options = new DbContextOptionsBuilder<MyDbContext>()
                .UseNpgsql(connectionString)
                .Options;

            var ctx = new MyDbContext(options);
            ctx.Database.EnsureCreated();
            return ctx;
        });
        services.RemoveAll<TimeProvider>();
        //var fakeTime = new FakeTimeProvider();
        //services.AddSingleton<TimeProvider>(fakeTime);
    }
}