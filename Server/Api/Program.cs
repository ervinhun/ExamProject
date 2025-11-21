using System.Text;
using Api.Configuration;
using api.Services;
using api.Services.Auth;
using Api.Services.Auth;
using Api.Services.Email;
using DataAccess;
using DataAccess.Configuration;
using DataAccess.Entities.Auth;
using DataAccess.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Utils;

namespace Api;

public static class Program
{
    private static void ConfigureServices(IServiceCollection services,  ConfigurationManager configuration)
    {
        DotNetEnv.Env.Load("../.env");
        
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
        services.AddScoped<IEmailService, EmailService>();
        
        
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

    private static async Task EnsureRolesAreCreatedAsync(IServiceProvider serviceProvider)
    {
        using var  scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();

        foreach (var roleName in Enum.GetValues<UserRole>())
        {
            var role = await dbContext.Roles.AnyAsync(r => r.Name.Equals(roleName));
            if (!role)
            {
                var newRole = new Role()
                {
                    Name = roleName,
                };
                dbContext.Roles.Add(newRole);
            }
        }
        
        await dbContext.SaveChangesAsync();
    }
    
    private static async Task EnsureSuperAdminIsCreatedAsync(IServiceProvider serviceProvider)
    {
        using var  scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();
        
        var email = Environment.GetEnvironmentVariable("SUPER_ADMIN_EMAIL");
        var password = Environment.GetEnvironmentVariable("SUPER_ADMIN_PASSWORD");

        if (email == null || password == null)
        {
            throw new Exception("Missing configuration");
        }
    
        var user = await dbContext.Users.AnyAsync(x => x.Email == email);
        var superAdminRole = await dbContext.Roles.FirstOrDefaultAsync(r=>r.Name == UserRole.SuperAdmin);
        
        if (!user && superAdminRole != null)
        {
            HashUtils.CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

            var superAdmin = new Admin()
            {
                Email = email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };
            
            
            superAdmin.Roles.Add(superAdminRole);
            superAdminRole.Users.Add(superAdmin);

            dbContext.Users.Add(superAdmin);
            dbContext.Roles.Add(superAdminRole);
            
            await dbContext.SaveChangesAsync();
        }
    }

    public static async Task Main(string[] args)
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

        await EnsureRolesAreCreatedAsync(app.Services);
        await EnsureSuperAdminIsCreatedAsync(app.Services);
        
        await app.RunAsync();
        
    }
}