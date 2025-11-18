namespace Api.Dto;

public class UpdateRoleDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool? IsDeleted { get; set; }
}
