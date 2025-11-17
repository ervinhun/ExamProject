using System;
using System.Collections.Generic;

namespace DataAccess;

public partial class User
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? PhoneNo { get; set; }

    public Guid? RoleId { get; set; }

    public string? JwtToken { get; set; }

    public string? JwtRefreshToken { get; set; }

    public bool? IsActive { get; set; }

    public DateOnly? ActiveStatusExpiryDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<Board> Boards { get; set; } = new List<Board>();

    public virtual ICollection<PlayingBoard> PlayingBoards { get; set; } = new List<PlayingBoard>();

    public virtual ICollection<PlayingHistory> PlayingHistories { get; set; } = new List<PlayingHistory>();

    public virtual Role? Role { get; set; }

    public virtual ICollection<Transaction> TransactionReviewedByNavigations { get; set; } = new List<Transaction>();

    public virtual ICollection<Transaction> TransactionUsers { get; set; } = new List<Transaction>();

    public virtual ICollection<UserBalance> UserBalances { get; set; } = new List<UserBalance>();

    public virtual ICollection<UserHistory> UserHistories { get; set; } = new List<UserHistory>();
}
