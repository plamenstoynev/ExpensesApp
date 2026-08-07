namespace ExpenseTracker.Domain.Common;

/// <summary>
/// Base entity with common audit properties.
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; protected set; }

    protected BaseEntity()
    {
        CreatedAtUtc = DateTime.UtcNow;
    }
}
