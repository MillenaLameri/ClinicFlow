namespace ClinicFlow.Api.Models;

public static class UserRoleNames
{
    public const string Admin = "Admin";

    public const string Doctor = "Doctor";

    public const string Patient = "Patient";

    public static readonly IReadOnlyCollection<string> All =
    [
        Admin,
        Doctor,
        Patient
    ];
}