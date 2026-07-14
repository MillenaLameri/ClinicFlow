using ClinicFlow.Api.Data;
using ClinicFlow.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Api.Services.Scheduling;

public sealed record GeneratedAvailableSlot(
    Guid AvailabilityId,
    DateOnly Date,
    WeekDay DayOfWeek,
    TimeOnly StartTime,
    TimeOnly EndTime,
    int DurationMinutes
);

public sealed class AvailableSlotsService
{
    private readonly ClinicFlowDbContext _dbContext;

    public AvailableSlotsService(
        ClinicFlowDbContext dbContext
    )
    {
        _dbContext = dbContext;
    }

    public async Task<
        IReadOnlyCollection<GeneratedAvailableSlot>
    > GenerateAsync(
        Guid doctorId,
        DateOnly date,
        CancellationToken cancellationToken
    )
    {
        var weekDay = ConvertToWeekDay(
            date.DayOfWeek
        );

        var availabilities =
            await _dbContext.DoctorAvailabilities
                .AsNoTracking()
                .Where(availability =>
                    availability.DoctorId == doctorId
                    && availability.DayOfWeek == weekDay
                    && availability.IsActive
                )
                .OrderBy(availability =>
                    availability.StartTime
                )
                .ToListAsync(cancellationToken);

        var scheduledAppointments =
            await _dbContext.Appointments
                .AsNoTracking()
                .Where(appointment =>
                    appointment.DoctorId == doctorId
                    && appointment.AppointmentDate == date
                    && appointment.Status ==
                        AppointmentStatus.Scheduled
                )
                .Select(appointment => new
                {
                    appointment.StartTime,
                    appointment.EndTime
                })
                .ToListAsync(cancellationToken);

        var generatedSlots =
            new List<GeneratedAvailableSlot>();

        foreach (var availability in availabilities)
        {
            var currentStart =
                availability.StartTime;

            while (true)
            {
                var currentEnd =
                    currentStart.AddMinutes(
                        availability.SlotDurationMinutes
                    );

                if (currentEnd > availability.EndTime)
                {
                    break;
                }

                var isOccupied =
                    scheduledAppointments.Any(
                        appointment =>
                            appointment.StartTime
                                < currentEnd
                            && currentStart
                                < appointment.EndTime
                    );

                if (!isOccupied)
                {
                    generatedSlots.Add(
                        new GeneratedAvailableSlot(
                            availability.Id,
                            date,
                            weekDay,
                            currentStart,
                            currentEnd,
                            availability
                                .SlotDurationMinutes
                        )
                    );
                }

                currentStart = currentEnd;
            }
        }

        return generatedSlots;
    }

    private static WeekDay ConvertToWeekDay(
        System.DayOfWeek dayOfWeek
    )
    {
        return dayOfWeek switch
        {
            System.DayOfWeek.Monday =>
                WeekDay.Monday,

            System.DayOfWeek.Tuesday =>
                WeekDay.Tuesday,

            System.DayOfWeek.Wednesday =>
                WeekDay.Wednesday,

            System.DayOfWeek.Thursday =>
                WeekDay.Thursday,

            System.DayOfWeek.Friday =>
                WeekDay.Friday,

            System.DayOfWeek.Saturday =>
                WeekDay.Saturday,

            System.DayOfWeek.Sunday =>
                WeekDay.Sunday,

            _ => throw new ArgumentOutOfRangeException(
                nameof(dayOfWeek),
                dayOfWeek,
                "Dia da semana inválido."
            )
        };
    }
}