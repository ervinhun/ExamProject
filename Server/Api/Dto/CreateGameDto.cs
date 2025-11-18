using System.ComponentModel.DataAnnotations;

namespace Api.Dto;

public class CreateGameDto
{
    [Required]
    public DateOnly StartDate { get; set; }

    [Required]
    public DateOnly EndDate { get; set; }
}
