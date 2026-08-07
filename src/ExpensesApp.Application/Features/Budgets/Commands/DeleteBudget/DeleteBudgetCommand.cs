using ExpensesApp.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpensesApp.Application.Features.Budgets.Commands.DeleteBudget;

public sealed record DeleteBudgetCommand(int Id);

public sealed class DeleteBudgetCommandHandler
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public DeleteBudgetCommandHandler(
        IApplicationDbContext context,
        ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteBudgetCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(_currentUser.UserId))
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var budget = await _context.Budgets
            .FirstOrDefaultAsync(b => b.Id == command.Id && b.UserId == _currentUser.UserId, cancellationToken);

        if (budget == null)
        {
            throw new InvalidOperationException("Budget not found or does not belong to user.");
        }

        _context.Budgets.Remove(budget);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
