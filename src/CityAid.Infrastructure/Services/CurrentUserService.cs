using CityAid.Application.Common.Interfaces;
using CityAid.Domain.Enums;
using CityAid.Domain.ValueObjects;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CityAid.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string UserId => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";

    public string UserName => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value ?? "system";

    public CityCode? CityCode
    {
        get
        {
            var cityValue = _httpContextAccessor.HttpContext?.User?.FindFirst("city")?.Value;
            return string.IsNullOrEmpty(cityValue) ? null : CityAid.Domain.ValueObjects.CityCode.Create(cityValue);
        }
    }

    public TeamType? TeamType
    {
        get
        {
            var teamValue = _httpContextAccessor.HttpContext?.User?.FindFirst("team")?.Value;
            return string.IsNullOrEmpty(teamValue) ? null : MapStringToTeamType(teamValue);
        }
    }

    public IEnumerable<string> Roles => _httpContextAccessor.HttpContext?.User?.FindAll(ClaimTypes.Role)?.Select(c => c.Value) ?? Enumerable.Empty<string>();

    public bool IsInRole(string role)
    {
        return _httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
    }

    private static TeamType? MapStringToTeamType(string team)
    {
        return team.ToUpperInvariant() switch
        {
            "ALPHA" or "AL" => Domain.Enums.TeamType.Alpha,
            "BETA" or "BE" => Domain.Enums.TeamType.Beta,
            "FINANCE" or "FIN" => Domain.Enums.TeamType.Finance,
            "PMO" => Domain.Enums.TeamType.PMO,
            "ANALYSIS" or "AN" => Domain.Enums.TeamType.Analysis,
            _ => null
        };
    }
}