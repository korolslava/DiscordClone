namespace DiscordClone.Shared.Contracts.Requests;

public record CreateServerRequest(string Name, string? Description);
public record UpdateServerRequest(string Name, string? Description);