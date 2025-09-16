using CityAid.Application.Common.Interfaces;
using CityAid.Application.Common.Models;
using CityAid.Application.DTOs;
using CityAid.Domain.Enums;
using CityAid.Domain.Repositories;
using CityAid.Domain.ValueObjects;
using MediatR;

namespace CityAid.Application.Cases.Queries.GetCases;

public class GetCasesQueryHandler : IRequestHandler<GetCasesQuery, Result<PagedResult<CaseDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetCasesQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result<PagedResult<CaseDto>>> Handle(GetCasesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Apply RBAC filters based on user's role
            CityCode? cityFilter = null;
            TeamType? teamFilter = null;
            CaseState? stateFilter = null;

            // Parse optional filters
            if (!string.IsNullOrEmpty(request.City))
                cityFilter = CityCode.Create(request.City);

            if (!string.IsNullOrEmpty(request.Team))
                teamFilter = MapStringToTeamType(request.Team);

            if (!string.IsNullOrEmpty(request.State))
                Enum.TryParse<CaseState>(request.State, out var parsedState);

            // Apply RBAC: Users can only see cases they have permission to view
            if (_currentUserService.TeamType != TeamType.PMO)
            {
                // Non-PMO users are restricted to their city
                cityFilter = _currentUserService.CityCode;

                // Alpha/Beta teams can only see their own cases
                if (_currentUserService.TeamType == TeamType.Alpha || _currentUserService.TeamType == TeamType.Beta)
                {
                    teamFilter = _currentUserService.TeamType;
                }
                // Finance can see both Alpha and Beta cases in their city (no team filter)
            }

            var (cases, total) = await _unitOfWork.Cases.GetPagedAsync(
                request.Page,
                request.PageSize,
                cityFilter,
                teamFilter,
                stateFilter,
                cancellationToken);

            // Additional client-side filtering for RBAC (belt and suspenders approach)
            var filteredCases = cases.Where(c => c.CanBeViewedByUser(_currentUserService.CityCode!, _currentUserService.TeamType!.Value));

            var dtos = filteredCases.Select(c => new CaseDto
            {
                Id = c.Id,
                City = c.CityCode,
                Team = MapTeamTypeToString(c.TeamType),
                State = c.State.ToString(),
                Title = c.Title,
                Description = c.Description,
                Budget = c.Budget,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                WorkNotes = c.WorkNotes,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                CreatedBy = c.CreatedBy,
                UpdatedBy = c.UpdatedBy
            });

            var result = new PagedResult<CaseDto>(dtos, total, request.Page, request.PageSize);
            return Result<PagedResult<CaseDto>>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<PagedResult<CaseDto>>.Failure($"Failed to get cases: {ex.Message}");
        }
    }

    private static TeamType? MapStringToTeamType(string team)
    {
        return team.ToUpperInvariant() switch
        {
            "AL" => TeamType.Alpha,
            "BE" => TeamType.Beta,
            "FIN" => TeamType.Finance,
            "PMO" => TeamType.PMO,
            "AN" => TeamType.Analysis,
            _ => null
        };
    }

    private static string MapTeamTypeToString(TeamType teamType)
    {
        return teamType switch
        {
            TeamType.Alpha => "AL",
            TeamType.Beta => "BE",
            TeamType.Finance => "FIN",
            TeamType.PMO => "PMO",
            TeamType.Analysis => "AN",
            _ => teamType.ToString()
        };
    }
}