using DiscordClone.Application.Features.Media.Commands;
using DiscordClone.Shared.Contracts.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscordClone.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class MediaController : ControllerBase
{
    private readonly ISender _sender;

    public MediaController(ISender sender) => _sender = sender;

    /// <summary>Upload avatar for the current user</summary>
    [HttpPost("avatar")]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), 200)]
    [ProducesResponseType(422)]
    public async Task<IActionResult> UploadAvatar(
        IFormFile file, CancellationToken ct)
    {
        await using var stream = file.OpenReadStream();

        var result = await _sender.Send(new UploadAvatarCommand(
            stream,
            file.FileName,
            file.ContentType,
            file.Length), ct);

        return Ok(ApiResponse<UserResponse>.Ok(result, "Avatar updated."));
    }

    /// <summary>Upload icon for a server</summary>
    [HttpPost("servers/{serverId:guid}/icon")]
    [ProducesResponseType(typeof(ApiResponse<ServerResponse>), 200)]
    [ProducesResponseType(403)]
    [ProducesResponseType(422)]
    public async Task<IActionResult> UploadServerIcon(
        Guid serverId, IFormFile file, CancellationToken ct)
    {
        await using var stream = file.OpenReadStream();

        var result = await _sender.Send(new UploadServerIconCommand(
            serverId,
            stream,
            file.FileName,
            file.ContentType,
            file.Length), ct);

        return Ok(ApiResponse<ServerResponse>.Ok(result, "Server icon updated."));
    }

    /// <summary>Upload attachment for a message</summary>
    [HttpPost("messages/{messageId:guid}/attachments")]
    [ProducesResponseType(typeof(ApiResponse<AttachmentResponse>), 201)]
    [ProducesResponseType(403)]
    [ProducesResponseType(422)]
    public async Task<IActionResult> UploadAttachment(
        Guid messageId, IFormFile file, CancellationToken ct)
    {
        await using var stream = file.OpenReadStream();

        var result = await _sender.Send(new UploadMessageAttachmentCommand(
            messageId,
            stream,
            file.FileName,
            file.ContentType,
            file.Length), ct);

        return CreatedAtAction(nameof(UploadAttachment),
            ApiResponse<AttachmentResponse>.Ok(result));
    }
}