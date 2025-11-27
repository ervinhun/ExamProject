
using DataAccess.Entities.Auth;
using DataAccess.Enums;

namespace Api.Dto.Auth.Response;

public record UserResponseDto
{
    public Guid Id { get; set; } = Guid.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; }  = string.Empty;
    public List<UserRole> Roles { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    
}