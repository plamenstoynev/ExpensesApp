using ExpenseTracker.Application.Common.Interfaces;
using ExpenseTracker.Application.Common.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Features.Transactions.Commands.UpdateTransaction;

public sealed record UpdateTransactionCommand
{
    public required int Id { get; init; }
    public required decimal Amount { get; init; }
    public string? Description { get; init; }
    public required DateTime TransactionDate { get; init; }
}

public sealed class UpdateTransactionCommandValidator : AbstractValidator<UpdateTransactionCommand>
{
    public UpdateTransactionCommandValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than zero.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

        RuleFor(x => x.TransactionDate)
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(365))
            .WithMessage("Transaction date cannot be more than 365 days in the future.");
    }
}

public sealed class UpdateTransactionCommandHandler
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IValidator<UpdateTransactionCommand> _validator;

    public UpdateTransactionCommandHandler(
        IApplicationDbContext context,
        ICurrentUser currentUser,
        IValidator<UpdateTransactionCommand> validator)
    {
        _context = context;
        _currentUser = currentUser;
        _validator = validator;
    }

    public async Task<TransactionDto> Handle(
        UpdateTransactionCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        if (string.IsNullOrEmpty(_currentUser.UserId))
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var transaction = await _context.Transactions
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == command.Id && t.UserId == _currentUser.UserId, cancellationToken);

        if (transaction == null)
        {
            throw new InvalidOperationException("Transaction not found or does not belong to user.");
        }

        transaction.UpdateAmount(command.Amount);
        transaction.UpdateDescription(command.Description);
        transaction.UpdateTransactionDate(command.TransactionDate);

        await _context.SaveChangesAsync(cancellationToken);

        return new TransactionDto
        {
            Id = transaction.Id,
            UserId = transaction.UserId,
            Type = transaction.Type,
            Amount = transaction.Amount,
            Currency = transaction.Currency,
            CategoryId = transaction.CategoryId,
            CategoryName = transaction.Category?.Name,
            Description = transaction.Description,
            TransactionDate = transaction.TransactionDate,
            CreatedAtUtc = transaction.CreatedAtUtc,
            UpdatedAtUtc = transaction.UpdatedAtUtc
        };
    }
}
