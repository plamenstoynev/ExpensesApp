using ExpensesApp.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpensesApp.Application.Features.Transactions.Commands.DeleteTransaction;

public sealed record DeleteTransactionCommand(int Id);

public sealed class DeleteTransactionCommandHandler
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public DeleteTransactionCommandHandler(
        IApplicationDbContext context,
        ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteTransactionCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(_currentUser.UserId))
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var transaction = await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == command.Id && t.UserId == _currentUser.UserId, cancellationToken);

        if (transaction == null)
        {
            throw new InvalidOperationException("Transaction not found or does not belong to user.");
        }

        _context.Transactions.Remove(transaction);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
