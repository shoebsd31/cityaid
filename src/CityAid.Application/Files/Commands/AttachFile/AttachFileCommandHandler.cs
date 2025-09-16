using CityAid.Application.Common.Interfaces;
using CityAid.Application.Common.Models;
using CityAid.Application.DTOs;
using CityAid.Domain.Enums;
using CityAid.Domain.Repositories;
using CityAid.Domain.ValueObjects;
using MediatR;
using File = CityAid.Domain.Entities.File;

namespace CityAid.Application.Files.Commands.AttachFile;

public class AttachFileCommandHandler : IRequestHandler<AttachFileCommand, Result<FileDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public AttachFileCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result<FileDto>> Handle(AttachFileCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var caseId = CaseId.FromString(request.CaseId);
            var @case = await _unitOfWork.Cases.GetByIdAsync(caseId, cancellationToken);

            if (@case == null)
                return Result<FileDto>.Failure("Case not found");

            // Check permissions
            if (!@case.CanBeModifiedByUser(_currentUserService.CityCode!, _currentUserService.TeamType!.Value))
                return Result<FileDto>.Failure("You don't have permission to attach files to this case");

            // Parse sensitivity level
            if (!Enum.TryParse<SensitivityLevel>(request.Sensitivity, out var sensitivityLevel))
                return Result<FileDto>.Failure("Invalid sensitivity level");

            // Create file entity
            var file = new File(
                Guid.NewGuid(),
                caseId,
                request.Name,
                request.SharePointUrl,
                @case.CityCode,
                @case.TeamType,
                sensitivityLevel,
                _currentUserService.UserId);

            // Attach to case (this will raise domain events)
            @case.AttachFile(file, _currentUserService.UserId);

            await _unitOfWork.Files.AddAsync(file, cancellationToken);
            await _unitOfWork.Cases.UpdateAsync(@case, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = new FileDto
            {
                Id = file.Id.ToString(),
                CaseId = file.CaseId,
                Name = file.Name,
                SharePointUrl = file.SharePointUrl,
                City = file.CityCode,
                Team = MapTeamTypeToString(file.TeamType),
                Sensitivity = file.SensitivityLevel.ToString(),
                CreatedAt = file.CreatedAt,
                CreatedBy = file.CreatedBy
            };

            return Result<FileDto>.Success(dto);
        }
        catch (Exception ex)
        {
            return Result<FileDto>.Failure($"Failed to attach file: {ex.Message}");
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