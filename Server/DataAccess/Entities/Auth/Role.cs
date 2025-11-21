using System.ComponentModel.DataAnnotations;
using DataAccess.Enums;

namespace DataAccess.Entities.Auth;

public class Role
{
    public Guid Id { get; set; }
    [MaxLength(255)]
    public UserRole Name { get; set; }
    [MaxLength(255)]
    public ICollection<User> Users { get; set; } = new List<User>();
}