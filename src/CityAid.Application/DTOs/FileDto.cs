namespace CityAid.Application.DTOs;

public class FileDto
{
    public string Id { get; set; } = string.Empty;
    public string CaseId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string SharePointUrl { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Team { get; set; } = string.Empty;
    public string Sensitivity { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}