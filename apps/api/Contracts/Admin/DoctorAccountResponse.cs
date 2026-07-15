namespace ClinicFlow.Api.Contracts.Admin;

public sealed record DoctorAccountResponse(
    Guid UserId,
    Guid DoctorId,
    string FullName,
    string Email,
    string Role,
    bool IsActive
);