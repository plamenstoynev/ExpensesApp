using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework Core configuration for the Budget entity.
/// </summary>
public sealed class BudgetConfiguration : IEntityTypeConfiguration<Budget>
{
    public void Configure(EntityTypeBuilder<Budget> builder)
    {
        builder.ToTable("Budgets");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .ValueGeneratedOnAdd();

        builder.Property(b => b.UserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(b => b.Amount)
            .IsRequired()
            .HasColumnType("decimal(18, 2)");

        builder.Property(b => b.Month)
            .IsRequired();

        builder.Property(b => b.Year)
            .IsRequired();

        builder.Property(b => b.CreatedAtUtc)
            .IsRequired();

        builder.Property(b => b.UpdatedAtUtc);

        // Relationships
        builder.HasOne(b => b.Category)
            .WithMany(c => c.Budgets)
            .HasForeignKey(b => b.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(b => b.UserId)
            .HasDatabaseName("IX_Budgets_UserId");

        builder.HasIndex(b => b.CategoryId)
            .HasDatabaseName("IX_Budgets_CategoryId");

        // Unique constraint: Only one budget per user, category, month, and year
        builder.HasIndex(b => new { b.UserId, b.CategoryId, b.Month, b.Year })
            .IsUnique()
            .HasDatabaseName("IX_Budgets_UserId_CategoryId_Month_Year_Unique");
    }
}
