using ClinicFlow.Api.Models;

namespace ClinicFlow.Api.Contracts.AvailableSlots;

public sealed record AvailableSlotResponse(
    Guid AvailabilityId,
    DateOnly Date,
    WeekDay DayOfWeek,
    TimeOnly StartTime,
    TimeOnly EndTime,
    int DurationMinutes
);