using System;

namespace Api.Dto;

public class PlayerDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid WalletId { get; set; }
    public bool IsActive { get; set; }
}

public class CreatePlayerDto
{
    public Guid UserId { get; set; }
    public Guid WalletId { get; set; }
    public bool IsActive { get; set; }
}

public class UpdatePlayerDto
{
    public Guid? UserId { get; set; }
    public Guid? WalletId { get; set; }
    public bool? IsActive { get; set; }
}
