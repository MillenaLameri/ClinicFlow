using System.Security.Claims;
using ClinicFlow.Api.Models;

namespace ClinicFlow.Api.Services.Authorization;

public sealed class CurrentUserService
{
    private readonly IHttpContextAccessor
        _httpContextAccessor;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor
    )
    {
        _httpContextAccessor =
            httpContextAccessor;
    }

    private ClaimsPrincipal? User =>
        _httpContextAccessor
            .HttpContext?
            .User;

    public bool IsAuthenticated =>
        User?.Identity?.IsAuthenticated == true;

    public Guid? UserId =>
        GetGuidClaim(
            ClaimTypes.NameIdentifier
        );

    public Guid? PatientId =>
        GetGuidClaim("patient_id");

    public Guid? DoctorId =>
        GetGuidClaim("doctor_id");

    public bool IsAdmin =>
        User?.IsInRole(
            UserRoleNames.Admin
        ) == true;

    public bool IsDoctor =>
        User?.IsInRole(
            UserRoleNames.Doctor
        ) == true;

    public bool IsPatient =>
        User?.IsInRole(
            UserRoleNames.Patient
        ) == true;

    private Guid? GetGuidClaim(
        string claimType
    )
    {
        var value =
            User?.FindFirstValue(
                claimType
            );

        return Guid.TryParse(
            value,
            out var id
        )
            ? id
            : null;
    }
}