using CityAid.Domain.Entities;
using CityAid.Domain.Enums;
using CityAid.Domain.Repositories;
using CityAid.Domain.ValueObjects;
using CityAid.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CityAid.Infrastructure.Repositories;

public class CaseRepository : ICaseRepository
{
    private readonly CityAidDbContext _context;

    public CaseRepository(CityAidDbContext context)
    {
        _context = context;
    }

    public async Task<Case?> GetByIdAsync(CaseId id, CancellationToken cancellationToken = default)
    {
        return await _context.Cases
            .Include(c => c.Files)
            .Include(c => c.ApprovalHistory)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Case>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Cases
            .Include(c => c.Files)
            .Include(c => c.ApprovalHistory)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Case>> GetByCityAndTeamAsync(CityCode cityCode, TeamType teamType, CancellationToken cancellationToken = default)
    {
        return await _context.Cases
            .Include(c => c.Files)
            .Include(c => c.ApprovalHistory)
            .Where(c => c.CityCode == cityCode && c.TeamType == teamType)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Case>> GetByStateAsync(CaseState state, CancellationToken cancellationToken = default)
    {
        return await _context.Cases
            .Include(c => c.Files)
            .Include(c => c.ApprovalHistory)
            .Where(c => c.State == state)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IEnumerable<Case> Cases, int Total)> GetPagedAsync(
        int page,
        int pageSize,
        CityCode? cityCode = null,
        TeamType? teamType = null,
        CaseState? state = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Cases
            .Include(c => c.Files)
            .Include(c => c.ApprovalHistory)
            .AsQueryable();

        if (cityCode != null)
            query = query.Where(c => c.CityCode == cityCode);

        if (teamType.HasValue)
            query = query.Where(c => c.TeamType == teamType.Value);

        if (state.HasValue)
            query = query.Where(c => c.State == state.Value);

        var total = await query.CountAsync(cancellationToken);

        var cases = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (cases, total);
    }

    public async Task<Case> AddAsync(Case @case, CancellationToken cancellationToken = default)
    {
        _context.Cases.Add(@case);
        return @case;
    }

    public Task UpdateAsync(Case @case, CancellationToken cancellationToken = default)
    {
        _context.Cases.Update(@case);
        return Task.CompletedTask;
    }

    public async Task<int> GetNextCaseNumberAsync(int year, CityCode cityCode, TeamType teamType, CancellationToken cancellationToken = default)
    {
        var teamCode = teamType == TeamType.Alpha ? "AL" : "BE";
        var prefix = $"CS-{year}-{cityCode.Value}-{teamCode}-";

        var lastCase = await _context.Cases
            .Where(c => c.Id.Value.StartsWith(prefix))
            .OrderByDescending(c => c.Id.Value)
            .FirstOrDefaultAsync(cancellationToken);

        if (lastCase == null)
            return 1;

        // Extract the number from the case ID (e.g., "CS-2025-PUN-AL-001" -> 1)
        var lastCaseId = lastCase.Id.Value;
        var lastNumber = lastCaseId.Split('-').Last();

        if (int.TryParse(lastNumber, out var number))
            return number + 1;

        return 1;
    }
}