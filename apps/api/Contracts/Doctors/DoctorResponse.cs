using ClinicFlow.Api.Models;

namespace ClinicFlow.Api.Contracts.Doctors;

public sealed record DoctorSpecialtyResponse(
    Guid Id,
    string Name
);

public sealed record DoctorResponse(
    Guid Id,
    string FullName,
    string CrmNumber,
    string CrmState,
    string Email,
    string? Phone,
    DoctorSpecialtyResponse Specialty,
    bool IsActive,
    DateTime CreatedAtUtc
)
{
    public static DoctorResponse FromEntity(Doctor doctor)
    {
        return FromEntity(doctor, doctor.Specialty);
    }

    public static DoctorResponse FromEntity(
        Doctor doctor,
        Specialty specialty
    )
    {
        return new DoctorResponse(
            doctor.Id,
            doctor.FullName,
            doctor.CrmNumber,
            doctor.CrmState,
            doctor.Email,
            doctor.Phone,
            new DoctorSpecialtyResponse(
                specialty.Id,
                specialty.Name
            ),
            doctor.IsActive,
            doctor.CreatedAtUtc
        );
    }
}