using System.ComponentModel.DataAnnotations;
using DataAccess.Enums;

namespace DataAccess.Entities.Game;

public class GameTemplate
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public GameType GameType { get; set; }
    
    [Range(0, 500)]
    public int PoolOfNumbers { get; set; }
    
    [Range(0, int.MaxValue)]
    public int MaxWinningNumbers { get; set; }
    
    [Range(0, int.MaxValue)]
    public int MaxNumbersPerTicket { get; set; }
    
    [Range(0, int.MaxValue)]
    public int MinNumbersPerTicket { get; set; }
    
    [Range(0, double.MaxValue)]
    public double BasePrice { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
}