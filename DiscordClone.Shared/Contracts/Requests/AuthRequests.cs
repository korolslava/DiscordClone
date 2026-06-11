namespace DiscordClone.Shared.Contracts.Requests;

public record RegisterRequest(
    string Username,
    string DisplayName,
    string Email,
    string Password
);

public record LoginRequest(
    string Email,
    string Password
);

public record RefreshTokenRequest(
    string AccessToken,
    string RefreshToken
);