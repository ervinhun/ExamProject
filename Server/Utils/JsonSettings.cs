using System.Text.Json;

namespace Utils;

public static class JsonSettings
{
    public static JsonSerializerOptions DefaultOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

}