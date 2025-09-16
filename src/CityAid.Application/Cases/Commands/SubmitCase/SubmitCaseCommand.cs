using CityAid.Application.Common.Models;
using CityAid.Application.DTOs;
using MediatR;

namespace CityAid.Application.Cases.Commands.SubmitCase;

public record SubmitCaseCommand(string CaseId) : IRequest<Result<CaseDto>>;