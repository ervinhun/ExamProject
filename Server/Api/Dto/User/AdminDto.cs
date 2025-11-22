using Api.Dto.User;

namespace Api.Dto.test;

public class AdminDto : UserDto
{
    public Guid AdminId { get; set; }
}

public class CreateAdminDto: CreateUserDto
{
}

public class UpdateAdminDetailsDto : UpdateUserDetailsDto
{
}
