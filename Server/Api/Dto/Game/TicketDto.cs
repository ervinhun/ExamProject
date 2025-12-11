using System.ComponentModel.DataAnnotations;

namespace Api.Dto.Game;

public class TicketDto
{
    public class CreateTicketRequestDto
    {
        [Required]
        public Guid GameInstanceId { get; set; }
        [Required]
        public Guid GameTemplateId { get; set; }
        [Required]
        public int[] SelectedNumbers { get; set; }
        public int Repeat { get; set; }
    }
    
    public class TicketResponseDto : CreateTicketRequestDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set;}
        public DateTime UpdatedAt { get; set;}
        public bool IsWinning { get; set;}
        public double TicketPrice { get; set;}
        public bool IsPaid { get; set;}
    }
}