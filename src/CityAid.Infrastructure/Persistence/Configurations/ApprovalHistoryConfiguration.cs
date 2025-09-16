using CityAid.Domain.Entities;
using CityAid.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CityAid.Infrastructure.Persistence.Configurations;

public class ApprovalHistoryConfiguration : IEntityTypeConfiguration<ApprovalHistory>
{
    public void Configure(EntityTypeBuilder<ApprovalHistory> builder)
    {
        builder.ToTable("ApprovalHistory");

        builder.HasKey(ah => ah.Id);

        builder.Property(ah => ah.Id)
            .IsRequired();

        // Configure CaseId value object
        builder.Property(ah => ah.CaseId)
            .HasConversion(
                id => id.Value,
                value => CaseId.FromString(value))
            .HasMaxLength(50)
            .IsRequired();

        // Configure enums
        builder.Property(ah => ah.FromState)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(ah => ah.ToState)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(ah => ah.ApprovedBy)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(ah => ah.Comments)
            .HasMaxLength(1000);

        builder.Property(ah => ah.CreatedBy)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(ah => ah.UpdatedBy)
            .HasMaxLength(100);

        // Configure indexes for querying
        builder.HasIndex(ah => ah.CaseId);
        builder.HasIndex(ah => ah.CreatedAt);
        builder.HasIndex(ah => ah.ApprovedBy);
    }
}