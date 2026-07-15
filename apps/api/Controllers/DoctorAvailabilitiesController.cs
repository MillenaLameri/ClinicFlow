using ClinicFlow.Api.Authorization;
using ClinicFlow.Api.Contracts.DoctorAvailabilities;
using ClinicFlow.Api.Data;
using ClinicFlow.Api.Models;
using ClinicFlow.Api.Services.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ClinicFlow.Api.Controllers;

[ApiController]
[Route(
    "api/doctors/{doctorId:guid}/availabilities"
)]
[Authorize(
    Policy =
        AuthorizationPolicies.AdminOrDoctor
)]
public sealed class DoctorAvailabilitiesController
    : ControllerBase
{
    private readonly ClinicFlowDbContext
        _dbContext;

    private readonly CurrentUserService
        _currentUser;

    public DoctorAvailabilitiesController(
        ClinicFlowDbContext dbContext,
        CurrentUserService currentUser
    )
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<
        ActionResult<
            IReadOnlyCollection<
                DoctorAvailabilityResponse
            >
        >
    > GetAll(
        Guid doctorId,
        [FromQuery]
        bool includeInactive = false,
        CancellationToken cancellationToken =
            default
    )
    {
        if (!CanManageDoctor(doctorId))
        {
            return Forbid();
        }

        var doctorExists =
            await _dbContext.Doctors
                .AsNoTracking()
                .AnyAsync(
                    doctor =>
                        doctor.Id == doctorId,
                    cancellationToken
                );

        if (!doctorExists)
        {
            return DoctorNotFound(
                doctorId
            );
        }

        var query =
            _dbContext
                .DoctorAvailabilities
                .AsNoTracking()
                .Where(
                    availability =>
                        availability.DoctorId
                        == doctorId
                );

        if (!includeInactive)
        {
            query = query.Where(
                availability =>
                    availability.IsActive
            );
        }

        var availabilities =
            await query
                .OrderBy(
                    availability =>
                        availability.DayOfWeek
                )
                .ThenBy(
                    availability =>
                        availability.StartTime
                )
                .ToListAsync(
                    cancellationToken
                );

        var response = availabilities
            .Select(ToResponse)
            .ToList();

        return Ok(response);
    }

    [HttpGet("{availabilityId:guid}")]
    public async Task<
        ActionResult<
            DoctorAvailabilityResponse
        >
    > GetById(
        Guid doctorId,
        Guid availabilityId,
        CancellationToken cancellationToken
    )
    {
        if (!CanManageDoctor(doctorId))
        {
            return Forbid();
        }

        var availability =
            await _dbContext
                .DoctorAvailabilities
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    item =>
                        item.Id
                            == availabilityId
                        && item.DoctorId
                            == doctorId,
                    cancellationToken
                );

        if (availability is null)
        {
            return AvailabilityNotFound(
                availabilityId
            );
        }

        return Ok(
            ToResponse(availability)
        );
    }

    [HttpPost]
    public async Task<
        ActionResult<
            DoctorAvailabilityResponse
        >
    > Create(
        Guid doctorId,
        [FromBody]
        CreateDoctorAvailabilityRequest request,
        CancellationToken cancellationToken
    )
    {
        if (!CanManageDoctor(doctorId))
        {
            return Forbid();
        }

        var doctorExists =
            await _dbContext.Doctors
                .AsNoTracking()
                .AnyAsync(
                    doctor =>
                        doctor.Id == doctorId
                        && doctor.IsActive,
                    cancellationToken
                );

        if (!doctorExists)
        {
            return DoctorNotFound(
                doctorId
            );
        }

        DoctorAvailability availability;

        try
        {
            availability =
                new DoctorAvailability(
                    doctorId,
                    request.DayOfWeek,
                    request.StartTime,
                    request.EndTime,
                    request
                        .SlotDurationMinutes
                );
        }
        catch (ArgumentException exception)
        {
            return InvalidAvailability(
                exception.Message
            );
        }

        var hasOverlap =
            await _dbContext
                .DoctorAvailabilities
                .AsNoTracking()
                .AnyAsync(
                    item =>
                        item.DoctorId
                            == doctorId
                        && item.DayOfWeek
                            == availability
                                .DayOfWeek
                        && item.IsActive
                        && item.StartTime
                            < availability
                                .EndTime
                        && availability
                            .StartTime
                            < item.EndTime,
                    cancellationToken
                );

        if (hasOverlap)
        {
            return AvailabilityConflict();
        }

        _dbContext
            .DoctorAvailabilities
            .Add(availability);

        try
        {
            await _dbContext.SaveChangesAsync(
                cancellationToken
            );
        }
        catch (DbUpdateException exception)
            when (
                IsUniqueConstraintViolation(
                    exception
                )
            )
        {
            return AvailabilityConflict();
        }

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                doctorId,
                availabilityId =
                    availability.Id
            },
            ToResponse(availability)
        );
    }

    [HttpPut("{availabilityId:guid}")]
    public async Task<
        ActionResult<
            DoctorAvailabilityResponse
        >
    > Update(
        Guid doctorId,
        Guid availabilityId,
        [FromBody]
        UpdateDoctorAvailabilityRequest request,
        CancellationToken cancellationToken
    )
    {
        if (!CanManageDoctor(doctorId))
        {
            return Forbid();
        }

        var doctorExists =
            await _dbContext.Doctors
                .AsNoTracking()
                .AnyAsync(
                    doctor =>
                        doctor.Id == doctorId
                        && doctor.IsActive,
                    cancellationToken
                );

        if (!doctorExists)
        {
            return DoctorNotFound(
                doctorId
            );
        }

        var availability =
            await _dbContext
                .DoctorAvailabilities
                .SingleOrDefaultAsync(
                    item =>
                        item.Id
                            == availabilityId
                        && item.DoctorId
                            == doctorId,
                    cancellationToken
                );

        if (availability is null)
        {
            return AvailabilityNotFound(
                availabilityId
            );
        }

        try
        {
            availability.Update(
                request.DayOfWeek,
                request.StartTime,
                request.EndTime,
                request
                    .SlotDurationMinutes
            );
        }
        catch (ArgumentException exception)
        {
            return InvalidAvailability(
                exception.Message
            );
        }

        var hasOverlap =
            await _dbContext
                .DoctorAvailabilities
                .AsNoTracking()
                .AnyAsync(
                    item =>
                        item.Id
                            != availabilityId
                        && item.DoctorId
                            == doctorId
                        && item.DayOfWeek
                            == availability
                                .DayOfWeek
                        && item.IsActive
                        && item.StartTime
                            < availability
                                .EndTime
                        && availability
                            .StartTime
                            < item.EndTime,
                    cancellationToken
                );

        if (hasOverlap)
        {
            return AvailabilityConflict();
        }

        try
        {
            await _dbContext.SaveChangesAsync(
                cancellationToken
            );
        }
        catch (DbUpdateException exception)
            when (
                IsUniqueConstraintViolation(
                    exception
                )
            )
        {
            return AvailabilityConflict();
        }

        return Ok(
            ToResponse(availability)
        );
    }

    [HttpDelete("{availabilityId:guid}")]
    public async Task<IActionResult> Delete(
        Guid doctorId,
        Guid availabilityId,
        CancellationToken cancellationToken
    )
    {
        if (!CanManageDoctor(doctorId))
        {
            return Forbid();
        }

        var availability =
            await _dbContext
                .DoctorAvailabilities
                .SingleOrDefaultAsync(
                    item =>
                        item.Id
                            == availabilityId
                        && item.DoctorId
                            == doctorId,
                    cancellationToken
                );

        if (availability is null)
        {
            return AvailabilityNotFound(
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

    private bool CanManageDoctor(
        Guid doctorId
    )
    {
        if (_currentUser.IsAdmin)
        {
            return true;
        }

        return
            _currentUser.IsDoctor
            && _currentUser.DoctorId
                is Guid currentDoctorId
            && currentDoctorId
                == doctorId;
    }

    private static
        DoctorAvailabilityResponse
        ToResponse(
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

    private ObjectResult DoctorNotFound(
        Guid doctorId
    )
    {
        return Problem(
            statusCode:
                StatusCodes.Status404NotFound,
            title:
                "Médico não encontrado.",
            detail:
                $"Não foi encontrado um médico ativo com o ID '{doctorId}'."
        );
    }

    private ObjectResult
        AvailabilityNotFound(
            Guid availabilityId
        )
    {
        return Problem(
            statusCode:
                StatusCodes.Status404NotFound,
            title:
                "Disponibilidade não encontrada.",
            detail:
                $"Não foi encontrada uma disponibilidade com o ID '{availabilityId}'."
        );
    }

    private ObjectResult
        InvalidAvailability(
            string detail
        )
    {
        return Problem(
            statusCode:
                StatusCodes.Status400BadRequest,
            title:
                "Disponibilidade inválida.",
            detail: detail
        );
    }

    private ObjectResult
        AvailabilityConflict()
    {
        return Problem(
            statusCode:
                StatusCodes.Status409Conflict,
            title:
                "Conflito de horário.",
            detail:
                "O médico já possui uma disponibilidade ativa que se sobrepõe ao período informado."
        );
    }

    private static bool
        IsUniqueConstraintViolation(
            DbUpdateException exception
        )
    {
        return exception.InnerException
            is PostgresException
            {
                SqlState:
                    PostgresErrorCodes
                        .UniqueViolation
            };
    }
}