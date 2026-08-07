using ExpensesApp.Domain.Entities;
using ExpensesApp.Domain.Enums;
using FluentAssertions;

namespace ExpensesApp.Domain.UnitTests.Entities;

public sealed class CategoryTests
{
    [Fact]
    public void CreateCategory_WithValidData_ShouldSucceed()
    {
        // Arrange & Act
        var category = new Category
        {
            UserId = "user123",
            Name = "Food",
            Type = TransactionType.Expense,
            Icon = "🍔"
        };

        // Assert
        category.Should().NotBeNull();
        category.UserId.Should().Be("user123");
        category.Name.Should().Be("Food");
        category.Type.Should().Be(TransactionType.Expense);
        category.Icon.Should().Be("🍔");
        category.CreatedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void CreateCategory_WithEmptyName_ShouldThrowException()
    {
        // Arrange & Act
        var act = () => new Category
        {
            UserId = "user123",
            Name = "",
            Type = TransactionType.Income
        };

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*name cannot be empty*");
    }

    [Fact]
    public void CreateCategory_WithWhitespaceOnlyName_ShouldThrowException()
    {
        // Arrange & Act
        var act = () => new Category
        {
            UserId = "user123",
            Name = "   ",
            Type = TransactionType.Income
        };

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*name cannot be empty*");
    }

    [Fact]
    public void CreateCategory_WithNameExceedingMaxLength_ShouldThrowException()
    {
        // Arrange
        var longName = new string('a', 101);

        // Act
        var act = () => new Category
        {
            UserId = "user123",
            Name = longName,
            Type = TransactionType.Expense
        };

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*cannot exceed 100 characters*");
    }

    [Fact]
    public void CreateCategory_WithNameContainingWhitespace_ShouldTrimIt()
    {
        // Arrange & Act
        var category = new Category
        {
            UserId = "user123",
            Name = "  Transport  ",
            Type = TransactionType.Expense
        };

        // Assert
        category.Name.Should().Be("Transport");
    }

    [Fact]
    public void CreateCategory_WithIncomeType_ShouldSucceed()
    {
        // Arrange & Act
        var category = new Category
        {
            UserId = "user123",
            Name = "Salary",
            Type = TransactionType.Income
        };

        // Assert
        category.Type.Should().Be(TransactionType.Income);
    }

    [Fact]
    public void CreateCategory_WithExpenseType_ShouldSucceed()
    {
        // Arrange & Act
        var category = new Category
        {
            UserId = "user123",
            Name = "Bills",
            Type = TransactionType.Expense
        };

        // Assert
        category.Type.Should().Be(TransactionType.Expense);
    }
}
