using CityAid.Application.Common.Interfaces;
using CityAid.Application.Common.Models;
using CityAid.Application.DTOs;
using CityAid.Domain.Repositories;
using CityAid.Domain.ValueObjects;
using MediatR;

namespace CityAid.Application.Cases.Commands.UpdateCase;

public class UpdateCaseCommandHandler : IRequestHandler<UpdateCaseCommand, Result<CaseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public UpdateCaseCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result<CaseDto>> Handle(UpdateCaseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var caseId = CaseId.FromString(request.CaseId);
            var @case = await _unitOfWork.Cases.GetByIdAsync(caseId, cancellationToken);

            if (@case == null)
                return Result<CaseDto>.Failure("Case not found");

            // Check permissions
            if (!@case.CanBeModifiedByUser(_currentUserService.CityCode!, _currentUserService.TeamType!.Value))
                return Result<CaseDto>.Failure("You don't have permission to modify this case");

            // Update metadata
            @case.UpdateMetadata(request.Title, request.Description, request.Budget,
                request.StartDate, request.EndDate, request.WorkNotes, _currentUserService.UserId);

            await _unitOfWork.Cases.UpdateAsync(@case, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Return DTO
            var dto = new CaseDto
            {
                Id = @case.Id,
                City = @case.CityCode,
                Team = @case.TeamType.ToString(),
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
            return Result<CaseDto>.Failure($"Failed to update case: {ex.Message}");
        }
    }
}