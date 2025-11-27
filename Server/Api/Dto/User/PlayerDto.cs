using Api.Dto.User;

namespace Api.Dto.test;

public class PlayerDto : UserDto
{
    public bool IsActive { get; set; }
}

public class CreatePlayerDto : CreateUserDto
{
}

public class UpdatePlayerDetailsDto : UpdateUserDetailsDto
{
}
