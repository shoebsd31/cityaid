using CityAid.Application.Common.Interfaces;
using CityAid.Application.Common.Models;
using CityAid.Application.DTOs;
using CityAid.Domain.Enums;
using CityAid.Domain.Repositories;
using CityAid.Domain.ValueObjects;
using MediatR;

namespace CityAid.Application.Cases.Commands.SubmitCase;

public class SubmitCaseCommandHandler : IRequestHandler<SubmitCaseCommand, Result<CaseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public SubmitCaseCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result<CaseDto>> Handle(SubmitCaseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var caseId = CaseId.FromString(request.CaseId);
            var @case = await _unitOfWork.Cases.GetByIdAsync(caseId, cancellationToken);

            if (@case == null)
                return Result<CaseDto>.Failure("Case not found");

            // Check permissions - only the owning team can submit
            if (!@case.CanBeModifiedByUser(_currentUserService.CityCode!, _currentUserService.TeamType!.Value))
                return Result<CaseDto>.Failure("You don't have permission to submit this case");

            // Submit to finance
            @case.SubmitToFinance(_currentUserService.UserId);

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
            return Result<CaseDto>.Failure($"Failed to submit case: {ex.Message}");
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