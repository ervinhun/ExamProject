using System;
using System.Collections.Generic;

namespace DataAccess;

public partial class Wallet
{
    public Guid Id { get; set; }

    public decimal Balance { get; set; }

    public Guid UserId { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<Player> Players { get; set; } = new List<Player>();

    public virtual User User { get; set; } = null!;
}
