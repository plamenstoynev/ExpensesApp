using ExpensesApp.Application.Common.Interfaces;
using ExpensesApp.Application.Common.Models;
using ExpensesApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ExpensesApp.Application.Features.Reports.Queries.GetMonthlyReport;

public sealed record GetMonthlyReportQuery
{
    public required int Month { get; init; }
    public required int Year { get; init; }
}

public sealed class GetMonthlyReportQueryHandler
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetMonthlyReportQueryHandler(
        IApplicationDbContext context,
        ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<MonthlyReportDto> Handle(
        GetMonthlyReportQuery query,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(_currentUser.UserId))
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var rangeStart = new DateTime(query.Year, query.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var rangeEnd = rangeStart.AddMonths(1);

        var grouped = await _context.Transactions
            .Where(t => t.UserId == _currentUser.UserId
                && t.TransactionDate >= rangeStart
                && t.TransactionDate < rangeEnd)
            .GroupBy(t => new { t.CategoryId, CategoryName = t.Category != null ? t.Category.Name : "Uncategorized", t.Type })
            .Select(g => new CategoryBreakdownDto
            {
                CategoryId = g.Key.CategoryId,
                CategoryName = g.Key.CategoryName ?? "Uncategorized",
                Type = g.Key.Type,
                Total = g.Sum(t => t.Amount)
            })
            .ToListAsync(cancellationToken);

        var incomeByCategory = grouped.Where(g => g.Type == TransactionType.Income).OrderByDescending(g => g.Total).ToList();
        var expenseByCategory = grouped.Where(g => g.Type == TransactionType.Expense).OrderByDescending(g => g.Total).ToList();

        var totalIncome = incomeByCategory.Sum(g => g.Total);
        var totalExpense = expenseByCategory.Sum(g => g.Total);

        incomeByCategory = incomeByCategory
            .Select(g => g with { PercentOfTypeTotal = totalIncome == 0 ? 0 : (double)(g.Total / totalIncome * 100) })
            .ToList();

        expenseByCategory = expenseByCategory
            .Select(g => g with { PercentOfTypeTotal = totalExpense == 0 ? 0 : (double)(g.Total / totalExpense * 100) })
            .ToList();

        return new MonthlyReportDto
        {
            Month = query.Month,
            Year = query.Year,
            TotalIncome = totalIncome,
            TotalExpense = totalExpense,
            IncomeByCategory = incomeByCategory,
            ExpenseByCategory = expenseByCategory
        };
    }
}
