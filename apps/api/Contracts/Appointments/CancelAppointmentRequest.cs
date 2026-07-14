using System.ComponentModel.DataAnnotations;

namespace ClinicFlow.Api.Contracts.Appointments;

public sealed class CancelAppointmentRequest
{
    [MaxLength(
        300,
        ErrorMessage = "O motivo do cancelamento deve possuir no máximo 300 caracteres."
    )]
    public string? CancellationReason { get; init; }
}