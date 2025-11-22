using DataAccess.Entities.Auth;

namespace Api.Dto.Auth.Response;

public class RoleDto
{
    public string Name { get; set; }

    public RoleDto(Role role)
    {
        Name = role.Name.ToString();
    }
}