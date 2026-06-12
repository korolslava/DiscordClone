using DiscordClone.Application.Features.Messages.Commands;
using DiscordClone.Application.Features.Messages.Queries;
using DiscordClone.Shared.Contracts.Requests;
using DiscordClone.Shared.Contracts.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscordClone.API.Controllers;

[Authorize]
[ApiController]
public sealed class MessagesController : ControllerBase
{
    private readonly ISender _sender;

    public MessagesController(ISender sender) => _sender = sender;

    /// <summary>Get paginated messages for a channel</summary>
    [HttpGet("api/channels/{channelId:guid}/messages")]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<MessageResponse>>), 200)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> GetMessages(
        Guid channelId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(
            new GetChannelMessagesQuery(channelId, page, pageSize), ct);

        return Ok(ApiResponse<PagedResponse<MessageResponse>>.Ok(result));
    }

    /// <summary>Send a message to a channel</summary>
    [HttpPost("api/channels/{channelId:guid}/messages")]
    [ProducesResponseType(typeof(ApiResponse<MessageResponse>), 201)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> SendMessage(
        Guid channelId,
        [FromBody] SendMessageRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(new SendMessageCommand(
            channelId,
            request.Content,
            request.ReplyToMessageId), ct);

        return CreatedAtAction(nameof(GetMessages),
            new { channelId },
            ApiResponse<MessageResponse>.Ok(result));
    }

    /// <summary>Edit a message</summary>
    [HttpPut("api/messages/{messageId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<MessageResponse>), 200)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> EditMessage(
        Guid messageId,
        [FromBody] EditMessageRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new EditMessageCommand(messageId, request.Content), ct);

        return Ok(ApiResponse<MessageResponse>.Ok(result));
    }

    /// <summary>Soft delete a message</summary>
    [HttpDelete("api/messages/{messageId:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> DeleteMessage(
        Guid messageId, CancellationToken ct)
    {
        await _sender.Send(new DeleteMessageCommand(messageId), ct);
        return NoContent();
    }

    /// <summary>Add a reaction to a message</summary>
    [HttpPost("api/messages/{messageId:guid}/reactions/{emoji}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> AddReaction(
        Guid messageId, string emoji, CancellationToken ct)
    {
        await _sender.Send(new AddReactionCommand(messageId, emoji), ct);
        return NoContent();
    }

    /// <summary>Remove a reaction from a message</summary>
    [HttpDelete("api/messages/{messageId:guid}/reactions/{emoji}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> RemoveReaction(
        Guid messageId, string emoji, CancellationToken ct)
    {
        await _sender.Send(new RemoveReactionCommand(messageId, emoji), ct);
        return NoContent();
    }
}