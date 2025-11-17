using System.ComponentModel.DataAnnotations;

namespace Api.Dto;

public class CreateUserHistoryDto
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public string Title { get; set; }

    public string Description { get; set; }
}
