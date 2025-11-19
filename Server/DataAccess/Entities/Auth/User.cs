namespace DataAccess.Entities.Auth;

public class User
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    
}