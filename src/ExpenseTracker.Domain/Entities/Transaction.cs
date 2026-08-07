using ExpenseTracker.Domain.Common;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.Exceptions;

namespace ExpenseTracker.Domain.Entities;

/// <summary>
/// Represents a financial transaction (income or expense).
/// </summary>
public sealed class Transaction : BaseEntity
{
    private const int MaxDescriptionLength = 500;
    private const int MaxFutureDays = 365;

    private decimal _amount;
    private string? _description;
    private DateTime _transactionDate;

    /// <summary>
    /// The ID of the user who owns this transaction.
    /// </summary>
    public required string UserId { get; init; }

    /// <summary>
    /// Type of transaction (Income or Expense).
    /// </summary>
    public required TransactionType Type { get; init; }

    /// <summary>
    /// Transaction amount. Must be greater than zero.
    /// </summary>
    public required decimal Amount
    {
        get => _amount;
        init
        {
            if (value <= 0)
                throw new InvalidTransactionException("Amount must be greater than zero.");

            _amount = value;
        }
    }

    /// <summary>
    /// Currency of the transaction.
    /// </summary>
    public required Currency Currency { get; init; }

    /// <summary>
    /// The ID of the category this transaction belongs to.
    /// </summary>
    public required int CategoryId { get; init; }

    /// <summary>
    /// Optional description of the transaction.
    /// </summary>
    public string? Description
    {
        get => _description;
        init
        {
            if (value != null && value.Length > MaxDescriptionLength)
                throw new InvalidTransactionException(
                    $"Description cannot exceed {MaxDescriptionLength} characters.");

            _description = value?.Trim();
        }
    }

    /// <summary>
    /// Date when the transaction occurred.
    /// </summary>
    public required DateTime TransactionDate
    {
        get => _transactionDate;
        init
        {
            var maxAllowedDate = DateTime.UtcNow.AddDays(MaxFutureDays);
            if (value > maxAllowedDate)
                throw new InvalidTransactionException(
                    $"Transaction date cannot be more than {MaxFutureDays} days in the future.");

            _transactionDate = value;
        }
    }

    /// <summary>
    /// Navigation property to the category.
    /// </summary>
    public Category? Category { get; init; }

    /// <summary>
    /// Updates the transaction description.
    /// </summary>
    public void UpdateDescription(string? description)
    {
        if (description != null && description.Length > MaxDescriptionLength)
            throw new InvalidTransactionException(
                $"Description cannot exceed {MaxDescriptionLength} characters.");

        _description = description?.Trim();
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the transaction amount.
    /// </summary>
    public void UpdateAmount(decimal amount)
    {
        if (amount <= 0)
            throw new InvalidTransactionException("Amount must be greater than zero.");

        _amount = amount;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the transaction date.
    /// </summary>
    public void UpdateTransactionDate(DateTime transactionDate)
    {
        var maxAllowedDate = DateTime.UtcNow.AddDays(MaxFutureDays);
        if (transactionDate > maxAllowedDate)
            throw new InvalidTransactionException(
                $"Transaction date cannot be more than {MaxFutureDays} days in the future.");

        _transactionDate = transactionDate;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
