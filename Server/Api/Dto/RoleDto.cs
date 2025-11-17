namespace Api.Dto;

public class RoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool? IsDeleted { get; set; }
}
