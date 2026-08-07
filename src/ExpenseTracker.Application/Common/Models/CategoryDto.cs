using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.Common.Models;

public sealed record CategoryDto
{
    public int Id { get; init; }
    public string UserId { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public TransactionType Type { get; init; }
    public string? Icon { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
