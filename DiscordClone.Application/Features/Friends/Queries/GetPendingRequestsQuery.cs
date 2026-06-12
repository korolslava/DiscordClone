using DiscordClone.Shared.Contracts.Responses;
using MediatR;

namespace DiscordClone.Application.Features.Friends.Queries;

public record GetPendingRequestsQuery
    : IRequest<IEnumerable<FriendRequestResponse>>;