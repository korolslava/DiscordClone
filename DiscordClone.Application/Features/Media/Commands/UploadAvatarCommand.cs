using DiscordClone.Shared.Contracts.Responses;
using MediatR;

namespace DiscordClone.Application.Features.Media.Commands;

public record UploadAvatarCommand(
    Stream FileStream,
    string FileName,
    string ContentType,
    long FileSize) : IRequest<UserResponse>;