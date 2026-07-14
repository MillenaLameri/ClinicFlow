using System.ComponentModel.DataAnnotations;

namespace ClinicFlow.Api.Contracts.Appointments;

public sealed class CreateAppointmentRequest
{
    public Guid DoctorId { get; init; }

    public Guid PatientId { get; init; }

    public Guid DoctorAvailabilityId { get; init; }

    public DateOnly AppointmentDate { get; init; }

    public TimeOnly StartTime { get; init; }

    [MaxLength(
        500,
        ErrorMessage = "As observações devem possuir no máximo 500 caracteres."
    )]
    public string? Notes { get; init; }
}