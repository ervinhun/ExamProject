namespace DataAccess.Entities.Auth;

public class UserConfirmationEntity
{
    public Guid PlayerId { get; set; }
    public String? ConfirmationToken { get; set; } = null;
    public Boolean isActive { get; set; }
    public String Result { get; set; }
    public String? Role { get; set; }
}