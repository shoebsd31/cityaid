using CityAid.Application.Common.Models;
using CityAid.Application.DTOs;
using MediatR;

namespace CityAid.Application.Cases.Commands.UpdateCase;

public record UpdateCaseCommand(
    string CaseId,
    string? Title,
    string? Description,
    decimal? Budget,
    DateTime? StartDate,
    DateTime? EndDate,
    string? WorkNotes
) : IRequest<Result<CaseDto>>;