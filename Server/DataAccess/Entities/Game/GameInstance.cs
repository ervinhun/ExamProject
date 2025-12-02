using System.ComponentModel.DataAnnotations;
using DataAccess.Entities.Auth;
using DataAccess.Enums;

namespace DataAccess.Entities.Game;

public class GameInstance
{
    public Guid Id { get; set; }
    
    public required Guid GameTemplateId { get; set; }
    public GameTemplate? GameTemplate { get; set; }
    
    [Range(1,6)] // 1-Sunday 6-Saturday
    public int? ExpirationDayOfWeek { get; set; }
    public TimeOnly? ExpirationTimeOfDay { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public bool IsAutoRepeatable { get; set; }
    public DateTime DrawDate { get; set; }
    public GameStatus Status { get; set; } 
    public required Guid CreatedById { get; set; }
    public ICollection<WinningNumber> WinningNumbers { get; set; } = new HashSet<WinningNumber>();
    [Range(1,52)]
    public int Week { get; set; }
    public bool IsExpired { get; set; }
    public bool IsDrawn { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
}