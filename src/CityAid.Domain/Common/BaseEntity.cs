namespace CityAid.Domain.Common;

public abstract class BaseEntity
{
    public DateTime CreatedAt { get; protected set; }
    public DateTime UpdatedAt { get; protected set; }
    public string CreatedBy { get; protected set; } = string.Empty;
    public string UpdatedBy { get; protected set; } = string.Empty;

    protected BaseEntity()
    {
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateTimestamp(string updatedBy)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void SetCreatedBy(string createdBy)
    {
        CreatedBy = createdBy;
    }
}