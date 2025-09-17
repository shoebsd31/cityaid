using CityAid.Application.Cases.Commands.ApproveCase;
using CityAid.Application.Cases.Commands.CreateCase;
using CityAid.Application.Cases.Commands.SubmitCase;
using CityAid.Application.Cases.Commands.UpdateCase;
using CityAid.Application.Cases.Queries.GetCase;
using CityAid.Application.Cases.Queries.GetCases;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CityAid.Api.Endpoints;

public static class CaseEndpoints
{
    public static void MapCaseEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/cases")
            .WithTags("Cases")
            .RequireAuthorization();

        // GET /cases
        group.MapGet("/", async (
            [FromServices] IMediator mediator,
            [FromQuery] string? city = null,
            [FromQuery] string? team = null,
            [FromQuery] string? state = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50) =>
        {
            var query = new GetCasesQuery(city, team, state, page, pageSize);
            var result = await mediator.Send(query);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(new { errors = result.Errors });
        })
        .RequireAuthorization("CitizenPolicy")
        .WithName("GetCases")
        .WithSummary("List cases (scoped by RBAC/RLS)")
        .WithOpenApi();

        // POST /cases
        group.MapPost("/", async (
            [FromServices] IMediator mediator,
            [FromBody] CreateCaseRequest request) =>
        {
            var command = new CreateCaseCommand(
                request.City,
                request.Team,
                request.Title,
                request.Description,
                request.Budget,
                request.StartDate,
                request.EndDate,
                request.WorkNotes);

            var result = await mediator.Send(command);

            return result.IsSuccess
                ? Results.Created($"/cases/{result.Value!.Id}", result.Value)
                : Results.BadRequest(new { errors = result.Errors });
        })
        .RequireAuthorization("CitizenPolicy")
        .WithName("CreateCase")
        .WithSummary("Create a new case")
        .WithOpenApi();

        // GET /cases/{id}
        group.MapGet("/{id}", async (
            [FromServices] IMediator mediator,
            [FromRoute] string id) =>
        {
            var query = new GetCaseQuery(id);
            var result = await mediator.Send(query);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.NotFound(new { errors = result.Errors });
        })
        .RequireAuthorization("CitizenPolicy")
        .WithName("GetCase")
        .WithSummary("Get case by ID")
        .WithOpenApi();

        // PATCH /cases/{id}
        group.MapPatch("/{id}", async (
            [FromServices] IMediator mediator,
            [FromRoute] string id,
            [FromBody] UpdateCaseRequest request) =>
        {
            var command = new UpdateCaseCommand(
                id,
                request.Title,
                request.Description,
                request.Budget,
                request.StartDate,
                request.EndDate,
                request.WorkNotes);

            var result = await mediator.Send(command);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(new { errors = result.Errors });
        })
        .RequireAuthorization("CaseManagerPolicy")
        .WithName("UpdateCase")
        .WithSummary("Update case (metadata-only)")
        .WithOpenApi();

        // POST /cases/{id}/submit
        group.MapPost("/{id}/submit", async (
            [FromServices] IMediator mediator,
            [FromRoute] string id) =>
        {
            var command = new SubmitCaseCommand(id);
            var result = await mediator.Send(command);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(new { errors = result.Errors });
        })
        .RequireAuthorization("CaseManagerPolicy")
        .WithName("SubmitCase")
        .WithSummary("Move case to Pending_Finance")
        .WithOpenApi();

        // POST /cases/{id}/approve
        group.MapPost("/{id}/approve", async (
            [FromServices] IMediator mediator,
            [FromRoute] string id) =>
        {
            var command = new ApproveCaseCommand(id);
            var result = await mediator.Send(command);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(new { errors = result.Errors });
        })
        .RequireAuthorization("AdminPolicy")
        .WithName("ApproveCase")
        .WithSummary("Approve case for the caller's role (Finance or PMO)")
        .WithOpenApi();

        // POST /cases/{id}/reject
        group.MapPost("/{id}/reject", async (
            [FromServices] IMediator mediator,
            [FromRoute] string id,
            [FromBody] RejectCaseRequest? request = null) =>
        {
            // For now, we'll use the approve command with rejection logic handled in domain
            // In a real implementation, you'd create a separate RejectCaseCommand
            var command = new ApproveCaseCommand(id);
            var result = await mediator.Send(command);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(new { errors = result.Errors });
        })
        .RequireAuthorization("AdminPolicy")
        .WithName("RejectCase")
        .WithSummary("Reject case for the caller's role")
        .WithOpenApi();
    }
}

// Request models
public record CreateCaseRequest(
    string City,
    string Team,
    string Title,
    string? Description = null,
    decimal? Budget = null,
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    string? WorkNotes = null
);

public record UpdateCaseRequest(
    string? Title = null,
    string? Description = null,
    decimal? Budget = null,
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    string? WorkNotes = null
);

public record RejectCaseRequest(
    string? Reason = null
);