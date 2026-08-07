using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Domain.Exceptions;
using FluentAssertions;

namespace ExpenseTracker.Domain.UnitTests.Entities;

public sealed class TransactionTests
{
    [Fact]
    public void CreateTransaction_WithValidData_ShouldSucceed()
    {
        // Arrange & Act
        var transaction = new Transaction
        {
            UserId = "user123",
            Type = TransactionType.Expense,
            Amount = 100.50m,
            Currency = Currency.USD,
            CategoryId = 1,
            Description = "Grocery shopping",
            TransactionDate = DateTime.UtcNow
        };

        // Assert
        transaction.Should().NotBeNull();
        transaction.UserId.Should().Be("user123");
        transaction.Type.Should().Be(TransactionType.Expense);
        transaction.Amount.Should().Be(100.50m);
        transaction.Currency.Should().Be(Currency.USD);
        transaction.CategoryId.Should().Be(1);
        transaction.Description.Should().Be("Grocery shopping");
        transaction.CreatedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void CreateTransaction_WithZeroAmount_ShouldThrowException()
    {
        // Arrange & Act
        var act = () => new Transaction
        {
            UserId = "user123",
            Type = TransactionType.Expense,
            Amount = 0m,
            Currency = Currency.USD,
            CategoryId = 1,
            TransactionDate = DateTime.UtcNow
        };

        // Assert
        act.Should().Throw<InvalidTransactionException>()
            .WithMessage("Amount must be greater than zero.");
    }

    [Fact]
    public void CreateTransaction_WithNegativeAmount_ShouldThrowException()
    {
        // Arrange & Act
        var act = () => new Transaction
        {
            UserId = "user123",
            Type = TransactionType.Expense,
            Amount = -50m,
            Currency = Currency.USD,
            CategoryId = 1,
            TransactionDate = DateTime.UtcNow
        };

        // Assert
        act.Should().Throw<InvalidTransactionException>()
            .WithMessage("Amount must be greater than zero.");
    }

    [Fact]
    public void CreateTransaction_WithFutureDateExceedingLimit_ShouldThrowException()
    {
        // Arrange & Act
        var act = () => new Transaction
        {
            UserId = "user123",
            Type = TransactionType.Income,
            Amount = 1000m,
            Currency = Currency.EUR,
            CategoryId = 1,
            TransactionDate = DateTime.UtcNow.AddDays(400)
        };

        // Assert
        act.Should().Throw<InvalidTransactionException>()
            .WithMessage("*cannot be more than 365 days in the future*");
    }

    [Fact]
    public void CreateTransaction_WithDescriptionExceedingMaxLength_ShouldThrowException()
    {
        // Arrange
        var longDescription = new string('a', 501);

        // Act
        var act = () => new Transaction
        {
            UserId = "user123",
            Type = TransactionType.Expense,
            Amount = 50m,
            Currency = Currency.BGN,
            CategoryId = 1,
            Description = longDescription,
            TransactionDate = DateTime.UtcNow
        };

        // Assert
        act.Should().Throw<InvalidTransactionException>()
            .WithMessage("*cannot exceed 500 characters*");
    }

    [Fact]
    public void CreateTransaction_WithDescriptionContainingWhitespace_ShouldTrimIt()
    {
        // Arrange & Act
        var transaction = new Transaction
        {
            UserId = "user123",
            Type = TransactionType.Expense,
            Amount = 25m,
            Currency = Currency.USD,
            CategoryId = 1,
            Description = "  Coffee   ",
            TransactionDate = DateTime.UtcNow
        };

        // Assert
        transaction.Description.Should().Be("Coffee");
    }

    [Fact]
    public void UpdateAmount_WithValidAmount_ShouldUpdateSuccessfully()
    {
        // Arrange
        var transaction = new Transaction
        {
            UserId = "user123",
            Type = TransactionType.Expense,
            Amount = 100m,
            Currency = Currency.USD,
            CategoryId = 1,
            TransactionDate = DateTime.UtcNow
        };

        // Act
        transaction.UpdateAmount(200m);

        // Assert
        transaction.Amount.Should().Be(200m);
        transaction.UpdatedAtUtc.Should().NotBeNull();
        transaction.UpdatedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdateAmount_WithZeroAmount_ShouldThrowException()
    {
        // Arrange
        var transaction = new Transaction
        {
            UserId = "user123",
            Type = TransactionType.Expense,
            Amount = 100m,
            Currency = Currency.USD,
            CategoryId = 1,
            TransactionDate = DateTime.UtcNow
        };

        // Act
        var act = () => transaction.UpdateAmount(0m);

        // Assert
        act.Should().Throw<InvalidTransactionException>()
            .WithMessage("Amount must be greater than zero.");
    }

    [Fact]
    public void UpdateDescription_WithValidDescription_ShouldUpdateSuccessfully()
    {
        // Arrange
        var transaction = new Transaction
        {
            UserId = "user123",
            Type = TransactionType.Expense,
            Amount = 50m,
            Currency = Currency.USD,
            CategoryId = 1,
            Description = "Original",
            TransactionDate = DateTime.UtcNow
        };

        // Act
        transaction.UpdateDescription("Updated description");

        // Assert
        transaction.Description.Should().Be("Updated description");
        transaction.UpdatedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public void UpdateDescription_WithTooLongDescription_ShouldThrowException()
    {
        // Arrange
        var transaction = new Transaction
        {
            UserId = "user123",
            Type = TransactionType.Expense,
            Amount = 50m,
            Currency = Currency.USD,
            CategoryId = 1,
            TransactionDate = DateTime.UtcNow
        };

        var longDescription = new string('x', 501);

        // Act
        var act = () => transaction.UpdateDescription(longDescription);

        // Assert
        act.Should().Throw<InvalidTransactionException>();
    }

    [Fact]
    public void UpdateTransactionDate_WithValidDate_ShouldUpdateSuccessfully()
    {
        // Arrange
        var transaction = new Transaction
        {
            UserId = "user123",
            Type = TransactionType.Income,
            Amount = 1000m,
            Currency = Currency.EUR,
            CategoryId = 1,
            TransactionDate = DateTime.UtcNow.AddDays(-10)
        };

        var newDate = DateTime.UtcNow.AddDays(-5);

        // Act
        transaction.UpdateTransactionDate(newDate);

        // Assert
        transaction.TransactionDate.Should().BeCloseTo(newDate, TimeSpan.FromSeconds(1));
        transaction.UpdatedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public void UpdateTransactionDate_WithFarFutureDate_ShouldThrowException()
    {
        // Arrange
        var transaction = new Transaction
        {
            UserId = "user123",
            Type = TransactionType.Income,
            Amount = 1000m,
            Currency = Currency.EUR,
            CategoryId = 1,
            TransactionDate = DateTime.UtcNow
        };

        // Act
        var act = () => transaction.UpdateTransactionDate(DateTime.UtcNow.AddDays(500));

        // Assert
        act.Should().Throw<InvalidTransactionException>();
    }
}
