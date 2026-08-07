using ExpenseTracker.Application.Common.Interfaces;
using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Features.Transactions.Queries.GetTransactions;

public sealed record GetTransactionsQuery
{
    public TransactionType? Type { get; init; }
    public int? CategoryId { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public sealed record PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];
    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}

public sealed class GetTransactionsQueryHandler
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetTransactionsQueryHandler(
        IApplicationDbContext context,
        ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<TransactionDto>> Handle(
        GetTransactionsQuery query,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(_currentUser.UserId))
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var queryable = _context.Transactions
            .Where(t => t.UserId == _currentUser.UserId);

        // Apply filters
        if (query.Type.HasValue)
        {
            queryable = queryable.Where(t => t.Type == query.Type.Value);
        }

        if (query.CategoryId.HasValue)
        {
            queryable = queryable.Where(t => t.CategoryId == query.CategoryId.Value);
        }

        if (query.StartDate.HasValue)
        {
            queryable = queryable.Where(t => t.TransactionDate >= query.StartDate.Value);
        }

        if (query.EndDate.HasValue)
        {
            queryable = queryable.Where(t => t.TransactionDate <= query.EndDate.Value);
        }

        var totalCount = await queryable.CountAsync(cancellationToken);

        var transactions = await queryable
            .OrderByDescending(t => t.TransactionDate)
            .ThenByDescending(t => t.CreatedAtUtc)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(t => new TransactionDto
            {
                Id = t.Id,
                UserId = t.UserId,
                Type = t.Type,
                Amount = t.Amount,
                Currency = t.Currency,
                CategoryId = t.CategoryId,
                CategoryName = t.Category != null ? t.Category.Name : null,
                Description = t.Description,
                TransactionDate = t.TransactionDate,
                CreatedAtUtc = t.CreatedAtUtc,
                UpdatedAtUtc = t.UpdatedAtUtc
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<TransactionDto>
        {
            Items = transactions,
            TotalCount = totalCount,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };
    }
}
