using System;
using System.Collections.Generic;

namespace Dataaccess;

public partial class Transaction
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string TransactionNumber { get; set; } = null!;

    public int Amount { get; set; }

    public string Status { get; set; } = null!;

    public Guid? ReviewedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User? ReviewedByNavigation { get; set; }

    public virtual User User { get; set; } = null!;
}
