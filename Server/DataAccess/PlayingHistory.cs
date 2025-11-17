using System;
using System.Collections.Generic;

namespace DataAccess;

public partial class PlayingHistory
{
    public long Id { get; set; }

    public Guid PublicId { get; set; }

    public Guid UserId { get; set; }

    public Guid TicketId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual PlayingBoard Ticket { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
