using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities.Game;

public class GameTemplate
{
    public Guid Id { get; set; }
    /*  0 - Sunday
        6 - Saturday */
    [Range(0,6)]
    public uint ExpirationDayOfWeek {get; set;}
   
    public TimeOnly ExpirationTime { get; set; }
    public uint NumberRangeMin { get; set; }
    public uint NumberRangeMax { get; set; }
    public uint MaxNumbersPerBoard { get; set; }
    public uint MinNumbersPerBoard { get; set; }
    
    [Range(0, double.MaxValue)]
    public double BasePrice { get; set; }
    public uint Multiplier { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
    
}