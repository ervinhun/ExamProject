/*
using System;
using System.Collections.Generic;

namespace DataAccess;

public class Usert
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? PhoneNo { get; set; }

    public string? JwtToken { get; set; }

    public string? JwtRefreshToken { get; set; }

    public DateOnly? ActiveStatusExpiryDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<Admin> Admins { get; set; } = new List<Admin>();

    public virtual ICollection<Board> Boards { get; set; } = new List<Board>();

    public virtual ICollection<Player> Players { get; set; } = new List<Player>();

    public virtual ICollection<PlayingBoard> PlayingBoards { get; set; } = new List<PlayingBoard>();

    public virtual ICollection<PlayingHistory> PlayingHistories { get; set; } = new List<PlayingHistory>();

    public virtual ICollection<UserHistory> UserHistories { get; set; } = new List<UserHistory>();

    public virtual ICollection<UserRole> UserRoleCreatedByNavigations { get; set; } = new List<UserRole>();

    public virtual ICollection<UserRole> UserRoleUsers { get; set; } = new List<UserRole>();

    public virtual Wallet? Wallet { get; set; }
}
*/
