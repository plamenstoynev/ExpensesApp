namespace ExpensesApp.Application.Common.Interfaces;

/// <summary>
/// Provides information about the currently authenticated user.
/// </summary>
public interface ICurrentUser
{
    /// <summary>
    /// Gets the ID of the currently authenticated user.
    /// </summary>
    string? UserId { get; }

    /// <summary>
    /// Gets a value indicating whether the user is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }
}
