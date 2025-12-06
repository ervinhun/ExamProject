using DotNetEnv;

namespace Utils;

public static class EnvironmentHelper
{
    /// <summary>
    /// Gets environment variable or throws exception if not found
    /// </summary>
    public static string GetRequired(string key)
    {
        var value = Environment.GetEnvironmentVariable(key);
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException(
                $"Required environment variable '{key}' is not set. " +
                $"Please check your .env file or environment configuration."
            );
        }
        return value;
    }

    /// <summary>
    /// Gets environment variable or returns default value
    /// </summary>
    public static string GetOrDefault(string key, string defaultValue)
    {
        var value = Environment.GetEnvironmentVariable(key);
        return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
    }

    /// <summary>
    /// Gets environment variable as integer or throws if not found/invalid
    /// </summary>
    public static int GetRequiredInt(string key)
    {
        var value = GetRequired(key);
        if (!int.TryParse(value, out var result))
        {
            throw new InvalidOperationException(
                $"Environment variable '{key}' has value '{value}' which is not a valid integer."
            );
        }
        return result;
    }

    /// <summary>
    /// Gets environment variable as integer or returns default
    /// </summary>
    public static int GetIntOrDefault(string key, int defaultValue)
    {
        var value = Environment.GetEnvironmentVariable(key);
        return int.TryParse(value, out var result) ? result : defaultValue;
    }

    /// <summary>
    /// Gets environment variable as boolean
    /// </summary>
    public static bool GetBool(string key, bool defaultValue = false)
    {
        var value = Environment.GetEnvironmentVariable(key);
        if (string.IsNullOrWhiteSpace(value)) return defaultValue;
        
        return value.ToLowerInvariant() switch
        {
            "true" or "1" or "yes" => true,
            "false" or "0" or "no" => false,
            _ => defaultValue
        };
    }

    /// <summary>
    /// Validates that all required environment variables are set
    /// </summary>
    public static void ValidateRequired(params string[] keys)
    {
        var missing = new List<string>();
        
        foreach (var key in keys)
        {
            var value = Environment.GetEnvironmentVariable(key);
            if (string.IsNullOrWhiteSpace(value))
            {
                missing.Add(key);
            }
        }

        if (missing.Any())
        {
            throw new InvalidOperationException(
                $"Missing required environment variables: {string.Join(", ", missing)}. " +
                "Please check your .env file or environment configuration."
            );
        }
    }

    /// <summary>
    /// Gets current environment name
    /// </summary>
    public static string GetEnvironment()
    {
        return GetOrDefault("ASPNETCORE_ENVIRONMENT", "Production");
    }

    /// <summary>
    /// Checks if running in development
    /// </summary>
    public static bool IsDevelopment()
    {
        return GetEnvironment().Equals("Development", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if running in production
    /// </summary>
    public static bool IsProduction()
    {
        return GetEnvironment().Equals("Production", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Special method for DbContext design-time factory to load and get connection string.
    /// Searches for .env file in current, parent, and grandparent directories.
    /// </summary>
    public static string LoadAndGetConnectionString()
    {
        const string connectionStringKey = "CONNECTION_STRING";
        
        // First check if already set in environment
        var existing = Environment.GetEnvironmentVariable(connectionStringKey);
        if (!string.IsNullOrWhiteSpace(existing))
        {
            return existing;
        }
        
        // Search for .env file upward from current directory
        var searchPaths = new[]
        {
            Directory.GetCurrentDirectory(),
            Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..")),
            Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..")),
            Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", ".."))
        };

        foreach (var basePath in searchPaths)
        {
            var envPath = Path.Combine(basePath, ".env");
            if (File.Exists(envPath))
            {
                try
                {
                    Env.Load(envPath);
                    var connectionString = Environment.GetEnvironmentVariable(connectionStringKey);
                    if (!string.IsNullOrWhiteSpace(connectionString))
                    {
                        Console.WriteLine($"[EnvironmentHelper] Loaded {connectionStringKey} from {envPath}");
                        return connectionString;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[EnvironmentHelper] Failed to load {envPath}: {ex.Message}");
                }
            }
        }
        
        // Fallback to local development connection string
        return Environment.GetEnvironmentVariable(connectionStringKey) 
               ?? "Host=localhost;Port=5432;Database=lotteryapp_dev;Username=postgres;Password=postgres";
    }
}
