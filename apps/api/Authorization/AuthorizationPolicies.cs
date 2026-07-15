namespace ClinicFlow.Api.Authorization;

public static class AuthorizationPolicies
{
    public const string ClinicUser =
        "ClinicUser";

    public const string AdminOnly =
        "AdminOnly";

    public const string AdminOrDoctor =
        "AdminOrDoctor";
}