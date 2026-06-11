using DiscordClone.Shared.Contracts.Responses;
using MediatR;

namespace DiscordClone.Application.Features.Auth.Queries;

public record GetCurrentUserQuery : IRequest<UserResponse>;