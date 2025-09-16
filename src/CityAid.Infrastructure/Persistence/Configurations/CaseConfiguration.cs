using CityAid.Domain.Entities;
using CityAid.Domain.Enums;
using CityAid.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CityAid.Infrastructure.Persistence.Configurations;

public class CaseConfiguration : IEntityTypeConfiguration<Case>
{
    public void Configure(EntityTypeBuilder<Case> builder)
    {
        builder.ToTable("Cases");

        builder.HasKey(c => c.Id);

        // Configure CaseId value object
        builder.Property(c => c.Id)
            .HasConversion(
                id => id.Value,
                value => CaseId.FromString(value))
            .HasMaxLength(50)
            .IsRequired();

        // Configure CityCode value object
        builder.Property(c => c.CityCode)
            .HasConversion(
                city => city.Value,
                value => CityCode.Create(value))
            .HasMaxLength(3)
            .IsRequired();

        // Configure enums
        builder.Property(c => c.TeamType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(c => c.State)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        // Configure properties
        builder.Property(c => c.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasMaxLength(2000);

        builder.Property(c => c.Budget)
            .HasPrecision(18, 2);

        builder.Property(c => c.WorkNotes)
            .HasMaxLength(4000);

        builder.Property(c => c.CreatedBy)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.UpdatedBy)
            .HasMaxLength(100);

        // Configure relationships
        builder.HasMany(c => c.Files)
            .WithOne()
            .HasForeignKey(f => f.CaseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.ApprovalHistory)
            .WithOne()
            .HasForeignKey(ah => ah.CaseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ignore domain events (they're handled separately)
        builder.Ignore(c => c.DomainEvents);

        // Configure indexes for performance and RBAC
        builder.HasIndex(c => new { c.CityCode, c.TeamType });
        builder.HasIndex(c => c.State);
        builder.HasIndex(c => c.CreatedAt);
    }
}