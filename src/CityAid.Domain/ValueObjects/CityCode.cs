namespace CityAid.Domain.ValueObjects;

public record CityCode
{
    public string Value { get; }

    private CityCode(string value)
    {
        Value = value;
    }

    public static CityCode Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("City code cannot be null or empty", nameof(value));

        if (value.Length != 3)
            throw new ArgumentException("City code must be exactly 3 characters", nameof(value));

        return new CityCode(value.ToUpperInvariant());
    }

    public static readonly CityCode Pune = Create("PUN");
    public static readonly CityCode Delhi = Create("DEL");
    public static readonly CityCode Mumbai = Create("MUM");
    public static readonly CityCode Allahabad = Create("ALL");

    public override string ToString() => Value;

    public static implicit operator string(CityCode cityCode) => cityCode.Value;
}