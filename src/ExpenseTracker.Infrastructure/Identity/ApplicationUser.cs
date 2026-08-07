using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Infrastructure.Identity;

/// <summary>
/// Represents an application user with Identity integration.
/// </summary>
public sealed class ApplicationUser : IdentityUser
{
    /// <summary>
    /// Gets or sets the date when the user registered.
    /// </summary>
    public DateTime RegisteredAtUtc { get; set; } = DateTime.UtcNow;
}
