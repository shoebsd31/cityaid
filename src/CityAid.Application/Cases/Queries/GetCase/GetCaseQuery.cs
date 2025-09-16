using CityAid.Application.Common.Models;
using CityAid.Application.DTOs;
using MediatR;

namespace CityAid.Application.Cases.Queries.GetCase;

public record GetCaseQuery(string CaseId) : IRequest<Result<CaseDto>>;