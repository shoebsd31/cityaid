using CityAid.Application.Common.Models;
using CityAid.Application.DTOs;
using MediatR;

namespace CityAid.Application.Files.Commands.AttachFile;

public record AttachFileCommand(
    string CaseId,
    string Name,
    string SharePointUrl,
    string Sensitivity
) : IRequest<Result<FileDto>>;