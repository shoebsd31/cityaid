using CityAid.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using File = CityAid.Domain.Entities.File;

namespace CityAid.Infrastructure.Persistence.Configurations;

public class FileConfiguration : IEntityTypeConfiguration<File>
{
    public void Configure(EntityTypeBuilder<File> builder)
    {
        builder.ToTable("Files");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.Id)
            .IsRequired();

        // Configure CaseId value object
        builder.Property(f => f.CaseId)
            .HasConversion(
                id => id.Value,
                value => CaseId.FromString(value))
            .HasMaxLength(50)
            .IsRequired();

        // Configure CityCode value object
        builder.Property(f => f.CityCode)
            .HasConversion(
                city => city.Value,
                value => CityCode.Create(value))
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(f => f.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(f => f.SharePointUrl)
            .HasMaxLength(1000)
            .IsRequired();

        // Configure enums
        builder.Property(f => f.TeamType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(f => f.SensitivityLevel)
            .HasConversion<string>()
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(f => f.CreatedBy)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(f => f.UpdatedBy)
            .HasMaxLength(100);

        // Configure indexes for performance and RBAC
        builder.HasIndex(f => f.CaseId);
        builder.HasIndex(f => new { f.CityCode, f.TeamType });
        builder.HasIndex(f => f.CreatedAt);
    }
}