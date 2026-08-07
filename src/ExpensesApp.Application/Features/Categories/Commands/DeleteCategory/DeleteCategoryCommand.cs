using ExpensesApp.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpensesApp.Application.Features.Categories.Commands.DeleteCategory;

public sealed record DeleteCategoryCommand(int Id);

public sealed class DeleteCategoryCommandHandler
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public DeleteCategoryCommandHandler(
        IApplicationDbContext context,
        ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteCategoryCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(_currentUser.UserId))
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == command.Id && c.UserId == _currentUser.UserId, cancellationToken);

        if (category == null)
        {
            throw new InvalidOperationException("Category not found or does not belong to user.");
        }

        var isInUse = await _context.Transactions.AnyAsync(t => t.CategoryId == command.Id, cancellationToken)
            || await _context.Budgets.AnyAsync(b => b.CategoryId == command.Id, cancellationToken);

        if (isInUse)
        {
            throw new InvalidOperationException("Category cannot be deleted because it has transactions or budgets associated with it.");
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
