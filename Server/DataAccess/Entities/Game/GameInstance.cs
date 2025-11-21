using System.ComponentModel.DataAnnotations;
using DataAccess.Entities.Auth;

namespace DataAccess.Entities.Game;

public class GameInstance
{
    public Guid Id { get; set; }
    
    public required Guid GameTemplateId { get; set; }
    public GameTemplate GameTemplate { get; set; }
    public required Guid CreatedById { get; set; }
    public Admin Admin { get; set; }
    public ICollection<WinningNumber> WinningNumbers { get; set; } = new HashSet<WinningNumber>();
    [Range(1,52)]
    public int Week { get; set; }
    public string Name { get; set; } = string.Empty;
    public Boolean IsExpired { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
}