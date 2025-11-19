/*
using System;
using System.Collections.Generic;

namespace DataAccess;

public partial class Game
{
    public Guid Id { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public bool? IsClosed { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ClosedAt { get; set; }

    public virtual ICollection<PlayingBoard> PlayingBoards { get; set; } = new List<PlayingBoard>();

    public virtual ICollection<PlayingHistory> PlayingHistories { get; set; } = new List<PlayingHistory>();

    public virtual ICollection<WinningNumber> WinningNumbers { get; set; } = new List<WinningNumber>();
}
*/
