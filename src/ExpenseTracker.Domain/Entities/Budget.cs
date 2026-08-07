using ExpenseTracker.Domain.Common;
using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Domain.Entities;

/// <summary>
/// Represents a monthly budget for a specific expense category.
/// </summary>
public sealed class Budget : BaseEntity
{
    private decimal _amount;

    /// <summary>
    /// The ID of the user who owns this budget.
    /// </summary>
    public required string UserId { get; init; }

    /// <summary>
    /// The category this budget applies to.
    /// Must be an expense category.
    /// </summary>
    public required int CategoryId { get; init; }

    /// <summary>
    /// Budget amount. Must be greater than zero.
    /// </summary>
    public required decimal Amount
    {
        get => _amount;
        init
        {
            if (value <= 0)
                throw new ArgumentException("Budget amount must be greater than zero.", nameof(Amount));

            _amount = value;
        }
    }

    /// <summary>
    /// The month this budget applies to (1-12).
    /// </summary>
    public required int Month { get; init; }

    /// <summary>
    /// The year this budget applies to.
    /// </summary>
    public required int Year { get; init; }

    /// <summary>
    /// Navigation property to the category.
    /// </summary>
    public Category? Category { get; init; }

    /// <summary>
    /// Updates the budget amount.
    /// </summary>
    public void UpdateAmount(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Budget amount must be greater than zero.", nameof(amount));

        _amount = amount;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Validates that the budget applies only to expense categories.
    /// </summary>
    public void ValidateCategoryType(TransactionType categoryType)
    {
        if (categoryType != TransactionType.Expense)
            throw new InvalidOperationException("Budgets can only be created for expense categories.");
    }
}
