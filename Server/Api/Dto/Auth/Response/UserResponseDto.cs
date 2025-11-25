using DataAccess.Entities.Auth;
<<<<<<< Updated upstream
<<<<<<< Updated upstream
using DataAccess.Enums;
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes

namespace Api.Dto.Auth.Response;

public record UserResponseDto
{
    public Guid Id { get; set; } = Guid.Empty;
    public string FullName { get; set; }
    public string Email { get; set; }
<<<<<<< Updated upstream
<<<<<<< Updated upstream
    public IEnumerable<UserRole> Roles { get; set; }
=======
    public List<RoleDto> Roles { get; set; }
>>>>>>> Stashed changes
=======
    public List<RoleDto> Roles { get; set; }
>>>>>>> Stashed changes
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    
}