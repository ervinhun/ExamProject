using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public partial class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Board> Boards { get; set; }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<PlayingBoard> PlayingBoards { get; set; }

    public virtual DbSet<PlayingHistory> PlayingHistories { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserBalance> UserBalances { get; set; }

    public virtual DbSet<UserHistory> UserHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<Board>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("board_pkey");

            entity.ToTable("board", "gameapp");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.FieldCount).HasColumnName("field_count");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.Numbers).HasColumnName("numbers");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Boards)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("board_user_id_fkey");
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("game_pkey");

            entity.ToTable("game", "gameapp");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.ClosedAt).HasColumnName("closed_at");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.IsClosed)
                .HasDefaultValue(false)
                .HasColumnName("is_closed");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.WinningNumber1).HasColumnName("winning_number_1");
            entity.Property(e => e.WinningNumber2).HasColumnName("winning_number_2");
            entity.Property(e => e.WinningNumber3).HasColumnName("winning_number_3");
        });

        modelBuilder.Entity<PlayingBoard>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("playing_board_pkey");

            entity.ToTable("playing_board", "gameapp");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.BoardId).HasColumnName("board_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.FieldCount).HasColumnName("field_count");
            entity.Property(e => e.GameId).HasColumnName("game_id");
            entity.Property(e => e.IsRepeat)
                .HasDefaultValue(false)
                .HasColumnName("is_repeat");
            entity.Property(e => e.IsWinningBoard)
                .HasDefaultValue(false)
                .HasColumnName("is_winning_board");
            entity.Property(e => e.Numbers).HasColumnName("numbers");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.RepeatCountRemaining)
                .HasDefaultValue(0)
                .HasColumnName("repeat_count_remaining");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Board).WithMany(p => p.PlayingBoards)
                .HasForeignKey(d => d.BoardId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("playing_board_board_id_fkey");

            entity.HasOne(d => d.Game).WithMany(p => p.PlayingBoards)
                .HasForeignKey(d => d.GameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("playing_board_game_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.PlayingBoards)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("playing_board_user_id_fkey");
        });

        modelBuilder.Entity<PlayingHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("playing_history_pkey");

            entity.ToTable("playing_history", "gameapp");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.PublicId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("public_id");
            entity.Property(e => e.TicketId).HasColumnName("ticket_id");
            entity.Property(e => e.Title).HasColumnName("title");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Ticket).WithMany(p => p.PlayingHistories)
                .HasForeignKey(d => d.TicketId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("playing_history_ticket_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.PlayingHistories)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("playing_history_user_id_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("roles_pkey");

            entity.ToTable("roles", "gameapp");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("transaction_pkey");

            entity.ToTable("transaction", "gameapp");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.ReviewedBy).HasColumnName("reviewed_by");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.TransactionNumber).HasColumnName("transaction_number");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.ReviewedByNavigation).WithMany(p => p.TransactionReviewedByNavigations)
                .HasForeignKey(d => d.ReviewedBy)
                .HasConstraintName("transaction_reviewed_by_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.TransactionUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("transaction_user_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users", "gameapp");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.ActiveStatusExpiryDate).HasColumnName("active_status_expiry_date");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.FullName).HasColumnName("full_name");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.JwtRefreshToken)
                .HasMaxLength(255)
                .HasColumnName("jwt_refresh_token");
            entity.Property(e => e.JwtToken)
                .HasMaxLength(255)
                .HasColumnName("jwt_token");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.PhoneNo)
                .HasMaxLength(100)
                .HasColumnName("phone_no");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("users_role_id_fkey");
        });

        modelBuilder.Entity<UserBalance>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_balance_pkey");

            entity.ToTable("user_balance", "gameapp");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.Balance)
                .HasDefaultValue(0)
                .HasColumnName("balance");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserBalances)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_balance_user_id_fkey");
        });

        modelBuilder.Entity<UserHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_history_pkey");

            entity.ToTable("user_history", "gameapp");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.PublicId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("public_id");
            entity.Property(e => e.Title).HasColumnName("title");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserHistories)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_history_user_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
