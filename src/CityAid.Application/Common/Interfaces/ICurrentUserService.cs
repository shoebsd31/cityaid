using CityAid.Domain.Enums;
using CityAid.Domain.ValueObjects;

namespace CityAid.Application.Common.Interfaces;

public interface ICurrentUserService
{
    string UserId { get; }
    string UserName { get; }
    CityCode? CityCode { get; }
    TeamType? TeamType { get; }
    IEnumerable<string> Roles { get; }
    bool IsInRole(string role);
}