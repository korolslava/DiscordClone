using DiscordClone.Shared.Contracts.Responses;
using MediatR;

namespace DiscordClone.Application.Features.Messages.Queries;

public record GetChannelMessagesQuery(
    Guid ChannelId,
    int Page = 1,
    int PageSize = 50) : IRequest<PagedResponse<MessageResponse>>;