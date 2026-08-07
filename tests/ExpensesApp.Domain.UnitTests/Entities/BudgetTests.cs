using ExpensesApp.Domain.Entities;
using ExpensesApp.Domain.Enums;
using FluentAssertions;

namespace ExpensesApp.Domain.UnitTests.Entities;

public sealed class BudgetTests
{
    [Fact]
    public void CreateBudget_WithValidData_ShouldSucceed()
    {
        // Arrange & Act
        var budget = new Budget
        {
            UserId = "user123",
            CategoryId = 1,
            Amount = 500m,
            Month = 8,
            Year = 2026
        };

        // Assert
        budget.Should().NotBeNull();
        budget.UserId.Should().Be("user123");
        budget.CategoryId.Should().Be(1);
        budget.Amount.Should().Be(500m);
        budget.Month.Should().Be(8);
        budget.Year.Should().Be(2026);
        budget.CreatedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void CreateBudget_WithZeroAmount_ShouldThrowException()
    {
        // Arrange & Act
        var act = () => new Budget
        {
            UserId = "user123",
            CategoryId = 1,
            Amount = 0m,
            Month = 8,
            Year = 2026
        };

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*amount must be greater than zero*");
    }

    [Fact]
    public void CreateBudget_WithNegativeAmount_ShouldThrowException()
    {
        // Arrange & Act
        var act = () => new Budget
        {
            UserId = "user123",
            CategoryId = 1,
            Amount = -100m,
            Month = 8,
            Year = 2026
        };

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*amount must be greater than zero*");
    }

    [Fact]
    public void UpdateAmount_WithValidAmount_ShouldUpdateSuccessfully()
    {
        // Arrange
        var budget = new Budget
        {
            UserId = "user123",
            CategoryId = 1,
            Amount = 500m,
            Month = 8,
            Year = 2026
        };

        // Act
        budget.UpdateAmount(750m);

        // Assert
        budget.Amount.Should().Be(750m);
        budget.UpdatedAtUtc.Should().NotBeNull();
        budget.UpdatedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdateAmount_WithZeroAmount_ShouldThrowException()
    {
        // Arrange
        var budget = new Budget
        {
            UserId = "user123",
            CategoryId = 1,
            Amount = 500m,
            Month = 8,
            Year = 2026
        };

        // Act
        var act = () => budget.UpdateAmount(0m);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*amount must be greater than zero*");
    }

    [Fact]
    public void ValidateCategoryType_WithExpenseCategory_ShouldSucceed()
    {
        // Arrange
        var budget = new Budget
        {
            UserId = "user123",
            CategoryId = 1,
            Amount = 500m,
            Month = 8,
            Year = 2026
        };

        // Act
        var act = () => budget.ValidateCategoryType(TransactionType.Expense);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void ValidateCategoryType_WithIncomeCategory_ShouldThrowException()
    {
        // Arrange
        var budget = new Budget
        {
            UserId = "user123",
            CategoryId = 1,
            Amount = 500m,
            Month = 8,
            Year = 2026
        };

        // Act
        var act = () => budget.ValidateCategoryType(TransactionType.Income);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*can only be created for expense categories*");
    }
}
