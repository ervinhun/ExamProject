using Api.Options;
using api.Services;
using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Api;

public static class Program
{
    private static void ConfigureServices(IServiceCollection services)
    {
        
        // Add controllers to the api
        services.AddControllers();

        // Needed for Swagger to work 
        services.AddOpenApiDocument();

        /*
         *  Add service to the scope,  means that a new instance is created per request -> each request, new instance of service
         */
        services.AddScoped<IAuthService, AuthService>();

        /*
         *  Add service as a singleton, means that one instance of a service is shared across the app -> longer lifetime (configuration, logging, cashing)  
         */
        // services.AddSingleton();
        
        // Adds db context 
         services.AddDbContext<MyDbContext>((serviceProvider, options) =>
         {
             options.UseNpgsql(serviceProvider.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection"));
         });
    }

    private static void ConfigureOptions(IServiceCollection services,  IConfiguration configuration)
    {
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection("JWT"))
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        ConfigureServices(builder.Services);
        ConfigureOptions(builder.Services, builder.Configuration);
        
        var app = builder.Build();

        // Maps added controllers to the API 
        app.MapControllers();

        // Needed for Swagger to work   
        app.UseOpenApi();
        app.UseSwaggerUi();
        Console.WriteLine(Environment.GetEnvironmentVariable("JWT_SECRET"));
        Console.WriteLine("Heeello");

        app.Run();
        Console.WriteLine(Environment.GetEnvironmentVariable("JWT_SECRET"));

    }
}




