using ExpensesApp.Application.Common.Interfaces;
using ExpensesApp.Application.Common.Mappings;
using ExpensesApp.Application.Common.Models;
using ExpensesApp.Domain.Entities;
using ExpensesApp.Domain.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ExpensesApp.Application.Features.Transactions.Commands.CreateTransaction;

public sealed record CreateTransactionCommand
{
    public required TransactionType Type { get; init; }
    public required decimal Amount { get; init; }
    public required Currency Currency { get; init; }
    public required int CategoryId { get; init; }
    public string? Description { get; init; }
    public required DateTime TransactionDate { get; init; }
}

public sealed class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
{
    public CreateTransactionCommandValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than zero.");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Category is required.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

        RuleFor(x => x.TransactionDate)
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(365))
            .WithMessage("Transaction date cannot be more than 365 days in the future.");
    }
}

public sealed class CreateTransactionCommandHandler
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IValidator<CreateTransactionCommand> _validator;

    public CreateTransactionCommandHandler(
        IApplicationDbContext context,
        ICurrentUser currentUser,
        IValidator<CreateTransactionCommand> validator)
    {
        _context = context;
        _currentUser = currentUser;
        _validator = validator;
    }

    public async Task<TransactionDto> Handle(
        CreateTransactionCommand command,
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

        // Verify category belongs to user and type matches
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == command.CategoryId && c.UserId == _currentUser.UserId, cancellationToken);

        if (category == null)
        {
            throw new InvalidOperationException("Category not found or does not belong to user.");
        }

        if (category.Type != command.Type)
        {
            throw new InvalidOperationException($"Category is for {category.Type} transactions, but transaction type is {command.Type}.");
        }

        var transaction = new Transaction
        {
            UserId = _currentUser.UserId,
            Type = command.Type,
            Amount = command.Amount,
            Currency = command.Currency,
            CategoryId = command.CategoryId,
            Description = command.Description,
            TransactionDate = command.TransactionDate
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync(cancellationToken);

        return transaction.ToDto(category.Name);
    }
}
