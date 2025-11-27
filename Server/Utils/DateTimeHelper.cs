namespace Utils;

public static class DateTimeHelper
{
    private static readonly TimeZoneInfo CopenhagenTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Copenhagen");
    
    /// <summary>
    /// Gets the current time in Copenhagen timezone (UTC+1/UTC+2 during daylight saving)
    /// </summary>
    public static DateTime NowCopenhagen => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, CopenhagenTimeZone);
    
    /// <summary>
    /// Converts a UTC DateTime to Copenhagen timezone
    /// </summary>
    public static DateTime ToCopenhagen(DateTime utcDateTime)
    {
        return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, CopenhagenTimeZone);
    }
    
    /// <summary>
    /// Converts a Copenhagen DateTime to UTC
    /// </summary>
    public static DateTime ToUtc(DateTime copenhagenDateTime)
    {
        return TimeZoneInfo.ConvertTimeToUtc(copenhagenDateTime, CopenhagenTimeZone);
    }
}
