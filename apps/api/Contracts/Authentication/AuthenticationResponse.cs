namespace ClinicFlow.Api.Contracts.Authentication;

public sealed record AuthenticatedUserResponse(
    Guid Id,
    string FullName,
    string Email,
    IReadOnlyCollection<string> Roles,
    Guid? PatientId,
    Guid? DoctorId
);

public sealed record AuthenticationResponse(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAtUtc,
    DateTime RefreshTokenExpiresAtUtc,
    AuthenticatedUserResponse User
);