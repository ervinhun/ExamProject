using System;
using System.Collections.Generic;

namespace Dataaccess;

public partial class Board
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Numbers { get; set; } = null!;

    public int FieldCount { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<PlayingBoard> PlayingBoards { get; set; } = new List<PlayingBoard>();

    public virtual User User { get; set; } = null!;
}
