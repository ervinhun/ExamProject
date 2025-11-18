using System;
using System.Collections.Generic;

namespace DataAccess;

public partial class WinningNumber
{
    public Guid Id { get; set; }

    public Guid GameId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Game Game { get; set; } = null!;
}
