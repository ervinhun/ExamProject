using System;
using System.Collections.Generic;

namespace DataAccess;

public partial class Admin
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual User User { get; set; } = null!;
}
