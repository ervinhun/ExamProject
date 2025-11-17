using Dataaccess;
using Microsoft.EntityFrameworkCore;

namespace Api, DataAccess;

public static class Program
{
    public static void ConfigureServices(IServiceCollection services)
    {
        // Add controllers to the api
        services.AddControllers();

        // Needed for Swagger to work 
        services.AddOpenApiDocument();

        // Adds service to the scope 
        // services.AddScoped<>();

        // Adds db context 
         services.AddDbContext<MyDbContext>((services, options) =>
         {
             options.UseNpgsql(services.GetRequiredService<IConfiguration>()
                 .GetValue<string>("Db"));
         });

    }

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        ConfigureServices(builder.Services);
        
        var app = builder.Build();

        // Maps added controllers to the API 
        app.MapControllers();

        // Needed for Swagger to work   
        app.UseOpenApi();
        app.UseSwaggerUi();

        app.Run();
    }
}




