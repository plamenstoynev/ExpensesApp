using ExpensesApp.Domain.Common;
using ExpensesApp.Domain.Enums;

namespace ExpensesApp.Domain.Entities;

/// <summary>
/// Represents a transaction category (e.g., Food, Salary).
/// Each category belongs to a specific user and transaction type.
/// </summary>
public sealed class Category : BaseEntity
{
    private string _name = string.Empty;

    /// <summary>
    /// The ID of the user who owns this category.
    /// </summary>
    public required string UserId { get; init; }

    /// <summary>
    /// Category name (e.g., "Food", "Salary").
    /// </summary>
    public required string Name
    {
        get => _name;
        init
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Category name cannot be empty.", nameof(Name));

            if (value.Length > 100)
                throw new ArgumentException("Category name cannot exceed 100 characters.", nameof(Name));

            _name = value.Trim();
        }
    }

    /// <summary>
    /// The type of transactions this category applies to (Income or Expense).
    /// </summary>
    public required TransactionType Type { get; init; }

    /// <summary>
    /// Optional icon identifier for UI display.
    /// </summary>
    public string? Icon { get; init; }

    /// <summary>
    /// Navigation property for transactions in this category.
    /// </summary>
    public ICollection<Transaction> Transactions { get; init; } = new List<Transaction>();

    /// <summary>
    /// Navigation property for budgets associated with this category.
    /// </summary>
    public ICollection<Budget> Budgets { get; init; } = new List<Budget>();
}
