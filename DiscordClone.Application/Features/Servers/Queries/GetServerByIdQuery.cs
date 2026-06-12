using DiscordClone.Shared.Contracts.Responses;
using MediatR;

namespace DiscordClone.Application.Features.Servers.Queries;

public record GetServerByIdQuery(Guid ServerId) : IRequest<ServerDetailResponse>;