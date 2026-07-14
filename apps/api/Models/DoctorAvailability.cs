namespace ClinicFlow.Api.Models;

public sealed class DoctorAvailability
{
    public Guid Id { get; private set; }

    public Guid DoctorId { get; private set; }

    public Doctor Doctor { get; private set; } = null!;

    public WeekDay DayOfWeek { get; private set; }

    public TimeOnly StartTime { get; private set; }

    public TimeOnly EndTime { get; private set; }

    public int SlotDurationMinutes { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    private DoctorAvailability()
    {
        // Construtor utilizado pelo Entity Framework.
    }

    public DoctorAvailability(
        Guid doctorId,
        WeekDay dayOfWeek,
        TimeOnly startTime,
        TimeOnly endTime,
        int slotDurationMinutes
    )
    {
        if (doctorId == Guid.Empty)
        {
            throw new ArgumentException(
                "O médico é obrigatório.",
                nameof(doctorId)
            );
        }

        ValidateSchedule(
            dayOfWeek,
            startTime,
            endTime,
            slotDurationMinutes
        );

        Id = Guid.NewGuid();
        DoctorId = doctorId;
        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        EndTime = endTime;
        SlotDurationMinutes = slotDurationMinutes;
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void Update(
        WeekDay dayOfWeek,
        TimeOnly startTime,
        TimeOnly endTime,
        int slotDurationMinutes
    )
    {
        ValidateSchedule(
            dayOfWeek,
            startTime,
            endTime,
            slotDurationMinutes
        );

        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        EndTime = endTime;
        SlotDurationMinutes = slotDurationMinutes;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    private static void ValidateSchedule(
        WeekDay dayOfWeek,
        TimeOnly startTime,
        TimeOnly endTime,
        int slotDurationMinutes
    )
    {
        if (!Enum.IsDefined(dayOfWeek))
        {
            throw new ArgumentException(
                "O dia da semana informado é inválido.",
                nameof(dayOfWeek)
            );
        }

        if (startTime >= endTime)
        {
            throw new ArgumentException(
                "O horário inicial deve ser anterior ao horário final.",
                nameof(startTime)
            );
        }

        if (slotDurationMinutes is < 10 or > 240)
        {
            throw new ArgumentException(
                "A duração da consulta deve estar entre 10 e 240 minutos.",
                nameof(slotDurationMinutes)
            );
        }

        var availableMinutes = Convert.ToInt32(
            (endTime.ToTimeSpan() - startTime.ToTimeSpan())
            .TotalMinutes
        );

        if (slotDurationMinutes > availableMinutes)
        {
            throw new ArgumentException(
                "A duração da consulta não pode ser maior que o período disponível.",
                nameof(slotDurationMinutes)
            );
        }

        if (availableMinutes % slotDurationMinutes != 0)
        {
            throw new ArgumentException(
                "O período disponível deve ser divisível pela duração da consulta.",
                nameof(slotDurationMinutes)
            );
        }
    }
}