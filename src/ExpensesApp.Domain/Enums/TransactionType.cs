namespace ExpensesApp.Domain.Enums;

/// <summary>
/// Represents the type of a financial transaction.
/// </summary>
public enum TransactionType
{
    /// <summary>
    /// Money coming in.
    /// </summary>
    Income = 1,

    /// <summary>
    /// Money going out.
    /// </summary>
    Expense = 2
}
