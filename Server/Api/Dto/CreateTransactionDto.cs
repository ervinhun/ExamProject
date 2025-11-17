using System.ComponentModel.DataAnnotations;

namespace Api.Dto;

public class CreateTransactionDto
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public int Amount { get; set; }
}
