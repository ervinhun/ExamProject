using System.ComponentModel.DataAnnotations;

namespace Api.Dto;

public class CreateUserDto
{
    [Required]
    public string FullName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }

    public string PhoneNo { get; set; }

    public Guid? RoleId { get; set; }
}
