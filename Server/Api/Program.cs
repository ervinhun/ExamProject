using System.Text;
using api.Services;
using api.Services.Auth;
using Api.Services.Auth;
using Api.Services.Email;
using DataAccess;
using DataAccess.Entities.Auth;
using DataAccess.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation.Processors.Security;
using Utils;
using Api.Configuration;
using Api.Services.Admin;
using Api.Services.Game;
using Api.Services.Management;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Npgsql;

namespace Api;

public static class Program
{
    private static void LoadEnvironmentVariables()
    {
        // Load .env file only in Development
        if (EnvironmentHelper.IsDevelopment())
        {
            var envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", ".env");
            if (File.Exists(envPath))
            {
                Env.Load(envPath);
            }
            else
            {
                Console.WriteLine("Warning: .env file not found. Using system environment variables.");
            }
        }
        
        // Validate required environment variables
        EnvironmentHelper.ValidateRequired(
            "CONNECTION_STRING",
            "JWT_SECRET",
            "JWT_ISSUER",
            "JWT_AUDIENCE",
            "JWT_EXPIRATION_MINUTES",
            "JWT_REFRESH_TOKEN_DAYS",
            "SUPER_EMAIL",
            "SUPER_PASSWORD",
            "CLIENT_HOST"
        );
    }
    
    private static AppSettings BuildAppSettings()
    {
        return new AppSettings
        {
            Database = new DatabaseSettings
            {
                ConnectionString = EnvironmentHelper.GetRequired("CONNECTION_STRING")
            },
            Jwt = new JwtSettings
            {
                Secret = EnvironmentHelper.GetRequired("JWT_SECRET"),
                Issuer = EnvironmentHelper.GetRequired("JWT_ISSUER"),
                Audience = EnvironmentHelper.GetRequired("JWT_AUDIENCE"),
                ExpirationMinutes = EnvironmentHelper.GetIntOrDefault("JWT_EXPIRATION_MINUTES", 60),
                RefreshTokenDays = EnvironmentHelper.GetIntOrDefault("JWT_REFRESH_TOKEN_DAYS", 7)
            },
            Email = new EmailSettings
            {
                SmtpHost = EnvironmentHelper.GetOrDefault("SMTP_HOST", ""),
                SmtpPort = EnvironmentHelper.GetIntOrDefault("SMTP_PORT", 587),
                Username = EnvironmentHelper.GetOrDefault("SMTP_USERNAME", ""),
                Password = EnvironmentHelper.GetOrDefault("SMTP_PASSWORD", ""),
                FromAddress = EnvironmentHelper.GetOrDefault("EMAIL_FROM", "noreply@lotteryapp.com")
            },
            Cors = new CorsSettings
            {
                AllowedOrigins = EnvironmentHelper.GetOrDefault("CLIENT_HOST", "http://localhost:5173")
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
            },
            Super = new SuperSettings
            {
                Email = EnvironmentHelper.GetRequired("SUPER_EMAIL"),
                Password = EnvironmentHelper.GetRequired("SUPER_PASSWORD")
            }
        };
    }
    
    public static void ConfigureServices(IServiceCollection services, AppSettings appSettings)
    {
        // Register AppSettings for dependency injection
        services.AddSingleton(appSettings);
        services.AddSingleton(appSettings.Database);
        services.AddSingleton(appSettings.Jwt);
        services.AddSingleton(appSettings.Email);
        services.AddSingleton(appSettings.Cors);
        
        // Add DbContext
        services.AddDbContext<MyDbContext>(options =>
        {
            options.UseNpgsql(appSettings.Database.ConnectionString);
        });
        
        // Add scoped services
        services.AddScoped<IJwt, Jwt>();
        services.AddScoped<IMyAuthenticationService, MyAuthenticationService>();
        services.AddScoped<IUserManagementService, UserManagementService>();
        services.AddScoped<IGameManagementService, GameManagementService>();
        services.AddScoped<IWalletTransactionsService, WalletTransactionsService>();
        services.AddScoped<IEmailService, EmailService>();
        
        // Configure JWT Authentication
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
                    ValidIssuer = appSettings.Jwt.Issuer,
                    ValidAudience = appSettings.Jwt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.Jwt.Secret))
                };
                
                // Read JWT from cookie or Authorization header
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Cookies["accessToken"];
                        if (token != null)
                        {
                            context.Token = token;
                        } 
                        
                        return Task.CompletedTask;
                    }
                };
            });
        
        // Configure CORS
        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy
                    .WithOrigins(appSettings.Cors.AllowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
        
        // Add controllers to the api
        services.AddControllers();
                
        services.AddControllers()
            .AddJsonOptions(configure: options =>
            {
                options.JsonSerializerOptions.Converters.Add(item: new DateTimeConverterCopenhagen());
            });

        services.AddAuthorization();
        
        
        // Add Swagger in Development
        if (EnvironmentHelper.IsDevelopment())
        {
            services.AddOpenApiDocument(configure =>
            {
                configure.Title = "Swagger UI";

                configure.AddSecurity(name: "JWT", swaggerSecurityScheme: new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme.",
                });
                configure.OperationProcessors.Add(item: new AspNetCoreOperationSecurityScopeProcessor(name: "JWT"));
            });
        }

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
    
    private static async Task EnsureSuperAdminIsCreatedAsync(IServiceProvider serviceProvider, SuperSettings superSettings)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();

        var user = await dbContext.Users.AnyAsync(x => x.Email == superSettings.Email);
        var superAdminRole = await dbContext.Roles.FirstOrDefaultAsync(r => r.Name == UserRole.SuperAdmin);
        
        if (superAdminRole == null)
        {
            throw new Exception("SuperAdmin role not found in database");
        }
        
        if (!user)
        {
            HashUtils.CreatePasswordHash(superSettings.Password, out var passwordHash, out var passwordSalt);

            var superAdmin = new Admin
            {
                Email = superSettings.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Activated = true
            };
            
            superAdmin.Roles.Add(superAdminRole);
            superAdminRole.Users.Add(superAdmin);

            dbContext.Users.Add(superAdmin);
            await dbContext.SaveChangesAsync();
            
            Console.WriteLine($"SuperAdmin created: {superSettings.Email}");
        }
    }

    public static async Task Main(string[] args)
    {
        try
        {
            // Load and validate environment variables
            LoadEnvironmentVariables();
            
            // Build configuration from environment
            var appSettings = BuildAppSettings();
            
            var builder = WebApplication.CreateBuilder(args);
            
            // Configure services with validated settings
            ConfigureServices(builder.Services, appSettings);
            
            // Add controllers
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new DateTimeConverterCopenhagen());
                });
            
            builder.Services.AddAuthorization();

            var app = builder.Build();
            
            // Development-only middleware
            if (app.Environment.IsDevelopment())
            {
                Console.WriteLine("✓ Running in Development mode");
                app.UseOpenApi();
                app.UseSwaggerUi();
            }
            else
            {
                Console.WriteLine("✓ Running in Production mode");
            }
            
            // Configure middleware pipeline
            app.UseCors("AllowFrontend");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            
            // Apply pending migrations automatically
            /*using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();
                var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
                var migrations = pendingMigrations as string[] ?? pendingMigrations.ToArray();
                if (migrations.Any())
                {
                    Console.WriteLine($"[DB] Applying {migrations.Count()} pending migration(s)...");
                    await dbContext.Database.MigrateAsync();
                    Console.WriteLine("[DB] ✓ Migrations applied successfully");
                }
                else
                {
                    Console.WriteLine("[DB] ✓ Database schema is up to date");
                }
            }*/
            
            // Initialize database
            await EnsureRolesAreCreatedAsync(app.Services);
            await EnsureSuperAdminIsCreatedAsync(app.Services, appSettings.Super);
            
            Console.WriteLine($"  Application started successfully");
            Console.WriteLine($"  Environment: {EnvironmentHelper.GetEnvironment()}");
            Console.WriteLine($"  Database: {appSettings.Database.ConnectionString.Split(';')[0]}");
            Console.WriteLine($"  CORS Origins: {string.Join(", ", appSettings.Cors.AllowedOrigins)}");
            
            await app.RunAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Application failed to start: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            throw;
        }
    }
}