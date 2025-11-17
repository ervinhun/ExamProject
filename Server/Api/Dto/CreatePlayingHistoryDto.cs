using System.ComponentModel.DataAnnotations;

namespace Api.Dto;

public class CreatePlayingHistoryDto
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid TicketId { get; set; }

    [Required]
    public string Title { get; set; }

    public string Description { get; set; }
}
