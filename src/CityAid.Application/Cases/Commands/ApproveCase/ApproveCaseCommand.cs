using CityAid.Application.Common.Models;
using CityAid.Application.DTOs;
using MediatR;

namespace CityAid.Application.Cases.Commands.ApproveCase;

public record ApproveCaseCommand(string CaseId) : IRequest<Result<CaseDto>>;