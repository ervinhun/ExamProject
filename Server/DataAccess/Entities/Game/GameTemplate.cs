using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities.Game;

public class GameTemplate
{
    public Guid Id { get; set; }
    /*
     * 0 - Sunday
     * 6 - Saturday
     */
    [Range(0,6)]
    public int ExpirationDayOfWeek {get; set;}
   
    public TimeOnly ExpirationTime { get; set; }
    public int NumberRangeMin { get; set; }
    public int NumberRangeMax { get; set; }
    public int MaxNumbersPerBoard { get; set; }
    public int MinNumbersPerBoard { get; set; }
    
    [Range(0, double.MaxValue)]
    public double BasePrice { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
    
}