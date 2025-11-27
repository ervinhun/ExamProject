using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities.Auth;

public class User
{
    public Guid Id { get; set; }
    
    [MaxLength(255)]
    public string FirstName { get; set; } = string.Empty;
    
    [MaxLength(255)]
    public string LastName { get; set; } = string.Empty;
    
    [Phone]
    [MaxLength(255)]
    public string PhoneNumber { get; set; } = string.Empty;
    
    [EmailAddress]
    [MaxLength(255)]
    [Required]
    public string Email { get; set; } =  string.Empty;
    public byte[] PasswordHash { get; set; } =  null!;
    public byte[] PasswordSalt { get; set; } = null!;

    public ICollection<Role> Roles { get; set; } = new List<Role>();
    
    [MaxLength(512)]
    public string RefreshTokenHash { get; set; } = string.Empty;
    public DateTime RefreshTokenExpires { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}