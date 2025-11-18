using System.ComponentModel.DataAnnotations;

namespace Api.Dto;

public class CreateUserBalanceDto
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public int Balance { get; set; }
}
