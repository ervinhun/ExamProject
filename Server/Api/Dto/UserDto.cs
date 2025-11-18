namespace Api.Dto;

public class UserDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNo { get; set; }
    public Guid? RoleId { get; set; }
    public bool? IsActive { get; set; }
    public DateOnly? ActiveStatusExpiryDate { get; set; }
    public DateTime CreatedAt { get; set; }
}
