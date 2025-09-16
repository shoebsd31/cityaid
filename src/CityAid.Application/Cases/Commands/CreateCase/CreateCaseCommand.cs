using CityAid.Application.Common.Models;
using CityAid.Application.DTOs;
using MediatR;

namespace CityAid.Application.Cases.Commands.CreateCase;

public record CreateCaseCommand(
    string City,
    string Team,
    string Title,
    string? Description,
    decimal? Budget,
    DateTime? StartDate,
    DateTime? EndDate,
    string? WorkNotes
) : IRequest<Result<CaseDto>>;