using DiscordClone.Application.Features.Channels.Commands;
using DiscordClone.Shared.Contracts.Requests;
using DiscordClone.Shared.Contracts.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscordClone.API.Controllers;

[Authorize]
[ApiController]
[Route("api/servers/{serverId:guid}/channels")]
public sealed class ChannelsController : ControllerBase
{
    private readonly ISender _sender;

    public ChannelsController(ISender sender) => _sender = sender;

    /// <summary>Create a new channel in a server</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ChannelResponse>), 201)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> CreateChannel(
        Guid serverId,
        [FromBody] CreateChannelRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(new CreateChannelCommand(
            serverId, request.Name, request.Type,
            request.Topic, request.IsPrivate), ct);

        return CreatedAtAction(nameof(CreateChannel),
            new { serverId, channelId = result.Id },
            ApiResponse<ChannelResponse>.Ok(result, "Channel created."));
    }

    /// <summary>Update a channel</summary>
    [HttpPut("{channelId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ChannelResponse>), 200)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> UpdateChannel(
        Guid serverId, Guid channelId,
        [FromBody] UpdateChannelRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new UpdateChannelCommand(channelId, request.Name, request.Topic), ct);

        return Ok(ApiResponse<ChannelResponse>.Ok(result));
    }

    /// <summary>Delete a channel</summary>
    [HttpDelete("{channelId:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> DeleteChannel(
        Guid serverId, Guid channelId, CancellationToken ct)
    {
        await _sender.Send(new DeleteChannelCommand(channelId), ct);
        return NoContent();
    }
}