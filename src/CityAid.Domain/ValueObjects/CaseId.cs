using CityAid.Domain.Enums;

namespace CityAid.Domain.ValueObjects;

public record CaseId
{
    public string Value { get; }

    private CaseId(string value)
    {
        Value = value;
    }

    public static CaseId Create(int year, CityCode cityCode, TeamType teamType, int caseNumber)
    {
        var teamCode = teamType == TeamType.Alpha ? "AL" : "BE";
        var value = $"CS-{year}-{cityCode.Value}-{teamCode}-{caseNumber:D3}";
        return new CaseId(value);
    }

    public static CaseId FromString(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Case ID cannot be null or empty", nameof(value));

        if (!IsValidFormat(value))
            throw new ArgumentException($"Invalid case ID format: {value}", nameof(value));

        return new CaseId(value);
    }

    private static bool IsValidFormat(string value)
    {
        // CS-YYYY-XXX-XX-### format validation
        var parts = value.Split('-');
        return parts.Length == 5 &&
               parts[0] == "CS" &&
               parts[1].Length == 4 &&
               int.TryParse(parts[1], out _) &&
               parts[2].Length == 3 &&
               (parts[3] == "AL" || parts[3] == "BE") &&
               parts[4].Length == 3 &&
               int.TryParse(parts[4], out _);
    }

    public override string ToString() => Value;

    public static implicit operator string(CaseId caseId) => caseId.Value;
}