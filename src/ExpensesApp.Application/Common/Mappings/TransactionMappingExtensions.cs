using ExpensesApp.Application.Common.Models;
using ExpensesApp.Domain.Entities;

namespace ExpensesApp.Application.Common.Mappings;

public static class TransactionMappingExtensions
{
    public static TransactionDto ToDto(this Transaction transaction, string? categoryName) => new()
    {
        Id = transaction.Id,
        UserId = transaction.UserId,
        Type = transaction.Type,
        Amount = transaction.Amount,
        Currency = transaction.Currency,
        CategoryId = transaction.CategoryId,
        CategoryName = categoryName,
        Description = transaction.Description,
        TransactionDate = transaction.TransactionDate,
        CreatedAtUtc = transaction.CreatedAtUtc,
        UpdatedAtUtc = transaction.UpdatedAtUtc
    };
}
