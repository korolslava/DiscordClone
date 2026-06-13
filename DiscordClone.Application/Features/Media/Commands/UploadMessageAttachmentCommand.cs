using DiscordClone.Shared.Contracts.Responses;
using MediatR;

namespace DiscordClone.Application.Features.Media.Commands;

public record UploadMessageAttachmentCommand(
    Guid MessageId,
    Stream FileStream,
    string FileName,
    string ContentType,
    long FileSize) : IRequest<AttachmentResponse>;