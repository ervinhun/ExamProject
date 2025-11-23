using DataAccess.Entities.Auth;
using DataAccess.Enums;

namespace Api.Dto.Auth;

public class JwtClaimsDto
{
    public required string UserId { get; set; }
    public required string Email { get; set; } 
    public ICollection<Role> Roles { get; set; } = new  List<Role>();
}