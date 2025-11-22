using DataAccess.Entities.Auth;

namespace Api.Dto.Auth.Response;

public record UserRepsonseDto
{
    public string name { get; set; }
    public string email { get; set; }
    public List<RoleDto> Roles { get; set; }

    public UserRepsonseDto(User user)
    {
        this.name = user.FullName;
        this.email = user.Email;
        Roles = user.Roles.Select(r => new RoleDto(r)).ToList();
    }
}