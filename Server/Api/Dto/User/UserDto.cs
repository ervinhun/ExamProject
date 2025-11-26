using DataAccess.Enums;

namespace Api.Dto.User;

public class UserDto
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public ICollection<UserRole> Roles {get; set;} = new List<UserRole>();
    public DateOnly? ActiveStatusExpiryDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool? IsDeleted { get; set; }
}

public class CreateUserDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
}

public class UpdateUserDetailsDto
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNo { get; set; }
}

public class UpdatePasswordDto
{
    public string? Password { get; set; }
}
