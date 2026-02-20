namespace Auth.Api.Contracts.Users;

public sealed record CreateUserRequest(
    string Email,
    string Password
);