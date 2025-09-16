using CityAid.Domain.Entities;
using CityAid.Domain.Enums;
using CityAid.Domain.ValueObjects;

namespace CityAid.Domain.Repositories;

public interface ICaseRepository
{
    Task<Case?> GetByIdAsync(CaseId id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Case>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Case>> GetByCityAndTeamAsync(CityCode cityCode, TeamType teamType, CancellationToken cancellationToken = default);
    Task<IEnumerable<Case>> GetByStateAsync(CaseState state, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Case> Cases, int Total)> GetPagedAsync(
        int page,
        int pageSize,
        CityCode? cityCode = null,
        TeamType? teamType = null,
        CaseState? state = null,
        CancellationToken cancellationToken = default);
    Task<Case> AddAsync(Case @case, CancellationToken cancellationToken = default);
    Task UpdateAsync(Case @case, CancellationToken cancellationToken = default);
    Task<int> GetNextCaseNumberAsync(int year, CityCode cityCode, TeamType teamType, CancellationToken cancellationToken = default);
}