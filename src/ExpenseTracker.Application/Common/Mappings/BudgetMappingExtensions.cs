using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Application.Common.Mappings;

public static class BudgetMappingExtensions
{
    public static BudgetDto ToDto(this Budget budget, string? categoryName, decimal spent) => new()
    {
        Id = budget.Id,
        UserId = budget.UserId,
        CategoryId = budget.CategoryId,
        CategoryName = categoryName,
        Amount = budget.Amount,
        Month = budget.Month,
        Year = budget.Year,
        Spent = spent,
        CreatedAtUtc = budget.CreatedAtUtc,
        UpdatedAtUtc = budget.UpdatedAtUtc
    };
}
