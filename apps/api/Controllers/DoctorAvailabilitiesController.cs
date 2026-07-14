using ClinicFlow.Api.Contracts.DoctorAvailabilities;
using ClinicFlow.Api.Data;
using ClinicFlow.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ClinicFlow.Api.Controllers;

[ApiController]
[Route("api/doctors/{doctorId:guid}/availabilities")]
public sealed class DoctorAvailabilitiesController : ControllerBase
{
    private readonly ClinicFlowDbContext _dbContext;

    public DoctorAvailabilitiesController(
        ClinicFlowDbContext dbContext
    )
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyCollection<DoctorAvailabilityResponse>),
        StatusCodes.Status200OK
    )]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<
        ActionResult<IReadOnlyCollection<DoctorAvailabilityResponse>>
    > GetAll(
        Guid doctorId,
        [FromQuery] bool includeInactive = false,
        CancellationToken cancellationToken = default
    )
    {
        var doctorExists = await _dbContext.Doctors
            .AsNoTracking()
            .AnyAsync(
                doctor => doctor.Id == doctorId,
                cancellationToken
            );

        if (!doctorExists)
        {
            return DoctorNotFound(doctorId);
        }

        var query = _dbContext.DoctorAvailabilities
            .AsNoTracking()
            .Where(
                availability =>
                    availability.DoctorId == doctorId
            );

        if (!includeInactive)
        {
            query = query.Where(
                availability => availability.IsActive
            );
        }

        var availabilities = await query
            .OrderBy(availability => availability.DayOfWeek)
            .ThenBy(availability => availability.StartTime)
            .Select(availability =>
                new DoctorAvailabilityResponse(
                    availability.Id,
                    availability.DoctorId,
                    availability.DayOfWeek,
                    availability.StartTime,
                    availability.EndTime,
                    availability.SlotDurationMinutes,
                    availability.IsActive,
                    availability.CreatedAtUtc
                )
            )
            .ToListAsync(cancellationToken);

        return Ok(availabilities);
    }

    [HttpGet("{availabilityId:guid}")]
    [ProducesResponseType(
        typeof(DoctorAvailabilityResponse),
        StatusCodes.Status200OK
    )]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<
        ActionResult<DoctorAvailabilityResponse>
    > GetById(
        Guid doctorId,
        Guid availabilityId,
        CancellationToken cancellationToken
    )
    {
        var availability =
            await _dbContext.DoctorAvailabilities
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    item =>
                        item.Id == availabilityId
                        && item.DoctorId == doctorId,
                    cancellationToken
                );

        if (availability is null)
        {
            return AvailabilityNotFound(
                doctorId,
                availabilityId
            );
        }

        return Ok(
            DoctorAvailabilityResponse.FromEntity(
                availability
            )
        );
    }

    [HttpPost]
    [ProducesResponseType(
        typeof(DoctorAvailabilityResponse),
        StatusCodes.Status201Created
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<
        ActionResult<DoctorAvailabilityResponse>
    > Create(
        Guid doctorId,
        [FromBody] CreateDoctorAvailabilityRequest request,
        CancellationToken cancellationToken
    )
    {
        var doctorExists = await _dbContext.Doctors
            .AsNoTracking()
            .AnyAsync(
                doctor =>
                    doctor.Id == doctorId
                    && doctor.IsActive,
                cancellationToken
            );

        if (!doctorExists)
        {
            return ActiveDoctorNotFound(doctorId);
        }

        DoctorAvailability availability;

        try
        {
            availability = new DoctorAvailability(
                doctorId,
                request.DayOfWeek,
                request.StartTime,
                request.EndTime,
                request.SlotDurationMinutes
            );
        }
        catch (ArgumentException exception)
        {
            return InvalidAvailability(exception.Message);
        }

        var hasOverlap = await HasOverlappingAvailabilityAsync(
            doctorId,
            request.DayOfWeek,
            request.StartTime,
            request.EndTime,
            ignoredAvailabilityId: null,
            cancellationToken
        );

        if (hasOverlap)
        {
            return AvailabilityOverlapConflict(
                request.DayOfWeek,
                request.StartTime,
                request.EndTime
            );
        }

        _dbContext.DoctorAvailabilities.Add(availability);

        try
        {
            await _dbContext.SaveChangesAsync(
                cancellationToken
            );
        }
        catch (DbUpdateException exception)
            when (IsUniqueConstraintViolation(exception))
        {
            return AvailabilityOverlapConflict(
                request.DayOfWeek,
                request.StartTime,
                request.EndTime
            );
        }

        var response =
            DoctorAvailabilityResponse.FromEntity(
                availability
            );

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                doctorId,
                availabilityId = availability.Id
            },
            response
        );
    }

    [HttpPut("{availabilityId:guid}")]
    [ProducesResponseType(
        typeof(DoctorAvailabilityResponse),
        StatusCodes.Status200OK
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<
        ActionResult<DoctorAvailabilityResponse>
    > Update(
        Guid doctorId,
        Guid availabilityId,
        [FromBody] UpdateDoctorAvailabilityRequest request,
        CancellationToken cancellationToken
    )
    {
        var doctorExists = await _dbContext.Doctors
            .AsNoTracking()
            .AnyAsync(
                doctor =>
                    doctor.Id == doctorId
                    && doctor.IsActive,
                cancellationToken
            );

        if (!doctorExists)
        {
            return ActiveDoctorNotFound(doctorId);
        }

        var availability =
            await _dbContext.DoctorAvailabilities
                .SingleOrDefaultAsync(
                    item =>
                        item.Id == availabilityId
                        && item.DoctorId == doctorId,
                    cancellationToken
                );

        if (availability is null)
        {
            return AvailabilityNotFound(
                doctorId,
                availabilityId
            );
        }

        var hasOverlap = await HasOverlappingAvailabilityAsync(
            doctorId,
            request.DayOfWeek,
            request.StartTime,
            request.EndTime,
            availabilityId,
            cancellationToken
        );

        if (hasOverlap)
        {
            return AvailabilityOverlapConflict(
                request.DayOfWeek,
                request.StartTime,
                request.EndTime
            );
        }

        try
        {
            availability.Update(
                request.DayOfWeek,
                request.StartTime,
                request.EndTime,
                request.SlotDurationMinutes
            );
        }
        catch (ArgumentException exception)
        {
            return InvalidAvailability(exception.Message);
        }

        try
        {
            await _dbContext.SaveChangesAsync(
                cancellationToken
            );
        }
        catch (DbUpdateException exception)
            when (IsUniqueConstraintViolation(exception))
        {
            return AvailabilityOverlapConflict(
                request.DayOfWeek,
                request.StartTime,
                request.EndTime
            );
        }

        return Ok(
            DoctorAvailabilityResponse.FromEntity(
                availability
            )
        );
    }

    [HttpDelete("{availabilityId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid doctorId,
        Guid availabilityId,
        CancellationToken cancellationToken
    )
    {
        var availability =
            await _dbContext.DoctorAvailabilities
                .SingleOrDefaultAsync(
                    item =>
                        item.Id == availabilityId
                        && item.DoctorId == doctorId,
                    cancellationToken
                );

        if (availability is null)
        {
            return AvailabilityNotFound(
                doctorId,
                availabilityId
            );
        }

        if (!availability.IsActive)
        {
            return NoContent();
        }

        availability.Deactivate();

        await _dbContext.SaveChangesAsync(
            cancellationToken
        );

        return NoContent();
    }

    private async Task<bool> HasOverlappingAvailabilityAsync(
        Guid doctorId,
        WeekDay dayOfWeek,
        TimeOnly startTime,
        TimeOnly endTime,
        Guid? ignoredAvailabilityId,
        CancellationToken cancellationToken
    )
    {
        return await _dbContext.DoctorAvailabilities
            .AsNoTracking()
            .AnyAsync(
                availability =>
                    availability.DoctorId == doctorId
                    && availability.DayOfWeek == dayOfWeek
                    && availability.IsActive
                    && (
                        ignoredAvailabilityId == null
                        || availability.Id
                            != ignoredAvailabilityId.Value
                    )
                    && availability.StartTime < endTime
                    && startTime < availability.EndTime,
                cancellationToken
            );
    }

    private ObjectResult DoctorNotFound(Guid doctorId)
    {
        return Problem(
            statusCode: StatusCodes.Status404NotFound,
            title: "Médico não encontrado.",
            detail:
                $"Não foi encontrado um médico com o ID '{doctorId}'."
        );
    }

    private ObjectResult ActiveDoctorNotFound(Guid doctorId)
    {
        return Problem(
            statusCode: StatusCodes.Status404NotFound,
            title: "Médico ativo não encontrado.",
            detail:
                $"Não foi encontrado um médico ativo com o ID '{doctorId}'."
        );
    }

    private ObjectResult AvailabilityNotFound(
        Guid doctorId,
        Guid availabilityId
    )
    {
        return Problem(
            statusCode: StatusCodes.Status404NotFound,
            title: "Disponibilidade não encontrada.",
            detail:
                $"A disponibilidade '{availabilityId}' não foi encontrada para o médico '{doctorId}'."
        );
    }

    private ObjectResult InvalidAvailability(string detail)
    {
        return Problem(
            statusCode: StatusCodes.Status400BadRequest,
            title: "Disponibilidade inválida.",
            detail: detail
        );
    }

    private ObjectResult AvailabilityOverlapConflict(
        WeekDay dayOfWeek,
        TimeOnly startTime,
        TimeOnly endTime
    )
    {
        return Problem(
            statusCode: StatusCodes.Status409Conflict,
            title: "Conflito de horários.",
            detail:
                $"O médico já possui um período que se sobrepõe a {dayOfWeek}, das {startTime:HH\\:mm} às {endTime:HH\\:mm}."
        );
    }

    private static bool IsUniqueConstraintViolation(
        DbUpdateException exception
    )
    {
        return exception.InnerException is PostgresException
        {
            SqlState: PostgresErrorCodes.UniqueViolation
        };
    }
}