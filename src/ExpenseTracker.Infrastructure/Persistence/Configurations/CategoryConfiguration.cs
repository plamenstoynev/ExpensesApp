using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework Core configuration for the Category entity.
/// </summary>
public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd();

        builder.Property(c => c.UserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(c => c.Icon)
            .HasMaxLength(50);

        builder.Property(c => c.CreatedAtUtc)
            .IsRequired();

        builder.Property(c => c.UpdatedAtUtc);

        // Indexes
        builder.HasIndex(c => c.UserId)
            .HasDatabaseName("IX_Categories_UserId");

        builder.HasIndex(c => c.Type)
            .HasDatabaseName("IX_Categories_Type");

        // Unique constraint: Category name must be unique per user and type
        builder.HasIndex(c => new { c.UserId, c.Name, c.Type })
            .IsUnique()
            .HasDatabaseName("IX_Categories_UserId_Name_Type_Unique");
    }
}
