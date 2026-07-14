using ClinicFlow.Api.Models;

namespace ClinicFlow.Api.Contracts.DoctorAvailabilities;

public sealed record DoctorAvailabilityResponse(
    Guid Id,
    Guid DoctorId,
    WeekDay DayOfWeek,
    TimeOnly StartTime,
    TimeOnly EndTime,
    int SlotDurationMinutes,
    bool IsActive,
    DateTime CreatedAtUtc
)
{
    public static DoctorAvailabilityResponse FromEntity(
        DoctorAvailability availability
    )
    {
        return new DoctorAvailabilityResponse(
            availability.Id,
            availability.DoctorId,
            availability.DayOfWeek,
            availability.StartTime,
            availability.EndTime,
            availability.SlotDurationMinutes,
            availability.IsActive,
            availability.CreatedAtUtc
        );
    }
}