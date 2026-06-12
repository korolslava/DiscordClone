using DiscordClone.Application.Features.Servers.Commands;
using DiscordClone.Application.Features.Servers.Queries;
using DiscordClone.Shared.Contracts.Requests;
using DiscordClone.Shared.Contracts.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscordClone.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class ServersController : ControllerBase
{
    private readonly ISender _sender;

    public ServersController(ISender sender) => _sender = sender;

    /// <summary>Get all servers the current user is a member of</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ServerResponse>>), 200)]
    public async Task<IActionResult> GetMyServers(CancellationToken ct)
    {
        var result = await _sender.Send(new GetMyServersQuery(), ct);
        return Ok(ApiResponse<IEnumerable<ServerResponse>>.Ok(result));
    }

    /// <summary>Get a server with channels and members</summary>
    [HttpGet("{serverId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ServerDetailResponse>), 200)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetServer(
        Guid serverId, CancellationToken ct)
    {
        var result = await _sender.Send(new GetServerByIdQuery(serverId), ct);
        return Ok(ApiResponse<ServerDetailResponse>.Ok(result));
    }

    /// <summary>Create a new server</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ServerResponse>), 201)]
    public async Task<IActionResult> CreateServer(
        [FromBody] CreateServerRequest request, CancellationToken ct)
    {
        var result = await _sender.Send(
            new CreateServerCommand(request.Name, request.Description), ct);

        return CreatedAtAction(nameof(GetServer),
            new { serverId = result.Id },
            ApiResponse<ServerResponse>.Ok(result, "Server created."));
    }

    /// <summary>Update server name and description</summary>
    [HttpPut("{serverId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ServerResponse>), 200)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> UpdateServer(
        Guid serverId, [FromBody] UpdateServerRequest request, CancellationToken ct)
    {
        var result = await _sender.Send(
            new UpdateServerCommand(serverId, request.Name, request.Description), ct);

        return Ok(ApiResponse<ServerResponse>.Ok(result));
    }

    /// <summary>Delete a server (owner only)</summary>
    [HttpDelete("{serverId:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> DeleteServer(
        Guid serverId, CancellationToken ct)
    {
        await _sender.Send(new DeleteServerCommand(serverId), ct);
        return NoContent();
    }

    /// <summary>Join a server by invite code</summary>
    [HttpPost("join/{inviteCode}")]
    [ProducesResponseType(typeof(ApiResponse<ServerResponse>), 200)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> JoinServer(
        string inviteCode, CancellationToken ct)
    {
        var result = await _sender.Send(new JoinServerCommand(inviteCode), ct);
        return Ok(ApiResponse<ServerResponse>.Ok(result, "Joined server."));
    }

    /// <summary>Leave a server</summary>
    [HttpPost("{serverId:guid}/leave")]
    [ProducesResponseType(204)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> LeaveServer(
        Guid serverId, CancellationToken ct)
    {
        await _sender.Send(new LeaveServerCommand(serverId), ct);
        return NoContent();
    }
}