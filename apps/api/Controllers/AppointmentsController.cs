using ClinicFlow.Api.Contracts.Appointments;
using ClinicFlow.Api.Data;
using ClinicFlow.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ClinicFlow.Api.Controllers;

[ApiController]
[Route("api/appointments")]
public sealed class AppointmentsController : ControllerBase
{
    private readonly ClinicFlowDbContext _dbContext;

    public AppointmentsController(
        ClinicFlowDbContext dbContext
    )
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyCollection<AppointmentResponse>),
        StatusCodes.Status200OK
    )]
    public async Task<
        ActionResult<IReadOnlyCollection<AppointmentResponse>>
    > GetAll(
        [FromQuery] Guid? doctorId = null,
        [FromQuery] Guid? patientId = null,
        [FromQuery] DateOnly? date = null,
        [FromQuery] AppointmentStatus? status = null,
        CancellationToken cancellationToken = default
    )
    {
        var query = _dbContext.Appointments
            .AsNoTracking()
            .Include(appointment => appointment.Doctor)
                .ThenInclude(doctor => doctor.Specialty)
            .Include(appointment => appointment.Patient)
            .AsQueryable();

        if (doctorId.HasValue)
        {
            query = query.Where(
                appointment =>
                    appointment.DoctorId == doctorId.Value
            );
        }

        if (patientId.HasValue)
        {
            query = query.Where(
                appointment =>
                    appointment.PatientId == patientId.Value
            );
        }

        if (date.HasValue)
        {
            query = query.Where(
                appointment =>
                    appointment.AppointmentDate == date.Value
            );
        }

        if (status.HasValue)
        {
            query = query.Where(
                appointment =>
                    appointment.Status == status.Value
            );
        }

        var appointments = await query
            .OrderBy(appointment =>
                appointment.AppointmentDate
            )
            .ThenBy(appointment =>
                appointment.StartTime
            )
            .ToListAsync(cancellationToken);

        var response = appointments
            .Select(AppointmentResponse.FromEntity)
            .ToList();

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(
        typeof(AppointmentResponse),
        StatusCodes.Status200OK
    )]
    [ProducesResponseType(
        StatusCodes.Status404NotFound
    )]
    public async Task<ActionResult<AppointmentResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        var appointment = await _dbContext.Appointments
            .AsNoTracking()
            .Include(item => item.Doctor)
                .ThenInclude(doctor => doctor.Specialty)
            .Include(item => item.Patient)
            .SingleOrDefaultAsync(
                item => item.Id == id,
                cancellationToken
            );

        if (appointment is null)
        {
            return AppointmentNotFound(id);
        }

        return Ok(
            AppointmentResponse.FromEntity(appointment)
        );
    }

    [HttpPost]
    [ProducesResponseType(
        typeof(AppointmentResponse),
        StatusCodes.Status201Created
    )]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest
    )]
    [ProducesResponseType(
        StatusCodes.Status404NotFound
    )]
    [ProducesResponseType(
        StatusCodes.Status409Conflict
    )]
    public async Task<ActionResult<AppointmentResponse>> Create(
        [FromBody] CreateAppointmentRequest request,
        CancellationToken cancellationToken
    )
    {
        var validationResult = ValidateRequiredIds(request);

        if (validationResult is not null)
        {
            return validationResult;
        }

        var today = DateOnly.FromDateTime(DateTime.Today);

        if (request.AppointmentDate == default)
        {
            return InvalidAppointment(
                "A data da consulta é obrigatória."
            );
        }

        if (request.AppointmentDate < today)
        {
            return InvalidAppointment(
                "Não é possível agendar uma consulta em uma data passada."
            );
        }

        var doctor = await _dbContext.Doctors
            .AsNoTracking()
            .Include(item => item.Specialty)
            .SingleOrDefaultAsync(
                item =>
                    item.Id == request.DoctorId
                    && item.IsActive,
                cancellationToken
            );

        if (doctor is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Médico ativo não encontrado.",
                detail:
                    $"Não foi encontrado um médico ativo com o ID '{request.DoctorId}'."
            );
        }

        var patient = await _dbContext.Patients
            .AsNoTracking()
            .SingleOrDefaultAsync(
                item =>
                    item.Id == request.PatientId
                    && item.IsActive,
                cancellationToken
            );

        if (patient is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Paciente ativo não encontrado.",
                detail:
                    $"Não foi encontrado um paciente ativo com o ID '{request.PatientId}'."
            );
        }

        var availability =
            await _dbContext.DoctorAvailabilities
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    item =>
                        item.Id ==
                            request.DoctorAvailabilityId
                        && item.DoctorId ==
                            request.DoctorId
                        && item.IsActive,
                    cancellationToken
                );

        if (availability is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Disponibilidade não encontrada.",
                detail:
                    "A disponibilidade informada não existe, está inativa ou pertence a outro médico."
            );
        }

        var appointmentWeekDay = ConvertToWeekDay(
            request.AppointmentDate.DayOfWeek
        );

        if (availability.DayOfWeek != appointmentWeekDay)
        {
            return InvalidAppointment(
                "A disponibilidade selecionada não corresponde ao dia da semana da consulta."
            );
        }

        var startTimeSpan =
            request.StartTime.ToTimeSpan();

        var availabilityStart =
            availability.StartTime.ToTimeSpan();

        var availabilityEnd =
            availability.EndTime.ToTimeSpan();

        var duration = TimeSpan.FromMinutes(
            availability.SlotDurationMinutes
        );

        var elapsedFromAvailabilityStart =
            startTimeSpan - availabilityStart;

        if (elapsedFromAvailabilityStart < TimeSpan.Zero)
        {
            return InvalidAppointment(
                "O horário selecionado é anterior ao início da agenda do médico."
            );
        }

        if (
            elapsedFromAvailabilityStart.Ticks
            % duration.Ticks != 0
        )
        {
            return InvalidAppointment(
                "O horário selecionado não corresponde a um horário válido da agenda."
            );
        }

        var endTimeSpan =
            startTimeSpan + duration;

        if (
            endTimeSpan > availabilityEnd
            || endTimeSpan.TotalHours >= 24
        )
        {
            return InvalidAppointment(
                "O horário selecionado ultrapassa o período disponível do médico."
            );
        }

        var endTime = TimeOnly.FromTimeSpan(
            endTimeSpan
        );

        var doctorHasConflict =
            await _dbContext.Appointments
                .AsNoTracking()
                .AnyAsync(
                    appointment =>
                        appointment.DoctorId ==
                            request.DoctorId
                        && appointment.AppointmentDate ==
                            request.AppointmentDate
                        && appointment.Status ==
                            AppointmentStatus.Scheduled
                        && appointment.StartTime < endTime
                        && request.StartTime <
                            appointment.EndTime,
                    cancellationToken
                );

        if (doctorHasConflict)
        {
            return Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Horário indisponível.",
                detail:
                    "O médico já possui uma consulta agendada nesse horário."
            );
        }

        var patientHasConflict =
            await _dbContext.Appointments
                .AsNoTracking()
                .AnyAsync(
                    appointment =>
                        appointment.PatientId ==
                            request.PatientId
                        && appointment.AppointmentDate ==
                            request.AppointmentDate
                        && appointment.Status ==
                            AppointmentStatus.Scheduled
                        && appointment.StartTime < endTime
                        && request.StartTime <
                            appointment.EndTime,
                    cancellationToken
                );

        if (patientHasConflict)
        {
            return Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Paciente já possui consulta.",
                detail:
                    "O paciente já possui uma consulta agendada nesse horário."
            );
        }

        Appointment appointment;

        try
        {
            appointment = new Appointment(
                request.DoctorId,
                request.PatientId,
                request.DoctorAvailabilityId,
                request.AppointmentDate,
                request.StartTime,
                endTime,
                request.Notes
            );
        }
        catch (ArgumentException exception)
        {
            return InvalidAppointment(
                exception.Message
            );
        }

        _dbContext.Appointments.Add(appointment);

        try
        {
            await _dbContext.SaveChangesAsync(
                cancellationToken
            );
        }
        catch (DbUpdateException exception)
            when (IsUniqueConstraintViolation(exception))
        {
            return Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Horário indisponível.",
                detail:
                    "O horário foi reservado por outra solicitação. Escolha outro horário."
            );
        }

        var response = AppointmentResponse.FromEntity(
            appointment,
            doctor,
            patient
        );

        return CreatedAtAction(
            nameof(GetById),
            new { id = appointment.Id },
            response
        );
    }

    [HttpPatch("{id:guid}/cancel")]
    [ProducesResponseType(
        typeof(AppointmentResponse),
        StatusCodes.Status200OK
    )]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest
    )]
    [ProducesResponseType(
        StatusCodes.Status404NotFound
    )]
    public async Task<ActionResult<AppointmentResponse>> Cancel(
        Guid id,
        [FromBody] CancelAppointmentRequest request,
        CancellationToken cancellationToken
    )
    {
        var appointment =
            await GetAppointmentForUpdateAsync(
                id,
                cancellationToken
            );

        if (appointment is null)
        {
            return AppointmentNotFound(id);
        }

        try
        {
            appointment.Cancel(
                request.CancellationReason
            );
        }
        catch (InvalidOperationException exception)
        {
            return InvalidAppointment(
                exception.Message
            );
        }
        catch (ArgumentException exception)
        {
            return InvalidAppointment(
                exception.Message
            );
        }

        await _dbContext.SaveChangesAsync(
            cancellationToken
        );

        return Ok(
            AppointmentResponse.FromEntity(
                appointment
            )
        );
    }

    [HttpPatch("{id:guid}/complete")]
    [ProducesResponseType(
        typeof(AppointmentResponse),
        StatusCodes.Status200OK
    )]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest
    )]
    [ProducesResponseType(
        StatusCodes.Status404NotFound
    )]
    public async Task<ActionResult<AppointmentResponse>> Complete(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        var appointment =
            await GetAppointmentForUpdateAsync(
                id,
                cancellationToken
            );

        if (appointment is null)
        {
            return AppointmentNotFound(id);
        }

        try
        {
            appointment.Complete();
        }
        catch (InvalidOperationException exception)
        {
            return InvalidAppointment(
                exception.Message
            );
        }

        await _dbContext.SaveChangesAsync(
            cancellationToken
        );

        return Ok(
            AppointmentResponse.FromEntity(
                appointment
            )
        );
    }

    [HttpPatch("{id:guid}/no-show")]
    [ProducesResponseType(
        typeof(AppointmentResponse),
        StatusCodes.Status200OK
    )]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest
    )]
    [ProducesResponseType(
        StatusCodes.Status404NotFound
    )]
    public async Task<ActionResult<AppointmentResponse>> MarkAsNoShow(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        var appointment =
            await GetAppointmentForUpdateAsync(
                id,
                cancellationToken
            );

        if (appointment is null)
        {
            return AppointmentNotFound(id);
        }

        try
        {
            appointment.MarkAsNoShow();
        }
        catch (InvalidOperationException exception)
        {
            return InvalidAppointment(
                exception.Message
            );
        }

        await _dbContext.SaveChangesAsync(
            cancellationToken
        );

        return Ok(
            AppointmentResponse.FromEntity(
                appointment
            )
        );
    }

    private async Task<Appointment?>
        GetAppointmentForUpdateAsync(
            Guid id,
            CancellationToken cancellationToken
        )
    {
        return await _dbContext.Appointments
            .Include(item => item.Doctor)
                .ThenInclude(doctor => doctor.Specialty)
            .Include(item => item.Patient)
            .SingleOrDefaultAsync(
                item => item.Id == id,
                cancellationToken
            );
    }

    private ObjectResult? ValidateRequiredIds(
        CreateAppointmentRequest request
    )
    {
        if (request.DoctorId == Guid.Empty)
        {
            return InvalidAppointment(
                "O médico é obrigatório."
            );
        }

        if (request.PatientId == Guid.Empty)
        {
            return InvalidAppointment(
                "O paciente é obrigatório."
            );
        }

        if (
            request.DoctorAvailabilityId
            == Guid.Empty
        )
        {
            return InvalidAppointment(
                "A disponibilidade do médico é obrigatória."
            );
        }

        return null;
    }

    private ObjectResult AppointmentNotFound(
        Guid id
    )
    {
        return Problem(
            statusCode:
                StatusCodes.Status404NotFound,
            title: "Consulta não encontrada.",
            detail:
                $"Não foi encontrada uma consulta com o ID '{id}'."
        );
    }

    private ObjectResult InvalidAppointment(
        string detail
    )
    {
        return Problem(
            statusCode:
                StatusCodes.Status400BadRequest,
            title: "Agendamento inválido.",
            detail: detail
        );
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

    private static bool IsUniqueConstraintViolation(
        DbUpdateException exception
    )
    {
        return exception.InnerException is PostgresException
        {
            SqlState:
                PostgresErrorCodes.UniqueViolation
        };
    }
}