using System.Text;
using Api.Configuration;
using api.Services;
using Api.Services.Admin;
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
using NSwag;
using NSwag.Generation.Processors.Security;
using Utils;
using System;

namespace Api;

public static class Program
{
    private static void ConfigureServices(IServiceCollection services,  ConfigurationManager configuration)
    {
        DotNetEnv.Env.Load("../.env");

        
        // Get Options 
        var dbOptions = configuration.GetSection("DbOptions").Get<DbOptions>();
        var jwtOptions = configuration.GetSection("JwtOptions").Get<JwtOptions>();

        if (dbOptions == null || jwtOptions == null)
        {
            throw new Exception("Missing options configuration");
        }
        
        // Adds db context 
        services.AddDbContext<MyDbContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(dbOptions.ConnectionString);
        });

        /*
         *  Add service to the scope,  means that a new instance is created per request -> each request, new instance of service
         */
        services.AddScoped<IJwt, Jwt>();
        services.AddScoped<IMyAuthenticationService, MyAuthenticationService>();
        services.AddScoped<IUserManagementService, UserManagementService>();
        services.AddScoped<IEmailService, EmailService>();
        
        
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

        services.AddOpenApiDocument(configure =>
        {
            configure.Title = "Swagger UI";

            configure.AddSecurity("JWT", new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.ApiKey,
                Name = "Authorization",
                In = OpenApiSecurityApiKeyLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme.",
            });
            configure.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
        });
        
        // Add controllers to the api
        services.AddControllers();
                
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new DateTimeConverterCopenhagen());
            });

        services.AddAuthorization();


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

        
        var existingRoleNames = await dbContext.Roles
            .Select(r => r.Name)
            .ToListAsync();

        foreach (var roleEnum in Enum.GetValues<UserRole>())
        {

            if (!existingRoleNames.Contains(roleEnum))
            {
                dbContext.Roles.Add(new Role
                {
                    Name = roleEnum
                });
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
            throw new Exception("Missing environment configuration");
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
        }
        await dbContext.SaveChangesAsync();
    }

    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        ConfigureOptions(builder.Services, builder.Configuration);
        ConfigureServices(builder.Services, builder.Configuration);
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy
                    .WithOrigins("http://localhost:5173") // ← React App
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials(); // ← REQUIRED when using cookies
            });
        });

        var app = builder.Build();
        app.UseCors("AllowFrontend");
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