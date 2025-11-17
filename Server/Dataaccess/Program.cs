var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

DotNetEnv.Env.Load($".env.{env}");
