using CityAid.Domain.Common;
using CityAid.Domain.Enums;
using CityAid.Domain.ValueObjects;

namespace CityAid.Domain.Entities;

public class File : BaseEntity
{
    public Guid Id { get; private set; }
    public CaseId CaseId { get; private set; }
    public string Name { get; private set; }
    public string SharePointUrl { get; private set; }
    public CityCode CityCode { get; private set; }
    public TeamType TeamType { get; private set; }
    public SensitivityLevel SensitivityLevel { get; private set; }

    // Private constructor for EF Core
    private File() { }

    public File(Guid id, CaseId caseId, string name, string sharePointUrl, CityCode cityCode, TeamType teamType, SensitivityLevel sensitivityLevel, string createdBy)
    {
        Id = id;
        CaseId = caseId ?? throw new ArgumentNullException(nameof(caseId));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        SharePointUrl = sharePointUrl ?? throw new ArgumentNullException(nameof(sharePointUrl));
        CityCode = cityCode ?? throw new ArgumentNullException(nameof(cityCode));
        TeamType = teamType;
        SensitivityLevel = sensitivityLevel;
        SetCreatedBy(createdBy);
    }

    public void UpdateMetadata(string? name, SensitivityLevel? sensitivityLevel, string updatedBy)
    {
        if (!string.IsNullOrWhiteSpace(name))
            Name = name;

        if (sensitivityLevel.HasValue)
            SensitivityLevel = sensitivityLevel.Value;

        UpdateTimestamp(updatedBy);
    }

    public bool CanBeAccessedByUser(CityCode userCity, TeamType userTeam)
    {
        // PMO can access all files
        if (userTeam == TeamType.PMO)
            return true;

        // Finance can access files from their city and both teams
        if (userTeam == TeamType.Finance)
            return CityCode == userCity;

        // Teams can only access their own files
        return CityCode == userCity && TeamType == userTeam;
    }
}