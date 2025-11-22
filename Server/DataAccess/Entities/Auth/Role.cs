using System.ComponentModel.DataAnnotations;
using DataAccess.Enums;

namespace DataAccess.Entities.Auth;

public class Role
{
    public Guid Id { get; set; }
    public UserRole Name { get; set; }
    public ICollection<User> Users { get; set; } = new List<User>();
}