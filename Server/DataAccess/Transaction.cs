/*using System;
using System.Collections.Generic;

namespace DataAccess;

public partial class Transaction
{
    public Guid Id { get; set; }

    public Guid PlayerId { get; set; }

    public string TransactionNumber { get; set; } = null!;

    public decimal Amount { get; set; }

    public string Status { get; set; } = null!;

    public Guid? ReviewedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Player Player { get; set; } = null!;

    public virtual Admin? ReviewedByNavigation { get; set; }
}
/#1#*/