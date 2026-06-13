using DiscordClone.Shared.Contracts.Responses;
using MediatR;

namespace DiscordClone.Application.Features.Media.Commands;

public record UploadServerIconCommand(
    Guid ServerId,
    Stream FileStream,
    string FileName,
    string ContentType,
    long FileSize) : IRequest<ServerResponse>;