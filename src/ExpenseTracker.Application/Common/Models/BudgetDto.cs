namespace ExpenseTracker.Application.Common.Models;

public sealed record BudgetDto
{
    public int Id { get; init; }
    public string UserId { get; init; } = string.Empty;
    public int CategoryId { get; init; }
    public string? CategoryName { get; init; }
    public decimal Amount { get; init; }
    public int Month { get; init; }
    public int Year { get; init; }
    public decimal Spent { get; init; }
    public decimal Remaining => Amount - Spent;
    public double PercentUsed => Amount == 0 ? 0 : Math.Min(100, (double)(Spent / Amount * 100));
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
