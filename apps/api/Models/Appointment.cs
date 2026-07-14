namespace ClinicFlow.Api.Models;

public sealed class Appointment
{
    public Guid Id { get; private set; }

    public Guid DoctorId { get; private set; }

    public Doctor Doctor { get; private set; } = null!;

    public Guid PatientId { get; private set; }

    public Patient Patient { get; private set; } = null!;

    public Guid DoctorAvailabilityId { get; private set; }

    public DoctorAvailability DoctorAvailability { get; private set; }
        = null!;

    public DateOnly AppointmentDate { get; private set; }

    public TimeOnly StartTime { get; private set; }

    public TimeOnly EndTime { get; private set; }

    public AppointmentStatus Status { get; private set; }

    public string? Notes { get; private set; }

    public string? CancellationReason { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? CancelledAtUtc { get; private set; }

    public DateTime? CompletedAtUtc { get; private set; }

    private Appointment()
    {
        // Construtor utilizado pelo Entity Framework.
    }

    public Appointment(
        Guid doctorId,
        Guid patientId,
        Guid doctorAvailabilityId,
        DateOnly appointmentDate,
        TimeOnly startTime,
        TimeOnly endTime,
        string? notes
    )
    {
        if (doctorId == Guid.Empty)
        {
            throw new ArgumentException(
                "O médico é obrigatório.",
                nameof(doctorId)
            );
        }

        if (patientId == Guid.Empty)
        {
            throw new ArgumentException(
                "O paciente é obrigatório.",
                nameof(patientId)
            );
        }

        if (doctorAvailabilityId == Guid.Empty)
        {
            throw new ArgumentException(
                "A disponibilidade do médico é obrigatória.",
                nameof(doctorAvailabilityId)
            );
        }

        if (appointmentDate == default)
        {
            throw new ArgumentException(
                "A data da consulta é obrigatória.",
                nameof(appointmentDate)
            );
        }

        if (startTime >= endTime)
        {
            throw new ArgumentException(
                "O horário inicial deve ser anterior ao horário final.",
                nameof(startTime)
            );
        }

        Id = Guid.NewGuid();
        DoctorId = doctorId;
        PatientId = patientId;
        DoctorAvailabilityId = doctorAvailabilityId;
        AppointmentDate = appointmentDate;
        StartTime = startTime;
        EndTime = endTime;
        Notes = NormalizeOptionalText(
            notes,
            500,
            "As observações devem possuir no máximo 500 caracteres."
        );
        Status = AppointmentStatus.Scheduled;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void Cancel(string? cancellationReason)
    {
        if (Status != AppointmentStatus.Scheduled)
        {
            throw new InvalidOperationException(
                "Somente consultas agendadas podem ser canceladas."
            );
        }

        CancellationReason = NormalizeOptionalText(
            cancellationReason,
            300,
            "O motivo do cancelamento deve possuir no máximo 300 caracteres."
        );

        Status = AppointmentStatus.Cancelled;
        CancelledAtUtc = DateTime.UtcNow;
    }

    public void Complete()
    {
        if (Status != AppointmentStatus.Scheduled)
        {
            throw new InvalidOperationException(
                "Somente consultas agendadas podem ser concluídas."
            );
        }

        Status = AppointmentStatus.Completed;
        CompletedAtUtc = DateTime.UtcNow;
    }

    public void MarkAsNoShow()
    {
        if (Status != AppointmentStatus.Scheduled)
        {
            throw new InvalidOperationException(
                "Somente consultas agendadas podem ser marcadas como ausência."
            );
        }

        Status = AppointmentStatus.NoShow;
    }

    private static string? NormalizeOptionalText(
        string? value,
        int maximumLength,
        string errorMessage
    )
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalizedValue = value.Trim();

        if (normalizedValue.Length > maximumLength)
        {
            throw new ArgumentException(errorMessage);
        }

        return normalizedValue;
    }
}