using CityAid.Application.Common.Models;
using CityAid.Application.DTOs;
using MediatR;

namespace CityAid.Application.Cases.Queries.GetCases;

public record GetCasesQuery(
    string? City = null,
    string? Team = null,
    string? State = null,
    int Page = 1,
    int PageSize = 50
) : IRequest<Result<PagedResult<CaseDto>>>;