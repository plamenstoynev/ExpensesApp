using ExpensesApp.Application.Common.Interfaces;
using ExpensesApp.Application.Common.Models;
using ExpensesApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ExpensesApp.Application.Features.Categories.Queries.GetCategories;

public sealed record GetCategoriesQuery
{
    public TransactionType? Type { get; init; }
}

public sealed class GetCategoriesQueryHandler
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetCategoriesQueryHandler(
        IApplicationDbContext context,
        ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<CategoryDto>> Handle(
        GetCategoriesQuery query,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(_currentUser.UserId))
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var queryable = _context.Categories
            .Where(c => c.UserId == _currentUser.UserId);

        if (query.Type.HasValue)
        {
            queryable = queryable.Where(c => c.Type == query.Type.Value);
        }

        return await queryable
            .OrderBy(c => c.Type)
            .ThenBy(c => c.Name)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                UserId = c.UserId,
                Name = c.Name,
                Type = c.Type,
                Icon = c.Icon,
                CreatedAtUtc = c.CreatedAtUtc,
                UpdatedAtUtc = c.UpdatedAtUtc
            })
            .ToListAsync(cancellationToken);
    }
}
