using System.ComponentModel.DataAnnotations;

namespace Api.Dto.Game;

public class TickettDto
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

public class PurchaseTicketDto
{
    public Guid GameInstanceId { get; set; }
    public Guid PlayerId { get; set; }
    public Guid WalletId { get; set; }
    public int[] PickedNumbers { get; set; } = [];
    public double FullPrice { get; set; }
}

public class TicketDto : PurchaseTicketDto
{
    public Guid Id { get; set; }
    public DateTime BoughtAt { get; set; }
    public bool IsWinning { get; set; }
    public bool IsPaid { get; set; }
}

public class StartTicketSubscriptionDto
{
    public Guid GameTemplateId { get; set; }
    public Guid PlayerId { get; set; }
    public Guid WalletId { get; set; }
    public int[] PickedNumbers { get; set; } = [];
    public double Price { get; set; }
}

public class SubscriptionDto
{
    public Guid Id { get; set; }
    public Guid GameTemplateId { get; set; }
    public Guid PlayerId { get; set; }
    public int[] PickedNumbers { get; set; } = [];
    public double Price { get; set; }
    public bool IsExpired { get; set; }
    public DateTime BoughtAt { get; set; }
    public GameTemplateResponseDto? GameTemplate { get; set; }
}