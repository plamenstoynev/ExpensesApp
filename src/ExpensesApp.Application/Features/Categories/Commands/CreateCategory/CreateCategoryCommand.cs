using ExpensesApp.Application.Common.Interfaces;
using ExpensesApp.Application.Common.Models;
using ExpensesApp.Domain.Entities;
using ExpensesApp.Domain.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ExpensesApp.Application.Features.Categories.Commands.CreateCategory;

public sealed record CreateCategoryCommand
{
    public required string Name { get; init; }
    public required TransactionType Type { get; init; }
    public string? Icon { get; init; }
}

public sealed class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(100).WithMessage("Category name cannot exceed 100 characters.");
    }
}

public sealed class CreateCategoryCommandHandler
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IValidator<CreateCategoryCommand> _validator;

    public CreateCategoryCommandHandler(
        IApplicationDbContext context,
        ICurrentUser currentUser,
        IValidator<CreateCategoryCommand> validator)
    {
        _context = context;
        _currentUser = currentUser;
        _validator = validator;
    }

    public async Task<CategoryDto> Handle(
        CreateCategoryCommand command,
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

        var nameExists = await _context.Categories
            .AnyAsync(c => c.UserId == _currentUser.UserId
                && c.Type == command.Type
                && c.Name == command.Name.Trim(), cancellationToken);

        if (nameExists)
        {
            throw new InvalidOperationException($"A {command.Type} category named '{command.Name.Trim()}' already exists.");
        }

        var category = new Category
        {
            UserId = _currentUser.UserId,
            Name = command.Name,
            Type = command.Type,
            Icon = command.Icon
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);

        return new CategoryDto
        {
            Id = category.Id,
            UserId = category.UserId,
            Name = category.Name,
            Type = category.Type,
            Icon = category.Icon,
            CreatedAtUtc = category.CreatedAtUtc,
            UpdatedAtUtc = category.UpdatedAtUtc
        };
    }
}
