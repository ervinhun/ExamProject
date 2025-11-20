using System.Text;
using Api.Configuration;
using api.Services;
using api.Services.Auth;
using Api.Services.Auth;
using DataAccess;
using DataAccess.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Api;

public static class Program
{
    private static void ConfigureServices(IServiceCollection services,  ConfigurationManager configuration)
    {
        // Adds db context 
        var dbOptions = configuration.GetSection("DbOptions").Get<DbOptions>();
        services.AddDbContext<MyDbContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(dbOptions.ConnectionString);
        });

        /*
         *  Add service to the scope,  means that a new instance is created per request -> each request, new instance of service
         */
        services.AddScoped<IJwt, Jwt>();
        services.AddScoped<IMyAuthenticationService, MyAuthenticationService>();
        
        
        var jwtOptions = configuration.GetSection("JwtOptions").Get<JwtOptions>();
        services
            .AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret))
                };
            });
        
        
        // Add controllers to the api
        services.AddControllers();

        // Needed for Swagger to work 
        services.AddOpenApiDocument();
        
        
        /*
         *  Add service as a singleton, means that one instance of a service is shared across the app -> longer lifetime (configuration, logging, cashing)  
         */
        // services.AddSingleton();
        
    }

    private static void ConfigureOptions(IServiceCollection services,  ConfigurationManager configuration)
    {
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection("JwtOptions"))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services.AddOptions<DbOptions>()
            .Bind(configuration.GetSection("DbOptions"))
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        ConfigureOptions(builder.Services, builder.Configuration);
        ConfigureServices(builder.Services, builder.Configuration);
        
        var app = builder.Build();

        app.UseAuthentication();
        app.UseAuthorization();
        
        // Maps added controllers to the API 
        app.MapControllers();

        // Needed for Swagger to work   
        app.UseOpenApi();
        app.UseSwaggerUi();

        app.Run();
    }
}