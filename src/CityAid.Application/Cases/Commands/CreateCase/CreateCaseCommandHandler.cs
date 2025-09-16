using CityAid.Application.Common.Interfaces;
using CityAid.Application.Common.Models;
using CityAid.Application.DTOs;
using CityAid.Domain.Entities;
using CityAid.Domain.Enums;
using CityAid.Domain.Repositories;
using CityAid.Domain.ValueObjects;
using MediatR;

namespace CityAid.Application.Cases.Commands.CreateCase;

public class CreateCaseCommandHandler : IRequestHandler<CreateCaseCommand, Result<CaseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CreateCaseCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result<CaseDto>> Handle(CreateCaseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate user permissions
            if (_currentUserService.TeamType != TeamType.Alpha && _currentUserService.TeamType != TeamType.Beta)
            {
                return Result<CaseDto>.Failure("Only Alpha and Beta teams can create cases");
            }

            var cityCode = CityCode.Create(request.City);
            var teamType = request.Team == "AL" ? TeamType.Alpha : TeamType.Beta;

            // Verify user's city and team match
            if (_currentUserService.CityCode != cityCode || _currentUserService.TeamType != teamType)
            {
                return Result<CaseDto>.Failure("You can only create cases for your own city and team");
            }

            // Get next case number
            var currentYear = DateTime.UtcNow.Year;
            var nextCaseNumber = await _unitOfWork.Cases.GetNextCaseNumberAsync(currentYear, cityCode, teamType, cancellationToken);

            // Create case ID
            var caseId = CaseId.Create(currentYear, cityCode, teamType, nextCaseNumber);

            // Create case entity
            var @case = new Case(caseId, cityCode, teamType, request.Title, request.Description, _currentUserService.UserId);

            // Update metadata if provided
            if (request.Budget.HasValue || request.StartDate.HasValue || request.EndDate.HasValue || !string.IsNullOrEmpty(request.WorkNotes))
            {
                @case.UpdateMetadata(null, null, request.Budget, request.StartDate, request.EndDate, request.WorkNotes, _currentUserService.UserId);
            }

            // Save to repository
            await _unitOfWork.Cases.AddAsync(@case, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Return DTO
            var dto = new CaseDto
            {
                Id = @case.Id,
                City = @case.CityCode,
                Team = @case.TeamType == TeamType.Alpha ? "AL" : "BE",
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
            return Result<CaseDto>.Failure($"Failed to create case: {ex.Message}");
        }
    }
}