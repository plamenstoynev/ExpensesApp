using ExpenseTracker.Application.Common.Interfaces;
using ExpenseTracker.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Features.Transactions.Queries.GetTransactionById;

public sealed record GetTransactionByIdQuery(int Id);

public sealed class GetTransactionByIdQueryHandler
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetTransactionByIdQueryHandler(
        IApplicationDbContext context,
        ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<TransactionDto?> Handle(
        GetTransactionByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(_currentUser.UserId))
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        return await _context.Transactions
            .AsNoTracking()
            .Include(t => t.Category)
            .Where(t => t.Id == query.Id && t.UserId == _currentUser.UserId)
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
            .FirstOrDefaultAsync(cancellationToken);
    }
}
