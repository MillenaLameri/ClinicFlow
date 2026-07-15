using ClinicFlow.Api.Authorization;
using ClinicFlow.Api.Contracts.Doctors;
using ClinicFlow.Api.Data;
using ClinicFlow.Api.Models;
using ClinicFlow.Api.Services.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ClinicFlow.Api.Controllers;

[ApiController]
[Route("api/doctors")]
[Authorize(
    Policy = AuthorizationPolicies.ClinicUser
)]
public sealed class DoctorsController
    : ControllerBase
{
    private readonly ClinicFlowDbContext
        _dbContext;

    private readonly CurrentUserService
        _currentUser;

    public DoctorsController(
        ClinicFlowDbContext dbContext,
        CurrentUserService currentUser
    )
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(
            IReadOnlyCollection<
                DoctorResponse
            >
        ),
        StatusCodes.Status200OK
    )]
    public async Task<
        ActionResult<
            IReadOnlyCollection<
                DoctorResponse
            >
        >
    > GetAll(
        [FromQuery]
        Guid? specialtyId = null,
        [FromQuery]
        string? search = null,
        [FromQuery]
        bool includeInactive = false,
        CancellationToken cancellationToken =
            default
    )
    {
        var query =
            _dbContext.Doctors
                .AsNoTracking()
                .Include(
                    doctor =>
                        doctor.Specialty
                )
                .AsQueryable();

        // Apenas Admin pode solicitar
        // médicos inativos.
        if (
            !_currentUser.IsAdmin
            || !includeInactive
        )
        {
            query = query.Where(
                doctor =>
                    doctor.IsActive
            );
        }

        // Médico enxerga somente o
        // próprio cadastro.
        if (
            _currentUser.IsDoctor
            && _currentUser.DoctorId
                is Guid currentDoctorId
        )
        {
            query = query.Where(
                doctor =>
                    doctor.Id
                    == currentDoctorId
            );
        }

        if (specialtyId.HasValue)
        {
            query = query.Where(
                doctor =>
                    doctor.SpecialtyId
                    == specialtyId.Value
            );
        }

        if (
            !string.IsNullOrWhiteSpace(
                search
            )
        )
        {
            var normalizedSearch =
                search
                    .Trim()
                    .ToLower();

            query = query.Where(
                doctor =>
                    doctor.FullName
                        .ToLower()
                        .Contains(
                            normalizedSearch
                        )
                    || doctor.CrmNumber
                        .ToLower()
                        .Contains(
                            normalizedSearch
                        )
                    || doctor.Email
                        .ToLower()
                        .Contains(
                            normalizedSearch
                        )
            );
        }

        var doctors =
            await query
                .OrderBy(
                    doctor =>
                        doctor.FullName
                )
                .Select(
                    doctor =>
                        new DoctorResponse(
                            doctor.Id,
                            doctor.FullName,
                            doctor.CrmNumber,
                            doctor.CrmState,
                            doctor.Email,
                            doctor.Phone,
                            new DoctorSpecialtyResponse(
                                doctor.Specialty.Id,
                                doctor.Specialty.Name
                            ),
                            doctor.IsActive,
                            doctor.CreatedAtUtc
                        )
                )
                .ToListAsync(
                    cancellationToken
                );

        return Ok(doctors);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(
        typeof(DoctorResponse),
        StatusCodes.Status200OK
    )]
    [ProducesResponseType(
        StatusCodes.Status403Forbidden
    )]
    [ProducesResponseType(
        StatusCodes.Status404NotFound
    )]
    public async Task<
        ActionResult<DoctorResponse>
    > GetById(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        // Um médico não pode consultar
        // diretamente o cadastro de outro.
        if (
            _currentUser.IsDoctor
            && _currentUser.DoctorId
                is Guid currentDoctorId
            && currentDoctorId != id
        )
        {
            return Forbid();
        }

        var doctor =
            await _dbContext.Doctors
                .AsNoTracking()
                .Include(
                    item =>
                        item.Specialty
                )
                .SingleOrDefaultAsync(
                    item =>
                        item.Id == id,
                    cancellationToken
                );

        if (doctor is null)
        {
            return DoctorNotFound(id);
        }

        // Paciente não deve acessar
        // médicos desativados.
        if (
            _currentUser.IsPatient
            && !doctor.IsActive
        )
        {
            return DoctorNotFound(id);
        }

        return Ok(
            DoctorResponse.FromEntity(
                doctor
            )
        );
    }

    [HttpPost]
    [Authorize(
        Policy =
            AuthorizationPolicies.AdminOnly
    )]
    [ProducesResponseType(
        typeof(DoctorResponse),
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
    [ProducesResponseType(
        StatusCodes.Status403Forbidden
    )]
    public async Task<
        ActionResult<DoctorResponse>
    > Create(
        [FromBody]
        CreateDoctorRequest request,
        CancellationToken cancellationToken
    )
    {
        if (
            request.SpecialtyId
            == Guid.Empty
        )
        {
            ModelState.AddModelError(
                nameof(
                    request.SpecialtyId
                ),
                "A especialidade é obrigatória."
            );

            return ValidationProblem(
                ModelState
            );
        }

        var specialty =
            await _dbContext.Specialties
                .SingleOrDefaultAsync(
                    item =>
                        item.Id
                            == request
                                .SpecialtyId
                        && item.IsActive,
                    cancellationToken
                );

        if (specialty is null)
        {
            return SpecialtyNotFound(
                request.SpecialtyId
            );
        }

        var normalizedCrmNumber =
            request.CrmNumber
                .Trim()
                .ToUpperInvariant();

        var normalizedCrmState =
            request.CrmState
                .Trim()
                .ToUpperInvariant();

        var normalizedEmail =
            request.Email
                .Trim()
                .ToLowerInvariant();

        var crmAlreadyExists =
            await _dbContext.Doctors
                .AnyAsync(
                    doctor =>
                        doctor.CrmNumber
                            == normalizedCrmNumber
                        && doctor.CrmState
                            == normalizedCrmState,
                    cancellationToken
                );

        if (crmAlreadyExists)
        {
            return DoctorCrmConflict(
                normalizedCrmNumber,
                normalizedCrmState
            );
        }

        var emailAlreadyExists =
            await _dbContext.Doctors
                .AnyAsync(
                    doctor =>
                        doctor.Email
                        == normalizedEmail,
                    cancellationToken
                );

        if (emailAlreadyExists)
        {
            return DoctorEmailConflict(
                normalizedEmail
            );
        }

        Doctor doctor;

        try
        {
            doctor = new Doctor(
                request.FullName,
                normalizedCrmNumber,
                normalizedCrmState,
                normalizedEmail,
                request.Phone,
                request.SpecialtyId
            );
        }
        catch (ArgumentException exception)
        {
            return InvalidDoctorData(
                exception.Message
            );
        }

        _dbContext.Doctors.Add(
            doctor
        );

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
            return Problem(
                statusCode:
                    StatusCodes
                        .Status409Conflict,
                title:
                    "Médico já cadastrado.",
                detail:
                    "Já existe um médico utilizando o CRM ou e-mail informado."
            );
        }

        var response =
            DoctorResponse.FromEntity(
                doctor,
                specialty
            );

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                id = doctor.Id
            },
            response
        );
    }

    [HttpPut("{id:guid}")]
    [Authorize(
        Policy =
            AuthorizationPolicies.AdminOnly
    )]
    [ProducesResponseType(
        typeof(DoctorResponse),
        StatusCodes.Status200OK
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
    [ProducesResponseType(
        StatusCodes.Status403Forbidden
    )]
    public async Task<
        ActionResult<DoctorResponse>
    > Update(
        Guid id,
        [FromBody]
        UpdateDoctorRequest request,
        CancellationToken cancellationToken
    )
    {
        var doctor =
            await _dbContext.Doctors
                .SingleOrDefaultAsync(
                    item =>
                        item.Id == id,
                    cancellationToken
                );

        if (doctor is null)
        {
            return DoctorNotFound(id);
        }

        if (
            request.SpecialtyId
            == Guid.Empty
        )
        {
            ModelState.AddModelError(
                nameof(
                    request.SpecialtyId
                ),
                "A especialidade é obrigatória."
            );

            return ValidationProblem(
                ModelState
            );
        }

        var specialty =
            await _dbContext.Specialties
                .SingleOrDefaultAsync(
                    item =>
                        item.Id
                            == request
                                .SpecialtyId
                        && item.IsActive,
                    cancellationToken
                );

        if (specialty is null)
        {
            return SpecialtyNotFound(
                request.SpecialtyId
            );
        }

        var normalizedEmail =
            request.Email
                .Trim()
                .ToLowerInvariant();

        var emailAlreadyExists =
            await _dbContext.Doctors
                .AnyAsync(
                    item =>
                        item.Email
                            == normalizedEmail
                        && item.Id != id,
                    cancellationToken
                );

        if (emailAlreadyExists)
        {
            return DoctorEmailConflict(
                normalizedEmail
            );
        }

        try
        {
            doctor.UpdateProfile(
                request.FullName,
                normalizedEmail,
                request.Phone,
                request.SpecialtyId
            );
        }
        catch (ArgumentException exception)
        {
            return InvalidDoctorData(
                exception.Message
            );
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
            return DoctorEmailConflict(
                normalizedEmail
            );
        }

        return Ok(
            DoctorResponse.FromEntity(
                doctor,
                specialty
            )
        );
    }

    [HttpDelete("{id:guid}")]
    [Authorize(
        Policy =
            AuthorizationPolicies.AdminOnly
    )]
    [ProducesResponseType(
        StatusCodes.Status204NoContent
    )]
    [ProducesResponseType(
        StatusCodes.Status404NotFound
    )]
    [ProducesResponseType(
        StatusCodes.Status403Forbidden
    )]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        var doctor =
            await _dbContext.Doctors
                .SingleOrDefaultAsync(
                    item =>
                        item.Id == id,
                    cancellationToken
                );

        if (doctor is null)
        {
            return DoctorNotFound(id);
        }

        if (!doctor.IsActive)
        {
            return NoContent();
        }

        doctor.Deactivate();
        
        var linkedUser =
            await _dbContext.Users
                .SingleOrDefaultAsync(
                    user =>
                        user.DoctorId == id,
                    cancellationToken
                );

        if (linkedUser is not null)
        {
            linkedUser.Deactivate();
        }

        await _dbContext.SaveChangesAsync(
            cancellationToken
        );

        return NoContent();
    }

    private ObjectResult
        DoctorNotFound(
            Guid id
        )
    {
        return Problem(
            statusCode:
                StatusCodes
                    .Status404NotFound,
            title:
                "Médico não encontrado.",
            detail:
                $"Não foi encontrado um médico com o ID '{id}'."
        );
    }

    private ObjectResult
        SpecialtyNotFound(
            Guid id
        )
    {
        return Problem(
            statusCode:
                StatusCodes
                    .Status404NotFound,
            title:
                "Especialidade não encontrada.",
            detail:
                $"Não foi encontrada uma especialidade ativa com o ID '{id}'."
        );
    }

    private ObjectResult
        DoctorCrmConflict(
            string crmNumber,
            string crmState
        )
    {
        return Problem(
            statusCode:
                StatusCodes
                    .Status409Conflict,
            title:
                "CRM já cadastrado.",
            detail:
                $"Já existe um médico com o CRM {crmNumber}/{crmState}."
        );
    }

    private ObjectResult
        DoctorEmailConflict(
            string email
        )
    {
        return Problem(
            statusCode:
                StatusCodes
                    .Status409Conflict,
            title:
                "E-mail já cadastrado.",
            detail:
                $"Já existe um médico utilizando o e-mail '{email}'."
        );
    }

    private ObjectResult
        InvalidDoctorData(
            string detail
        )
    {
        return Problem(
            statusCode:
                StatusCodes
                    .Status400BadRequest,
            title:
                "Dados do médico inválidos.",
            detail: detail
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