using ExpenseTracker.Application.Common.Interfaces;
using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Features.Budgets.Commands.CreateBudget;

public sealed record CreateBudgetCommand
{
    public required int CategoryId { get; init; }
    public required decimal Amount { get; init; }
    public required int Month { get; init; }
    public required int Year { get; init; }
}

public sealed class CreateBudgetCommandValidator : AbstractValidator<CreateBudgetCommand>
{
    public CreateBudgetCommandValidator()
    {
        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Category is required.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than zero.");

        RuleFor(x => x.Month)
            .InclusiveBetween(1, 12).WithMessage("Month must be between 1 and 12.");

        RuleFor(x => x.Year)
            .InclusiveBetween(2000, 2100).WithMessage("Year must be between 2000 and 2100.");
    }
}

public sealed class CreateBudgetCommandHandler
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IValidator<CreateBudgetCommand> _validator;

    public CreateBudgetCommandHandler(
        IApplicationDbContext context,
        ICurrentUser currentUser,
        IValidator<CreateBudgetCommand> validator)
    {
        _context = context;
        _currentUser = currentUser;
        _validator = validator;
    }

    public async Task<BudgetDto> Handle(
        CreateBudgetCommand command,
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

        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == command.CategoryId && c.UserId == _currentUser.UserId, cancellationToken);

        if (category == null)
        {
            throw new InvalidOperationException("Category not found or does not belong to user.");
        }

        var budget = new Budget
        {
            UserId = _currentUser.UserId,
            CategoryId = command.CategoryId,
            Amount = command.Amount,
            Month = command.Month,
            Year = command.Year
        };

        budget.ValidateCategoryType(category.Type);

        var alreadyExists = await _context.Budgets
            .AnyAsync(b => b.UserId == _currentUser.UserId
                && b.CategoryId == command.CategoryId
                && b.Month == command.Month
                && b.Year == command.Year, cancellationToken);

        if (alreadyExists)
        {
            throw new InvalidOperationException("A budget for this category and month already exists.");
        }

        _context.Budgets.Add(budget);
        await _context.SaveChangesAsync(cancellationToken);

        return new BudgetDto
        {
            Id = budget.Id,
            UserId = budget.UserId,
            CategoryId = budget.CategoryId,
            CategoryName = category.Name,
            Amount = budget.Amount,
            Month = budget.Month,
            Year = budget.Year,
            Spent = 0,
            CreatedAtUtc = budget.CreatedAtUtc,
            UpdatedAtUtc = budget.UpdatedAtUtc
        };
    }
}
