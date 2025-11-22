namespace Api.Dto.User;

public class UserDto
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PhoneNo { get; set; }
    public DateOnly? ActiveStatusExpiryDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool? IsDeleted { get; set; }
}

public class CreateUserDto
{
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNo { get; set; } = null!;
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
