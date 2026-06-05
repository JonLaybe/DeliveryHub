using System;
using System.Collections.Generic;

namespace Auth.Api.Contracts.Profile;

public sealed record UserProfileResponse(
    Guid Id,
    string? Email,
    string? FirstName,
    string? LastName,
    string? PhotoUrl,
    DateOnly? BirthDate,
    string? PhoneNumber,
    string? Country,
    string? City,
    IReadOnlyCollection<string> Roles
);