using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.Common.Models;

public sealed record TransactionDto
{
    public int Id { get; init; }
    public string UserId { get; init; } = string.Empty;
    public TransactionType Type { get; init; }
    public decimal Amount { get; init; }
    public Currency Currency { get; init; }
    public int CategoryId { get; init; }
    public string? CategoryName { get; init; }
    public string? Description { get; init; }
    public DateTime TransactionDate { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
