using System;
using System.Collections.Generic;

namespace DataAccess;

public partial class UserBalance
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public int Balance { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual User User { get; set; } = null!;
}
