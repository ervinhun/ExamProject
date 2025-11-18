using System;

namespace Api.Dto;

public class TransactionDto
{
    public Guid Id { get; set; }
    public Guid PlayerId { get; set; }
    public string TransactionNumber { get; set; } = null!;
    public decimal Amount { get; set; }
    public string Status { get; set; } = null!;
    public Guid? ReviewedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateTransactionDto
{
    public Guid PlayerId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = null!;
}

public class UpdateTransactionDto
{
    public Guid? PlayerId { get; set; }
    public decimal? Amount { get; set; }
    public string? Status { get; set; }
    public Guid? ReviewedBy { get; set; }
}
