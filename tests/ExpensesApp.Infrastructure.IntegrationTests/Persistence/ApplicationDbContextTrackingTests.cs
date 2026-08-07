using ExpensesApp.Domain.Entities;
using ExpensesApp.Domain.Enums;
using ExpensesApp.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ExpensesApp.Infrastructure.IntegrationTests.Persistence;

/// <summary>
/// Verifies the NoTracking-by-default ApplicationDbContext still allows explicit
/// AsTracking() updates to persist, since the write path (UpdateTransactionCommandHandler)
/// relies on this combination to save changes.
/// </summary>
public sealed class ApplicationDbContextTrackingTests : IDisposable
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"expensesapp-test-{Guid.NewGuid():N}.db");

    private ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite($"Data Source={_dbPath}")
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task Update_via_AsTracking_persists_changes_despite_NoTracking_default()
    {
        await using (var setup = CreateContext())
        {
            await setup.Database.MigrateAsync();

            var category = new Category
            {
                UserId = "user-1",
                Name = "Groceries",
                Type = TransactionType.Expense
            };
            setup.Categories.Add(category);
            await setup.SaveChangesAsync();

            setup.Transactions.Add(new Transaction
            {
                UserId = "user-1",
                Type = TransactionType.Expense,
                Amount = 10m,
                Currency = Currency.USD,
                CategoryId = category.Id,
                TransactionDate = DateTime.UtcNow
            });
            await setup.SaveChangesAsync();
        }

        // Read-then-mutate-then-save, mirroring UpdateTransactionCommandHandler exactly.
        await using (var updateContext = CreateContext())
        {
            var transaction = await updateContext.Transactions
                .AsTracking()
                .Include(t => t.Category)
                .FirstAsync();

            transaction.UpdateAmount(42m);
            await updateContext.SaveChangesAsync();
        }

        await using var verifyContext = CreateContext();
        var reloaded = await verifyContext.Transactions.FirstAsync();
        reloaded.Amount.Should().Be(42m);
    }

    [Fact]
    public async Task Query_without_AsTracking_does_not_persist_mutations()
    {
        await using (var setup = CreateContext())
        {
            await setup.Database.MigrateAsync();

            var category = new Category
            {
                UserId = "user-1",
                Name = "Salary",
                Type = TransactionType.Income
            };
            setup.Categories.Add(category);
            await setup.SaveChangesAsync();

            setup.Transactions.Add(new Transaction
            {
                UserId = "user-1",
                Type = TransactionType.Income,
                Amount = 100m,
                Currency = Currency.USD,
                CategoryId = category.Id,
                TransactionDate = DateTime.UtcNow
            });
            await setup.SaveChangesAsync();
        }

        await using (var readContext = CreateContext())
        {
            // No AsTracking() here - relies on the DbContext's NoTracking default.
            var transaction = await readContext.Transactions.FirstAsync();
            transaction.UpdateAmount(999m);
            await readContext.SaveChangesAsync();
        }

        await using var verifyContext = CreateContext();
        var reloaded = await verifyContext.Transactions.FirstAsync();
        reloaded.Amount.Should().Be(100m, "mutating an untracked entity must not silently persist");
    }

    public void Dispose()
    {
        if (File.Exists(_dbPath))
        {
            File.Delete(_dbPath);
        }
    }
}
