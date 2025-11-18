using System.ComponentModel.DataAnnotations;

namespace Api.Dto;

public class CreateBoardDto
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public string Numbers { get; set; }

    [Required]
    public int FieldCount { get; set; }
}
