/*
using System;
using System.Collections.Generic;

namespace DataAccess;

public partial class PlayingBoard
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid BoardId { get; set; }

    public Guid GameId { get; set; }

    public string Numbers { get; set; } = null!;

    public int FieldCount { get; set; }

    public decimal Price { get; set; }

    public bool? IsRepeat { get; set; }

    public int? RepeatCountRemaining { get; set; }

    public bool? IsWinningBoard { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Board Board { get; set; } = null!;

    public virtual Game Game { get; set; } = null!;

    public virtual ICollection<PlayingHistory> PlayingHistories { get; set; } = new List<PlayingHistory>();

    public virtual ICollection<PlayingNumber> PlayingNumbers { get; set; } = new List<PlayingNumber>();

    public virtual User User { get; set; } = null!;
}
*/
