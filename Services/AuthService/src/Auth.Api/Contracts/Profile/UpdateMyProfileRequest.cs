namespace Auth.Api.Contracts.Profile;

public sealed record UpdateMyProfileRequest(
    string? FirstName,
    string? LastName,
    string? PhotoUrl,
    DateOnly? BirthDate,
    string? PhoneNumber,
    string? Country,
    string? City
);