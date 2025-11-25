using DataAccess.Entities.Auth;
using DataAccess.Enums;

namespace Api.Dto.Auth.Response;

public record UserResponseDto
{
    public Guid Id { get; set; } = Guid.Empty;
    public string FullName { get; set; }
    public string Email { get; set; }
    public IEnumerable<UserRole> Roles { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    
}