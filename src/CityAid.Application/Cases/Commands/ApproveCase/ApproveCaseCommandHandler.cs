using CityAid.Application.Common.Interfaces;
using CityAid.Application.Common.Models;
using CityAid.Application.DTOs;
using CityAid.Domain.Enums;
using CityAid.Domain.Repositories;
using CityAid.Domain.ValueObjects;
using MediatR;

namespace CityAid.Application.Cases.Commands.ApproveCase;

public class ApproveCaseCommandHandler : IRequestHandler<ApproveCaseCommand, Result<CaseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public ApproveCaseCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result<CaseDto>> Handle(ApproveCaseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var caseId = CaseId.FromString(request.CaseId);
            var @case = await _unitOfWork.Cases.GetByIdAsync(caseId, cancellationToken);

            if (@case == null)
                return Result<CaseDto>.Failure("Case not found");

            // Check permissions and determine action based on role
            switch (_currentUserService.TeamType)
            {
                case TeamType.Finance:
                    if (@case.CityCode != _currentUserService.CityCode)
                        return Result<CaseDto>.Failure("Finance users can only approve cases in their city");
                    @case.ApproveByFinance(_currentUserService.UserId);
                    break;

                case TeamType.PMO:
                    @case.ApproveByPMO(_currentUserService.UserId);
                    break;

                default:
                    return Result<CaseDto>.Failure("Only Finance and PMO users can approve cases");
            }

            await _unitOfWork.Cases.UpdateAsync(@case, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = new CaseDto
            {
                Id = @case.Id,
                City = @case.CityCode,
                Team = MapTeamTypeToString(@case.TeamType),
                State = @case.State.ToString(),
                Title = @case.Title,
                Description = @case.Description,
                Budget = @case.Budget,
                StartDate = @case.StartDate,
                EndDate = @case.EndDate,
                WorkNotes = @case.WorkNotes,
                CreatedAt = @case.CreatedAt,
                UpdatedAt = @case.UpdatedAt,
                CreatedBy = @case.CreatedBy,
                UpdatedBy = @case.UpdatedBy
            };

            return Result<CaseDto>.Success(dto);
        }
        catch (Exception ex)
        {
            return Result<CaseDto>.Failure($"Failed to approve case: {ex.Message}");
        }
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