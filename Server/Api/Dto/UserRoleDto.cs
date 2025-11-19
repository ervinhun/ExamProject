using System;

namespace Api.Dto;

public class UserRoleDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
}

public class CreateUserRoleDto
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public Guid? CreatedBy { get; set; }
}

public class UpdateUserRoleDto
{
    public Guid? UserId { get; set; }
    public Guid? RoleId { get; set; }
    public Guid? CreatedBy { get; set; }
}
