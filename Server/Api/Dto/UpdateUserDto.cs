using System.ComponentModel.DataAnnotations;

namespace Api.Dto;

public class UpdateUserDto
{
    public string FullName { get; set; }

    [EmailAddress]
    public string Email { get; set; }

    public string PhoneNo { get; set; }

    public Guid? RoleId { get; set; }
    
    public bool? IsActive { get; set; }
}
