using DiscordClone.Application.Features.Friends.Commands;
using DiscordClone.Application.Features.Friends.Queries;
using DiscordClone.Shared.Contracts.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscordClone.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class FriendsController : ControllerBase
{
    private readonly ISender _sender;

    public FriendsController(ISender sender) => _sender = sender;

    /// <summary>Get all friends with online status</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<FriendResponse>>), 200)]
    public async Task<IActionResult> GetFriends(CancellationToken ct)
    {
        var result = await _sender.Send(new GetFriendsQuery(), ct);
        return Ok(ApiResponse<IEnumerable<FriendResponse>>.Ok(result));
    }

    /// <summary>Get all incoming pending friend requests</summary>
    [HttpGet("requests/pending")]
    [ProducesResponseType(
        typeof(ApiResponse<IEnumerable<FriendRequestResponse>>), 200)]
    public async Task<IActionResult> GetPendingRequests(CancellationToken ct)
    {
        var result = await _sender.Send(new GetPendingRequestsQuery(), ct);
        return Ok(ApiResponse<IEnumerable<FriendRequestResponse>>.Ok(result));
    }

    /// <summary>Send a friend request by username</summary>
    [HttpPost("requests/{username}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> SendFriendRequest(
        string username, CancellationToken ct)
    {
        await _sender.Send(new SendFriendRequestCommand(username), ct);
        return NoContent();
    }

    /// <summary>Accept or decline a friend request</summary>
    [HttpPut("requests/{requestId:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> RespondToRequest(
        Guid requestId,
        [FromQuery] bool accept,
        CancellationToken ct)
    {
        await _sender.Send(
            new RespondFriendRequestCommand(requestId, accept), ct);
        return NoContent();
    }

    /// <summary>Remove a friend</summary>
    [HttpDelete("{friendUserId:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> RemoveFriend(
        Guid friendUserId, CancellationToken ct)
    {
        await _sender.Send(new RemoveFriendCommand(friendUserId), ct);
        return NoContent();
    }
}