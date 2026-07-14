using ClinicFlow.Api.Models;

namespace ClinicFlow.Api.Contracts.Patients;

public sealed record PatientResponse(
    Guid Id,
    string FullName,
    string Cpf,
    DateOnly BirthDate,
    string Email,
    string? Phone,
    bool IsActive,
    DateTime CreatedAtUtc
)
{
    public static PatientResponse FromEntity(
        Patient patient
    )
    {
        return new PatientResponse(
            patient.Id,
            patient.FullName,
            FormatCpf(patient.Cpf),
            patient.BirthDate,
            patient.Email,
            patient.Phone,
            patient.IsActive,
            patient.CreatedAtUtc
        );
    }

    private static string FormatCpf(string cpf)
    {
        return Convert.ToUInt64(cpf)
            .ToString(@"000\.000\.000\-00");
    }
}