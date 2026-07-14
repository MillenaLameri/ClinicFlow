using ClinicFlow.Api.Models;

namespace ClinicFlow.Api.Contracts.Appointments;

public sealed record AppointmentSpecialtyResponse(
    Guid Id,
    string Name
);

public sealed record AppointmentDoctorResponse(
    Guid Id,
    string FullName,
    string CrmNumber,
    string CrmState,
    AppointmentSpecialtyResponse Specialty
);

public sealed record AppointmentPatientResponse(
    Guid Id,
    string FullName,
    string Cpf,
    string Email
);

public sealed record AppointmentResponse(
    Guid Id,
    AppointmentDoctorResponse Doctor,
    AppointmentPatientResponse Patient,
    Guid DoctorAvailabilityId,
    DateOnly AppointmentDate,
    TimeOnly StartTime,
    TimeOnly EndTime,
    AppointmentStatus Status,
    string? Notes,
    string? CancellationReason,
    DateTime CreatedAtUtc,
    DateTime? CancelledAtUtc,
    DateTime? CompletedAtUtc
)
{
    public static AppointmentResponse FromEntity(
        Appointment appointment
    )
    {
        return FromEntity(
            appointment,
            appointment.Doctor,
            appointment.Patient
        );
    }

    public static AppointmentResponse FromEntity(
        Appointment appointment,
        Doctor doctor,
        Patient patient
    )
    {
        return new AppointmentResponse(
            appointment.Id,
            new AppointmentDoctorResponse(
                doctor.Id,
                doctor.FullName,
                doctor.CrmNumber,
                doctor.CrmState,
                new AppointmentSpecialtyResponse(
                    doctor.Specialty.Id,
                    doctor.Specialty.Name
                )
            ),
            new AppointmentPatientResponse(
                patient.Id,
                patient.FullName,
                FormatCpf(patient.Cpf),
                patient.Email
            ),
            appointment.DoctorAvailabilityId,
            appointment.AppointmentDate,
            appointment.StartTime,
            appointment.EndTime,
            appointment.Status,
            appointment.Notes,
            appointment.CancellationReason,
            appointment.CreatedAtUtc,
            appointment.CancelledAtUtc,
            appointment.CompletedAtUtc
        );
    }

    private static string FormatCpf(string cpf)
    {
        if (cpf.Length != 11)
        {
            return cpf;
        }

        return
            $"{cpf[..3]}.{cpf.Substring(3, 3)}." +
            $"{cpf.Substring(6, 3)}-{cpf.Substring(9, 2)}";
    }
}