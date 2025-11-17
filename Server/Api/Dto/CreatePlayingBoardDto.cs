using System.ComponentModel.DataAnnotations;

namespace Api.Dto;

public class CreatePlayingBoardDto
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid BoardId { get; set; }

    [Required]
    public Guid GameId { get; set; }

    [Required]
    public string Numbers { get; set; }

    [Required]
    public int FieldCount { get; set; }

    [Required]
    public int Price { get; set; }

    public bool? IsRepeat { get; set; }

    public int? RepeatCountRemaining { get; set; }
}
