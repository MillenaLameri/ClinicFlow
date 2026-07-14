using System.ComponentModel.DataAnnotations;
using ClinicFlow.Api.Models;

namespace ClinicFlow.Api.Contracts.DoctorAvailabilities;

public sealed class UpdateDoctorAvailabilityRequest
{
    [EnumDataType(
        typeof(WeekDay),
        ErrorMessage = "O dia da semana informado é inválido."
    )]
    public WeekDay DayOfWeek { get; init; }

    public TimeOnly StartTime { get; init; }

    public TimeOnly EndTime { get; init; }

    [Range(
        10,
        240,
        ErrorMessage = "A duração deve estar entre 10 e 240 minutos."
    )]
    public int SlotDurationMinutes { get; init; }
}