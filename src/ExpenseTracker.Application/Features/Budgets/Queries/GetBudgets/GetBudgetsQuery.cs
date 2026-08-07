using ExpenseTracker.Application.Common.Interfaces;
using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Features.Budgets.Queries.GetBudgets;

public sealed record GetBudgetsQuery
{
    public required int Month { get; init; }
    public required int Year { get; init; }
}

public sealed class GetBudgetsQueryHandler
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetBudgetsQueryHandler(
        IApplicationDbContext context,
        ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<BudgetDto>> Handle(
        GetBudgetsQuery query,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(_currentUser.UserId))
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var budgets = await _context.Budgets
            .AsNoTracking()
            .Include(b => b.Category)
            .Where(b => b.UserId == _currentUser.UserId && b.Month == query.Month && b.Year == query.Year)
            .OrderBy(b => b.Category != null ? b.Category.Name : string.Empty)
            .ToListAsync(cancellationToken);

        if (budgets.Count == 0)
        {
            return new List<BudgetDto>();
        }

        var categoryIds = budgets.Select(b => b.CategoryId).ToList();

        var rangeStart = new DateTime(query.Year, query.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var rangeEnd = rangeStart.AddMonths(1);

        var spentByCategory = await _context.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == _currentUser.UserId
                && t.Type == TransactionType.Expense
                && categoryIds.Contains(t.CategoryId)
                && t.TransactionDate >= rangeStart
                && t.TransactionDate < rangeEnd)
            .GroupBy(t => t.CategoryId)
            .Select(g => new { CategoryId = g.Key, Total = g.Sum(t => t.Amount) })
            .ToDictionaryAsync(x => x.CategoryId, x => x.Total, cancellationToken);

        return budgets.Select(b => new BudgetDto
        {
            Id = b.Id,
            UserId = b.UserId,
            CategoryId = b.CategoryId,
            CategoryName = b.Category?.Name,
            Amount = b.Amount,
            Month = b.Month,
            Year = b.Year,
            Spent = spentByCategory.GetValueOrDefault(b.CategoryId, 0m),
            CreatedAtUtc = b.CreatedAtUtc,
            UpdatedAtUtc = b.UpdatedAtUtc
        }).ToList();
    }
}
