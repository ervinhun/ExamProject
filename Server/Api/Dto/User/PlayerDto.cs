namespace Api.Dto.test;

public class PlayerDto : UserDto
{
    public Guid PlayerId { get; set; }
    public Guid WalletId { get; set; }
    public bool IsActive { get; set; }
}

public class CreatePlayerDto : CreateUserDto
{
}

public class UpdatePlayerDetailsDto : UpdateUserDetailsDto
{
}
