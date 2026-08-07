using ExpensesApp.Domain.Enums;

namespace ExpensesApp.Application.Common.Models;

public sealed record CategoryBreakdownDto
{
    public int CategoryId { get; init; }
    public string CategoryName { get; init; } = string.Empty;
    public TransactionType Type { get; init; }
    public decimal Total { get; init; }
    public double PercentOfTypeTotal { get; init; }
}

public sealed record MonthlyReportDto
{
    public int Month { get; init; }
    public int Year { get; init; }
    public decimal TotalIncome { get; init; }
    public decimal TotalExpense { get; init; }
    public decimal Net => TotalIncome - TotalExpense;
    public IReadOnlyList<CategoryBreakdownDto> IncomeByCategory { get; init; } = [];
    public IReadOnlyList<CategoryBreakdownDto> ExpenseByCategory { get; init; } = [];
}
