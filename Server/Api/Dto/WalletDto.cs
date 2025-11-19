using System;

namespace Api.Dto;

public class WalletDto
{
    public Guid Id { get; set; }
    public decimal Balance { get; set; }
    public Guid UserId { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool? IsDeleted { get; set; }
}

public class CreateWalletDto
{
    public decimal Balance { get; set; }
    public Guid UserId { get; set; }
}

public class UpdateWalletDto
{
    public decimal? Balance { get; set; }
    public bool? IsDeleted { get; set; }
}
