using System.ComponentModel.DataAnnotations;

namespace Api.Dto;

public class CreateRoleDto
{
    [Required]
    public string Name { get; set; }
    public string Description { get; set; }
}
