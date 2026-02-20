namespace Auth.Api.Contracts.Users;

public sealed record LockUserRequest(int? Minutes); // null => lock indefinitely