using ExpensesApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExpensesApp.Application.Common.Interfaces;

/// <summary>
/// Abstraction for the application database context.
/// Allows the Application layer to access data without depending on EF Core implementation details.
/// </summary>
public interface IApplicationDbContext
{
    DbSet<Transaction> Transactions { get; }
    DbSet<Category> Categories { get; }
    DbSet<Budget> Budgets { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
