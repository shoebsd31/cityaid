using Microsoft.EntityFrameworkCore;

namespace CityAid.Api.Persistence;

public sealed class CityAidDbContext : DbContext
{
    public CityAidDbContext(DbContextOptions<CityAidDbContext> options) : base(options) {}
    public DbSet<CaseEntity> Cases => Set<CaseEntity>();
}

public sealed class CaseEntity
{
    public string CaseId { get; set; } = default!;
    public string CityCode { get; set; } = default!;
    public string TeamCode { get; set; } = default!;
    public string State { get; set; } = "Initiated";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
