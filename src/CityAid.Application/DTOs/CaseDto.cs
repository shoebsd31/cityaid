using CityAid.Domain.Enums;

namespace CityAid.Application.DTOs;

public class CaseDto
{
    public string Id { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Team { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? Budget { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? WorkNotes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string UpdatedBy { get; set; } = string.Empty;
}