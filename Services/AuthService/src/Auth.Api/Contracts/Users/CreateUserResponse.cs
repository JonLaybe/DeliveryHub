using System;

namespace Auth.Api.Contracts.Users;

public sealed record CreateUserResponse(
    Guid Id,
    string Email
);