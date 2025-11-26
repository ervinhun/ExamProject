using System.Text.Json;
using System.Text.Json.Serialization;

namespace Utils;

public class DateTimeConverterCopenhagen : JsonConverter<DateTime>
{
    private static readonly TimeZoneInfo TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Europe/Copenhagen");
    
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var local = reader.GetDateTime();
        return TimeZoneInfo.ConvertTimeFromUtc(local, TimeZoneInfo);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        var local = TimeZoneInfo.ConvertTimeFromUtc(value.ToUniversalTime(), TimeZoneInfo);
        writer.WriteStringValue(local);
    }
}