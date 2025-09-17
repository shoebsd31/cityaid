using CityAid.Application.Files.Commands.AttachFile;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CityAid.Api.Endpoints;

public static class FileEndpoints
{
    public static void MapFileEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/cases")
            .WithTags("Files")
            .RequireAuthorization();

        // POST /cases/{id}/files
        group.MapPost("/{id}/files", async (
            [FromServices] IMediator mediator,
            [FromRoute] string id,
            [FromBody] AttachFileRequest request) =>
        {
            var command = new AttachFileCommand(
                id,
                request.Name,
                request.SharePointUrl,
                request.Sensitivity);

            var result = await mediator.Send(command);

            return result.IsSuccess
                ? Results.Created($"/cases/{id}/files/{result.Value!.Id}", result.Value)
                : Results.BadRequest(new { errors = result.Errors });
        })
        .RequireAuthorization("CitizenPolicy")
        .WithName("AttachFile")
        .WithSummary("Attach a file (metadata link to SharePoint item)")
        .WithOpenApi();

        // GET /cases/{id}/files
        group.MapGet("/{id}/files", async (
            [FromServices] IMediator mediator,
            [FromRoute] string id) =>
        {
            // For now, return empty list - in real implementation, you'd create a GetFilesQuery
            return Results.Ok(new List<object>());
        })
        .RequireAuthorization("CitizenPolicy")
        .WithName("GetFiles")
        .WithSummary("List files for case (scoped)")
        .WithOpenApi();
    }
}

// Request models
public record AttachFileRequest(
    string Name,
    string SharePointUrl,
    string Sensitivity
);