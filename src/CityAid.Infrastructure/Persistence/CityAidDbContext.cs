using Microsoft.EntityFrameworkCore;
using System.Reflection;
using File = CityAid.Domain.Entities.File;
using Case = CityAid.Domain.Entities.Case;
using ApprovalHistory = CityAid.Domain.Entities.ApprovalHistory;

namespace CityAid.Infrastructure.Persistence;

public class CityAidDbContext : DbContext
{
    public CityAidDbContext(DbContextOptions<CityAidDbContext> options) : base(options)
    {
    }

    public DbSet<Case> Cases { get; set; }
    public DbSet<File> Files { get; set; }
    public DbSet<ApprovalHistory> ApprovalHistory { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // This will be overridden by DI configuration
            optionsBuilder.UseSqlServer("Server=localhost;Database=CityAid;Trusted_Connection=true;");
        }
    }
}