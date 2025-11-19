using System;

namespace Api.Dto;

public class AdminDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
}

public class CreateAdminDto
{
    public Guid UserId { get; set; }
}

public class UpdateAdminDto
{
    public Guid? UserId { get; set; }
}
