namespace ExpensesApp.Domain.Exceptions;

/// <summary>
/// Thrown when transaction data violates domain rules.
/// </summary>
public sealed class InvalidTransactionException : DomainException
{
    public InvalidTransactionException(string message) : base(message)
    {
    }
}
